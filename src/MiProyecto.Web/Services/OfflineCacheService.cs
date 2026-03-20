using Microsoft.JSInterop;
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

    // 🔥 AHORA USA DTO
    public async Task SaveProductosAsync(List<ProductoDto> productos)
    {
        try
        {
            if (productos == null) return;

            string json = JsonSerializer.Serialize(productos);
            await _js.InvokeVoidAsync("offlineService.saveItems", STORE_NAME, json);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error guardando productos en cache: {ex.Message}");
        }
    }

    public async Task<List<ProductoDto>> GetProductosAsync()
    {
        try
        {
            string json = await _js.InvokeAsync<string>("offlineService.getItems", STORE_NAME);

            if (string.IsNullOrEmpty(json))
                return new List<ProductoDto>();

            return JsonSerializer.Deserialize<List<ProductoDto>>(json)
                   ?? new List<ProductoDto>();
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error obteniendo productos del cache: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    public async Task<ProductoDto?> GetProductoByIdAsync(int id)
    {
        try
        {
            var productos = await GetProductosAsync();
            return productos.FirstOrDefault(p => p.Id_producto == id);
        }
        catch (JSException ex)
        {
            Console.WriteLine($"Error al obtener producto desde cache: {ex.Message}");
            return null;
        }
    }

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