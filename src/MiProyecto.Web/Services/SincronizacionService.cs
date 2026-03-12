using MiProyecto.Web.Models;
using MiProyecto.Web.Services;
using System.Net.Http.Json;
using MiProyecto.Domain;
using System.Text.Json;

public class SincronizacionService
{
    private readonly ProductoOfflineService _offlineService;
    private readonly CambioProductoService _cambios;
    private readonly HttpClient _http;
    private readonly NetworkService _network;    

    public SincronizacionService( ProductoOfflineService offlineService, CambioProductoService cambios, HttpClient http, NetworkService network)
    {
        _offlineService = offlineService;
        _cambios = cambios;
        _http = http;
        _network = network;
    }

    public async Task<List<Producto>> SincronizarProductos()
    {
        var resultado = new List<Producto>();

        if (!await _network.IsOnline())
            return resultado;

        var cambios = await _cambios.ObtenerPendientes();

        // evitar registros corruptos con ProductoId = 0
        cambios = cambios
            .Where(c => !c.Sincronizado && c.ProductoId != 0)
            .ToList();

        Console.WriteLine($"Pendientes válidos: {cambios.Count}");

        if (!cambios.Any())
            return resultado;

        Console.WriteLine($"SYNC -> Cambios pendientes: {cambios.Count}");

        var grupos = cambios
            .GroupBy(c => c.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                Cambios = g.OrderBy(x => x.Fecha).ToList(),
                Ultimo = g.OrderByDescending(x => x.Fecha).First()
            })
            .ToList();

        foreach (var grupo in grupos)
        {
            try
            {
                var tieneCrear = grupo.Cambios.Any(x => x.Operacion == "crear");
                var tieneEliminar = grupo.Cambios.Any(x => x.Operacion == "eliminar");

                Console.WriteLine($"SYNC -> Producto {grupo.ProductoId}");

                if (tieneCrear && tieneEliminar)
                {
                    Console.WriteLine("CREATE + DELETE -> ignorado");

                    foreach (var c in grupo.Cambios)
                        await _cambios.MarcarSincronizado(c.Id);

                    continue;
                }

                var producto = JsonSerializer.Deserialize<Producto>(grupo.Ultimo.DatosJson);

                if (producto == null)
                    continue;

                if (tieneEliminar)
                {
                    await _http.DeleteAsync($"api/producto/{grupo.ProductoId}");
                }
                else if (tieneCrear)
                {
                    var response = await _http.PostAsJsonAsync("api/producto", producto);

                    if (!response.IsSuccessStatusCode)
                        continue;

                    var servidor = await response.Content.ReadFromJsonAsync<Producto>();

                    if (servidor != null)
                    {
                        resultado.Add(servidor);

                        await _offlineService.ActualizarIdTemporal(
                            producto.Id_producto,
                            servidor.Id_producto);
                    }
                }
                else
                {
                    await _http.PutAsJsonAsync($"api/producto/{producto.Id_producto}", producto);
                }

                foreach (var c in grupo.Cambios)
                    await _cambios.MarcarSincronizado(c.Id);

                await _offlineService.MarcarComoSincronizado(grupo.ProductoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SYNC ERROR -> {grupo.ProductoId} : {ex.Message}");
            }
        }

        return resultado;
    }

}