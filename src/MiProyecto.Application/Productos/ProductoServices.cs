using MySqlConnector;
using Dapper;
using MiProyecto.Domain;
using Microsoft.Extensions.Configuration;

namespace MiProyecto.Aplicacion.Productos;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodos();
    Task CrearProducto(Producto Producto);
}

public class ProductoService : IProductoService
{
    private readonly string _connectionString;

    public ProductoService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new ArgumentNullException("DefaultConnection missing");
    }

    public async Task<IEnumerable<Producto>> ObtenerTodos()
    {
        using var conn = new MySqlConnection(_connectionString);
        var Productos = await conn.QueryAsync<Producto>("SELECT * FROM productos");
        var resultado = Productos
        .Where(p => p.Id_producto > 0)
        .OrderBy(p => p.Id_producto);
        return resultado;
    }

    public async Task CrearProducto(Producto Producto)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = @"INSERT INTO productos (nombre, precio) 
                    VALUES (@Nombre,@Precio)";
        await conn.ExecuteAsync(sql, Producto);
    }
}
