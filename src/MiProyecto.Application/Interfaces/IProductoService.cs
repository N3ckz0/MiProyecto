using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodos();
    Task<Producto?> ObtenerPorId(int id);
    Task ActualizarProducto(Producto producto);
    Task<Producto> CrearProducto(Producto producto);
    Task EliminarProducto(int id);
}