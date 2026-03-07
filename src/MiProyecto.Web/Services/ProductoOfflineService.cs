using TG.Blazor.IndexedDB;
using MiProyecto.Web.Models;

namespace MiProyecto.Web.Services;

public class ProductoOfflineService
{
    private readonly IndexedDBManager _db;

    public ProductoOfflineService(IndexedDBManager db)
    {
        _db = db;
    }

    public async Task GuardarAsync(ProductoLocal producto)
    {
        await _db.UpdateRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });
    }

    public async Task GuardarListaAsync(List<ProductoLocal> productos)
    {
        foreach (var producto in productos)
        {
            await GuardarAsync(producto);
        }
    }

    public async Task<List<ProductoLocal>> ObtenerTodosAsync()
    {
        return await _db.GetRecords<ProductoLocal>("productos");
    }

    public async Task EliminarAsync(int id)
    {
        await _db.DeleteRecord("productos", id);
    }

    public async Task LimpiarAsync()
    {
        await _db.ClearStore("productos");
    }
}