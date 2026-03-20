using System.Net.Http.Json;
using MiProyecto.Domain.Entities;
using System.Text.Json;

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

    // Obtiene los productos
    public async Task<List<Producto>> GetProductosAsync()
    {
        if (await _networkService.IsOnline())
        {
            await _queueService.ProcessQueueAsync();
            try
            {
                var productos = await _http.GetFromJsonAsync<List<Producto>>("api/producto")
                               ?? new List<Producto>();

                // Guardar cache local
                await _cacheService.SaveProductosAsync(productos);
                return productos;
            }
            catch
            {
                // En caso de error, usar cache
                return await _cacheService.GetProductosAsync();
            }
        }
        else
        {
            // Offline → usar cache
            return await _cacheService.GetProductosAsync();
        }
    }

    // Crear producto
    public async Task<bool> CrearProductoAsync(Producto producto)
    {
        if (await _networkService.IsOnline())
        {
            await _queueService.ProcessQueueAsync();
            try
            {
                var response = await _http.PostAsJsonAsync("api/producto", producto);
                if (response.IsSuccessStatusCode)
                {
                    // Actualizar cache
                    var productos = await GetProductosAsync();
                    productos.Add(producto);
                    await _cacheService.SaveProductosAsync(productos);
                    return true;
                }
                return false;
            }
            catch
            {
                // Offline → encolar operación
                await _queueService.EnqueueAsync(new OfflineOperation
                {
                    OperationType = "Create",
                    EntityType = "Producto",
                    EntityData = JsonSerializer.Serialize(producto),
                    CreatedAt = DateTime.UtcNow
                });
                return false;
            }
        }
        else
        {
            await _queueService.EnqueueAsync(new OfflineOperation
            {
                OperationType = "Create",
                EntityType = "Producto",
                EntityData = JsonSerializer.Serialize(producto),
                CreatedAt = DateTime.UtcNow
            });

            // Actualizar cache local
            var productos = await _cacheService.GetProductosAsync();
            productos.Add(producto);
            await _cacheService.SaveProductosAsync(productos);

            return false;
        }
    }

    // Editar producto
    public async Task<bool> EditarProductoAsync(Producto producto)
    {
        if (await _networkService.IsOnline())
        {
            await _queueService.ProcessQueueAsync();
            try
            {
                var response = await _http.PutAsJsonAsync($"api/producto/{producto.Id_producto}", producto);
                if (response.IsSuccessStatusCode)
                {
                    var productos = await GetProductosAsync();
                    var index = productos.FindIndex(p => p.Id_producto == producto.Id_producto);
                    if (index >= 0) productos[index] = producto;
                    await _cacheService.SaveProductosAsync(productos);
                    return true;
                }
                return false;
            }
            catch
            {
                await EnqueueOfflineEdit(producto);
                return false;
            }
        }
        else
        {
            await EnqueueOfflineEdit(producto);
            var productos = await _cacheService.GetProductosAsync();
            var index = productos.FindIndex(p => p.Id_producto == producto.Id_producto);
            if (index >= 0) productos[index] = producto;
            await _cacheService.SaveProductosAsync(productos);
            return false;
        }
    }

    private async Task EnqueueOfflineEdit(Producto producto)
    {
        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Update",
            EntityType = "Producto",
            EntityData = JsonSerializer.Serialize(producto),
            CreatedAt = DateTime.UtcNow
        });
    }

    // Eliminar producto
    public async Task<bool> EliminarProductoAsync(int id)
    {
        if (await _networkService.IsOnline())
        {
            await _queueService.ProcessQueueAsync();
            try
            {
                var response = await _http.DeleteAsync($"api/producto/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var productos = await GetProductosAsync();
                    productos.RemoveAll(p => p.Id_producto == id);
                    await _cacheService.SaveProductosAsync(productos);
                    return true;
                }
                return false;
            }
            catch
            {
                await EnqueueOfflineDelete(id);
                return false;
            }
        }
        else
        {
            await EnqueueOfflineDelete(id);
            var productos = await _cacheService.GetProductosAsync();
            productos.RemoveAll(p => p.Id_producto == id);
            await _cacheService.SaveProductosAsync(productos);
            return false;
        }
    }

    private async Task EnqueueOfflineDelete(int id)
    {
        await _queueService.EnqueueAsync(new OfflineOperation
        {
            OperationType = "Delete",
            EntityType = "Producto",
            EntityData = JsonSerializer.Serialize(new { Id_producto = id }),
            CreatedAt = DateTime.UtcNow
        });
    }

    // Obtener producto por id
    public async Task<Producto?> GetProductoByIdAsync(int id)
    {
        var productos = await GetProductosAsync();
        return productos.FirstOrDefault(p => p.Id_producto == id);
    }
}