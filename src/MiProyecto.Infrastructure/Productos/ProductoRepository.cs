using MySqlConnector;
using Dapper;
using MiProyecto.Domain;
using MiProyecto.Application.Productos;
using Microsoft.Extensions.Configuration;

namespace MiProyecto.Infrastructure.Productos;

public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("DefaultConnection missing");
    }

    public async Task<IEnumerable<Producto>> ObtenerTodos()
    {
        using var conn = new MySqlConnection(_connectionString);

        var productos = await conn.QueryAsync<Producto>(
            "SELECT * FROM productos");

        return productos
            .Where(p => p.Id_producto > 0)
            .OrderBy(p => p.Id_producto);
    }

    public async Task CrearProducto(Producto producto)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO productos (nombre, precio) 
                    VALUES (@Nombre, @Precio)";

        await conn.ExecuteAsync(sql, producto);
    }
}