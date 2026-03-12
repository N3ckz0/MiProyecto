using TG.Blazor.IndexedDB;
using MiProyecto.Web.Models;
using System.Text.Json;

namespace MiProyecto.Web.Services;

public class CambioProductoService
{
    private readonly IndexedDBManager _db;

    public CambioProductoService(IndexedDBManager db)
    {
        _db = db;
    }

    public async Task RegistrarCambio(string operacion, ProductoLocal producto)
    {
        Console.WriteLine($"RegistrarCambio -> ID:{producto.Id_producto} | Nombre:{producto.Nombre}");
        Console.WriteLine(Environment.StackTrace);

        if (producto.Id_producto == 0)
            return;

        var cambio = new CambioProducto
        {
            ProductoId = producto.Id_producto,
            Operacion = operacion.ToLower(),
            DatosJson = JsonSerializer.Serialize(producto),
            Fecha = DateTime.UtcNow,
            Sincronizado = false
        };

        await _db.AddRecord(new StoreRecord<CambioProducto>
        {
            Storename = "cambios_producto",
            Data = cambio
        });
    }

    public async Task<List<CambioProducto>> ObtenerPendientes()
    {
        var todos = await _db.GetRecords<CambioProducto>("cambios_producto");

        return todos
            .Where(c => !c.Sincronizado)
            .OrderBy(c => c.Fecha)
            .ToList();
    }

    public async Task<List<CambioProducto>> ObtenerTodos()
    {
        var todos = await _db.GetRecords<CambioProducto>("cambios_producto");

        return todos
            .OrderBy(c => c.Fecha)
            .ToList();
    }

    public async Task MarcarSincronizado(int id)
    {
        var cambio = await _db.GetRecordById<int, CambioProducto>("cambios_producto", id);
        if (cambio != null)
        {
            cambio.Sincronizado = true;
            await _db.UpdateRecord(new StoreRecord<CambioProducto>
            {
                Storename = "cambios_producto",
                Data = cambio
            });
        }
    }
}