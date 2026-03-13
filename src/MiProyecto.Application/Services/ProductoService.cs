using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;

    public ProductoService(IProductoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodos()
    {
        return await _repo.ObtenerTodos();
    }

    public async Task<Producto?> ObtenerPorId(int id)
    {
        return await _repo.ObtenerPorId(id);
    }

    public async Task ActualizarProducto(Producto producto)
    {
        await _repo.ActualizarProducto(producto);
    }

    public async Task<Producto> CrearProducto(Producto producto)
    {
        producto = await _repo.CrearProducto(producto);
        return producto;
    }
    public async Task EliminarProducto(int id)
    {
        await _repo.EliminarProducto(id);
    }
}