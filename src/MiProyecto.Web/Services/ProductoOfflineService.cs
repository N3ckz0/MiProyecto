using TG.Blazor.IndexedDB;
using MiProyecto.Web.Models;

namespace MiProyecto.Web.Services;

public class ProductoOfflineService
{
    private readonly IndexedDBManager _db;
    private readonly CambioProductoService _cambios;

    public ProductoOfflineService(IndexedDBManager db, CambioProductoService cambios)
    {
        _db = db;
        _cambios = cambios;
    }

    // CREAR producto
    public async Task GuardarAsync(ProductoLocal producto)
    {
        await _db.UpdateRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });
        if (!producto.IsSynced && producto.Id_producto < 0 )
        {
            await _cambios.RegistrarCambio("Crear", producto);
        }
    }

    public async Task GuardoDeApiAsync(ProductoLocal producto)
    {
        await _db.UpdateRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });
    }

    // MODIFICAR producto
    public async Task ModificarAsync(ProductoLocal producto)
    {
        await _db.UpdateRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });

        await _cambios.RegistrarCambio("Modificar", producto);
    }

    // ELIMINAR producto
    public async Task EliminarAsync(int id)
    {
        var producto = await _db.GetRecordById<int, ProductoLocal>("productos", id);

        if (producto == null)
        {
            producto = new ProductoLocal
            {
                Id_producto = id
            };
        }

        // Registrar siempre el cambio
        await _cambios.RegistrarCambio("Eliminar", producto);

        // Intentar eliminar de IndexedDB
        await _db.DeleteRecord("productos", id);
    }

    public async Task EliminarLocalAsync(int id)
    {
        await _db.DeleteRecord("productos", id);
    }

    // Obtener todos
    public async Task<List<ProductoLocal>> ObtenerTodosAsync()
    {
        return await _db.GetRecords<ProductoLocal>("productos");
    }

    // Obtener pendientes
    public async Task<List<ProductoLocal>> ObtenerPendientesAsync()
    {
        var todos = await ObtenerTodosAsync();
        return todos.Where(p => !p.IsSynced).ToList();
    }

    // Marcar sincronizado
    public async Task MarcarComoSincronizado(int id_producto)
    {
        var producto = await _db.GetRecordById<int, ProductoLocal>("productos", id_producto);

        if (producto != null)
        {
            producto.IsSynced = true;

            await _db.UpdateRecord(new StoreRecord<ProductoLocal>
            {
                Storename = "productos",
                Data = producto
            });
        }
    }

    // Limpiar productos
    public async Task LimpiarAsync()
    {
        await _db.ClearStore("productos");
    }

    // ProductoOfflineService.cs
    public async Task ActualizarIdTemporal(int idTemporal, int idServidor)
    {
        // Obtener el producto temporal
        var producto = await _db.GetRecordById<int, ProductoLocal>("productos", idTemporal);
        if (producto == null)
            return;

        // Eliminar cualquier producto offline con el mismo nombre
        await _db.DeleteRecord("productos", idTemporal);

        // Actualizar Id y marcar como sincronizado
        producto.Id_producto = idServidor;
        producto.IsSynced = true;

        await _db.AddRecord(new StoreRecord<ProductoLocal>
        {
            Storename = "productos",
            Data = producto
        });

        var cambios = await _db.GetRecords<CambioProducto>("cambios_producto");

        var cambiosProducto = cambios.Where(c => c.ProductoId == idTemporal).ToList();

        foreach (var cambio in cambiosProducto)
        {
            cambio.ProductoId = idServidor;

            await _db.UpdateRecord(new StoreRecord<CambioProducto>
            {
                Storename = "cambios_producto",
                Data = cambio
            });
        }
    }
}