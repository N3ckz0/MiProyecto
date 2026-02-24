using MiProyecto.Domain;

namespace MiProyecto.Application.Productos;

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

    public async Task CrearProducto(Producto producto)
    {
        await _repo.CrearProducto(producto);
    }
}