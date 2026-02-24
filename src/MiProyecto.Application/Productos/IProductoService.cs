using MiProyecto.Domain;

namespace MiProyecto.Application.Productos;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodos();
    Task CrearProducto(Producto producto);
}