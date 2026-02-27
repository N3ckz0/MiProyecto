using MiProyecto.Domain;

namespace MiProyecto.Application.Productos;

public interface IProductoRepository
{
    Task<IEnumerable<Producto>> ObtenerTodos();
    Task<Producto?> ObtenerPorId(int id);
    Task ActualizarProducto(Producto producto);
    Task CrearProducto(Producto producto);
    Task EliminarProducto(int id);
}