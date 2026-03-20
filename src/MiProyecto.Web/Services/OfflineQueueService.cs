using Microsoft.JSInterop;
using MiProyecto.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;

public class OfflineQueueService
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;
    private readonly OfflineCacheService _cacheService;
    private const string STORE_QUEUE = "Queue";

    public OfflineQueueService(IJSRuntime js, HttpClient http, OfflineCacheService cacheService)
    {
        _js = js;
        _http = http;
        _cacheService = cacheService;
    }

    public async Task InitializeAsync()
    {
        await _js.InvokeVoidAsync("offlineService.initDB");
    }

    public async Task EnqueueAsync(OfflineOperation op)
    {
        string json = JsonSerializer.Serialize(op);
        await _js.InvokeVoidAsync("offlineService.addToQueue", STORE_QUEUE, json);
    }

    public async Task<List<OfflineOperation>> GetQueueAsync()
    {
        string json = await _js.InvokeAsync<string>("offlineService.getQueue", STORE_QUEUE);
        if (string.IsNullOrEmpty(json)) return new List<OfflineOperation>();
        return JsonSerializer.Deserialize<List<OfflineOperation>>(json) ?? new List<OfflineOperation>();
    }

    public async Task ClearQueueAsync()
    {
        await _js.InvokeVoidAsync("offlineService.clearStore", STORE_QUEUE);
    }

    // 🔥 PROCESO DE SINCRONIZACIÓN REAL
    public async Task ProcessQueueAsync()
    {
        var queue = await GetQueueAsync();
        if (!queue.Any()) return;

        var productos = await _cacheService.GetProductosAsync();
        var operationsByProduct = new Dictionary<int, OfflineOperation>();

        // Agrupamos las operaciones por producto
        foreach (var op in queue)
        {
            var producto = JsonSerializer.Deserialize<Producto>(op.EntityData);
            if (producto == null) continue;

            // Si el producto ya tiene una operación en la cola, decidimos qué hacer
            if (operationsByProduct.ContainsKey(producto.Id_producto))
            {
                var existingOp = operationsByProduct[producto.Id_producto];

                // Prioridad de operaciones (Create > Update > Delete)
                if (op.OperationType == "Delete")
                {
                    // Si hay una operación Delete, no la combinamos con Create o Update
                    operationsByProduct[producto.Id_producto] = op;
                }
                else if (op.OperationType == "Update" && existingOp.OperationType != "Create")
                {
                    // Si ya existe una operación Create, mantenemos la Create, si es solo Update lo reemplazamos
                    operationsByProduct[producto.Id_producto] = op;
                }
                // Si es un Create y no hay ninguna operación anterior, lo agregamos
                else if (op.OperationType == "Create" && existingOp.OperationType != "Create")
                {
                    operationsByProduct[producto.Id_producto] = op;
                }
            }
            else
            {
                // Si no existe una operación para este producto, la agregamos
                operationsByProduct[producto.Id_producto] = op;
            }
        }

        // Ahora procesamos las operaciones optimizadas (coalesced)
        foreach (var op in operationsByProduct.Values)
        {
            try
            {
                switch (op.OperationType)
                {
                    case "Create":
                        var productoCreate = JsonSerializer.Deserialize<Producto>(op.EntityData);

                        if (productoCreate != null)
                        {
                            var response = await _http.PostAsJsonAsync("api/producto", new Producto
                            {
                                Nombre = productoCreate.Nombre,
                                Precio = productoCreate.Precio
                            });

                            if (response.IsSuccessStatusCode)
                            {
                                var createdFromApi = await response.Content.ReadFromJsonAsync<Producto>();

                                if (createdFromApi != null)
                                {
                                    // 🔥 reemplazar el offline
                                    var local = productos.FirstOrDefault(p =>
                                        p.Id_producto == productoCreate.Id_producto &&
                                        p.SyncStatus == SyncStatus.PendingCreate);

                                    if (local != null)
                                    {
                                        local.Id_producto = createdFromApi.Id_producto;
                                        local.SyncStatus = SyncStatus.Synced;
                                    }
                                }

                            }
                        }
                        break;

                    case "Update":
                        var productoUpdate = JsonSerializer.Deserialize<Producto>(op.EntityData);

                        if (productoUpdate != null)
                        {
                            var response = await _http.PutAsJsonAsync(
                                $"api/producto/{productoUpdate.Id_producto}",
                                productoUpdate);

                            if (response.IsSuccessStatusCode)
                            {
                                var local = productos.FirstOrDefault(p =>
                                    p.Id_producto == productoUpdate.Id_producto);

                                if (local != null)
                                {
                                    local.SyncStatus = SyncStatus.Synced;
                                }
                            }
                        }
                        break;

                    case "Delete":
                        var obj = JsonSerializer.Deserialize<JsonElement>(op.EntityData);

                        if (obj.TryGetProperty("Id_producto", out var idProp))
                        {
                            int id = idProp.GetInt32();

                            var response = await _http.DeleteAsync($"api/producto/{id}");

                            if (response.IsSuccessStatusCode)
                            {
                                productos.RemoveAll(p => p.Id_producto == id);
                            }
                        }
                        break;
                }
            }
            catch
            {
                // Si hay error, la operación se reintentará más tarde
            }
        }

        // 🔥 guardar estado final limpio
        await _cacheService.SaveProductosAsync(productos);

        // 🔥 limpiar cola
        await ClearQueueAsync();
    }
}