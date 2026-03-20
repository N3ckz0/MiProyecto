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

    /// <summary>
    /// Procesa la cola de operaciones pendientes y las sincroniza con la API.
    /// </summary>
    public async Task ProcessQueueAsync()
    {
        var queue = await GetQueueAsync();
        if (!queue.Any()) return;

        foreach (var op in queue)
        {
            try
            {
                bool success = false;
                switch (op.OperationType)
                {
                    case "Create":
                        var productoCreate = JsonSerializer.Deserialize<Producto>(op.EntityData);
                        if (productoCreate != null)
                        {
                            var response = await _http.PostAsJsonAsync("api/producto", productoCreate);
                            success = response.IsSuccessStatusCode;
                        }
                        break;

                    case "Update":
                        var productoUpdate = JsonSerializer.Deserialize<Producto>(op.EntityData);
                        if (productoUpdate != null)
                        {
                            var response = await _http.PutAsJsonAsync($"api/producto/{productoUpdate.Id_producto}", productoUpdate);
                            success = response.IsSuccessStatusCode;
                        }
                        break;

                    case "Delete":
                        var obj = JsonSerializer.Deserialize<JsonElement>(op.EntityData);
                        if (obj.TryGetProperty("Id_producto", out var idProp))
                        {
                            int id = idProp.GetInt32();
                            var response = await _http.DeleteAsync($"api/producto/{id}");
                            success = response.IsSuccessStatusCode;
                        }
                        break;
                }

                if (success)
                {
                    // Actualizar cache local
                    var productos = await _cacheService.GetProductosAsync();
                    switch (op.OperationType)
                    {
                        case "Create":
                            var created = JsonSerializer.Deserialize<Producto>(op.EntityData);
                            if (created != null) productos.Add(created);
                            break;
                        case "Update":
                            var updated = JsonSerializer.Deserialize<Producto>(op.EntityData);
                            if (updated != null)
                            {
                                var index = productos.FindIndex(p => p.Id_producto == updated.Id_producto);
                                if (index >= 0) productos[index] = updated;
                            }
                            break;
                        case "Delete":
                            var obj = JsonSerializer.Deserialize<JsonElement>(op.EntityData);
                            if (obj.TryGetProperty("Id_producto", out var deleteIdProp))
                            {
                                int deleteId = deleteIdProp.GetInt32();
                                productos.RemoveAll(p => p.Id_producto == deleteId);
                            }
                            break;
                    }
                    await _cacheService.SaveProductosAsync(productos);
                }
            }
            catch
            {
                // Si falla, simplemente se queda en la cola para intentar después
            }
        }

        // Limpiar cola procesada
        await ClearQueueAsync();
    }
}