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
            Console.WriteLine($"Guardando producto: {producto.Id_producto} - {producto.Nombre}");
        await _db.AddRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });
            Console.WriteLine("Producto guardado");
    }

    public async Task<List<ProductoLocal>> ObtenerTodosAsync()
    {
        return await _db.GetRecords<ProductoLocal>("productos");
    }
}