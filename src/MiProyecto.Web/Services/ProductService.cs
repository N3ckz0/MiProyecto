using System.Net.Http.Json;
using System.Text.Json;
using MiProyecto.Domain.Entities;

public class ProductoService
{
    private readonly HttpClient _http;
    private readonly OfflineCacheService _cacheService;
    private readonly OfflineQueueService _queueService;
    private readonly NetworkService _networkService;

    public ProductoService(
        HttpClient http,
        OfflineCacheService cacheService,
        OfflineQueueService queueService,
        NetworkService networkService)
    {
        _http = http;
        _cacheService = cacheService;
        _queueService = queueService;
        _networkService = networkService;
    }

    // 🔥 GET CON MERGE REAL
    public async Task<List<ProductoDto>> GetProductosAsync()
    {
        var local = await _cacheService.GetProductosAsync();

        if (await _networkService.IsOnline())
        {

            try
            {
                var api = await _http.GetFromJsonAsync<List<Producto>>("api/producto")
                          ?? new List<Producto>();

                var apiDtos = api.Select(p => new ProductoDto
                {
                    Id_producto = p.Id_producto,
                    Nombre = p.Nombre ?? "",
                    Precio = p.Precio ?? 0,
                    SyncStatus = SyncStatus.Synced
                }).ToList();

                // 🔥 conservar offline
                var offline = local
                    .Where(p => p.SyncStatus != SyncStatus.Synced)
                    .ToList();

                var merged = apiDtos.Concat(offline).ToList();

                await _cacheService.SaveProductosAsync(merged);

                return merged;
            }
            catch
            {
                return local;
            }
        }

        return local;
    }

    // 🔥 CREAR
    public async Task<bool> CrearProductoAsync(Producto producto)
    {
        if (await _networkService.IsOnline())
        {

            try
            {
                var response = await _http.PostAsJsonAsync("api/producto", producto);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch { }
        }

        // 🔥 OFFLINE
        var dto = new ProductoDto
        {
            Id_producto = (int)(-DateTime.Now.Ticks % int.MaxValue),
            Nombre = producto.Nombre ?? "",
            SyncStatus = SyncStatus.PendingCreate
        };

        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Create",
            EntityType = "Producto",
            EntityData = JsonSerializer.Serialize(producto),
            CreatedAt = DateTime.UtcNow
        });

        var productos = await _cacheService.GetProductosAsync();
        productos.Add(dto);
        await _cacheService.SaveProductosAsync(productos);

        return false;
    }

    // 🔥 EDITAR
    public async Task<bool> EditarProductoAsync(Producto producto)
    {
        if (await _networkService.IsOnline())
        {

            try
            {
                var response = await _http.PutAsJsonAsync($"api/producto/{producto.Id_producto}", producto);

                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch { }
        }

        // OFFLINE
        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Update",
            EntityType = "Producto",
            EntityData = JsonSerializer.Serialize(producto),
            CreatedAt = DateTime.UtcNow
        });

        var productos = await _cacheService.GetProductosAsync();

        var index = productos.FindIndex(p => p.Id_producto == producto.Id_producto);
        if (index >= 0)
        {
            productos[index].Nombre = producto.Nombre ?? "";
            productos[index].SyncStatus = SyncStatus.PendingUpdate;
        }

        await _cacheService.SaveProductosAsync(productos);

        return false;
    }

    // 🔥 ELIMINAR
    public async Task<bool> EliminarProductoAsync(int id)
    {
        if (await _networkService.IsOnline())
        {

            try
            {
                var response = await _http.DeleteAsync($"api/producto/{id}");

                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch { }
        }

        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Delete",
            EntityType = "Producto",
            EntityData = JsonSerializer.Serialize(new { Id_producto = id }),
            CreatedAt = DateTime.UtcNow
        });

        var productos = await _cacheService.GetProductosAsync();

        var item = productos.FirstOrDefault(p => p.Id_producto == id);
        if (item != null)
        {
            item.SyncStatus = SyncStatus.PendingDelete;
        }

        await _cacheService.SaveProductosAsync(productos);

        return false;
    }

    public async Task<ProductoDto?> GetProductoByIdAsync(int id)
    {
        var productos = await GetProductosAsync();
        return productos.FirstOrDefault(p => p.Id_producto == id);
    }

    // Recibe lista de ProductoDto
    public async Task GuardarProductosDtoEnCacheAsync(List<ProductoDto> productosDto)
    {
        await _cacheService.SaveProductosAsync(productosDto);
    }

    // Recibe lista de Producto (si en algún caso necesitas convertir)
    public async Task GuardarProductosEnCacheAsync(List<Producto> productos)
    {
        // Convertimos Producto a ProductoDto para guardarlo en el cache
        var productosDto = productos.Select(p => new ProductoDto
        {
            Id_producto = p.Id_producto,
            Nombre = p.Nombre ?? "",
            Precio = p.Precio ?? 0
        }).ToList();

        await _cacheService.SaveProductosAsync(productosDto);
    }
}