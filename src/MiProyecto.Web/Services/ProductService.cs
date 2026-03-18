using System.Net.Http.Json;
using MiProyecto.Domain.Entities;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }
    //Obtiene los productos de la API
    public async Task<List<Producto>> GetProductosAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Producto>>("api/producto")
                   ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
    //Obtiene un producto con la API
    public async Task<Producto?> GetProductoByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<Producto>($"api/producto/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener producto: {ex.Message}");
            return null;
        }
    }
    //Crea un producto con la API
    public async Task<bool> CrearProductoAsync(Producto producto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/producto", producto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    //Edita un producto con la API
    public async Task<bool> EditarProductoAsync(Producto producto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/producto/{producto.Id_producto}", producto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    //Elimina un producto con la API
    public async Task<bool> EliminarProductoAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/producto/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}