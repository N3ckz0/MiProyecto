using Microsoft.JSInterop;
using MiProyecto.Domain.Entities;
using System.Text.Json;

public class OfflineCacheService
{
    private readonly IJSRuntime _js;
    private const string DB_NAME = "MiProyectoOfflineDB";
    private const string STORE_NAME = "Productos";

    public OfflineCacheService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Inicializa la base de datos y el object store de productos.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("offlineService.initDB", DB_NAME, STORE_NAME);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error inicializando IndexedDB: {ex.Message}");
        }
    }

    /// <summary>
    /// Guarda la lista de productos en IndexedDB.
    /// </summary>
    public async Task SaveProductosAsync(List<Producto> productos)
    {
        try
        {
            if (productos == null || !productos.Any()) return;
            string json = JsonSerializer.Serialize(productos);
            await _js.InvokeVoidAsync("offlineService.saveItems", "Productos", json);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error guardando productos en cache: {ex.Message}");
        }
    }

    public async Task<List<Producto>> GetProductosAsync()
    {
        try
        {
            string json = await _js.InvokeAsync<string>("offlineService.getItems", "Productos");
            if (string.IsNullOrEmpty(json)) return new List<Producto>();
            return JsonSerializer.Deserialize<List<Producto>>(json) ?? new List<Producto>();
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error obteniendo productos del cache: {ex.Message}");
            return new List<Producto>();
        }
    }

    public async Task<Producto?> GetProductoByIdAsync(int id)
    {
        try
        {
            // Obtener todos los productos desde IndexedDB
            var productos = await GetProductosAsync();
            // Buscar el producto con el Id solicitado
            return productos.FirstOrDefault(p => p.Id_producto == id);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error al obtener producto desde cache: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Limpia todos los productos del cache.
    /// </summary>
    public async Task ClearAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("offlineService.clearStore", DB_NAME, STORE_NAME);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error limpiando cache de productos: {ex.Message}");
        }
    }
}