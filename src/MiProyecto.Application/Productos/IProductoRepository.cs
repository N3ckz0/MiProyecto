using MiProyecto.Domain;

namespace MiProyecto.Application.Productos;

public interface IProductoRepository
{
    Task<IEnumerable<Producto>> ObtenerTodos();
    Task CrearProducto(Producto producto);
}