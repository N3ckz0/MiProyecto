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

        foreach (var op in queue)
        {
            try
            {

                switch (op.OperationType)
                {
                    case "Create":
                        var productoCreate = JsonSerializer.Deserialize<Producto>(op.EntityData);

                        if (productoCreate != null)
                        {
                            var response = await _http.PostAsJsonAsync("api/producto", productoCreate);

                            if (response.IsSuccessStatusCode)
                            {
                                var createdFromApi = await response.Content.ReadFromJsonAsync<Producto>();

                                if (createdFromApi != null)
                                {
                                    // 🔥 reemplazar el offline
                                    var local = productos.FirstOrDefault(p =>
                                        p.Nombre == productoCreate.Nombre &&
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
                // se reintenta después
            }
        }

        // 🔥 guardar estado final limpio
        await _cacheService.SaveProductosAsync(productos);

        // 🔥 limpiar cola
        await ClearQueueAsync();
    }
}