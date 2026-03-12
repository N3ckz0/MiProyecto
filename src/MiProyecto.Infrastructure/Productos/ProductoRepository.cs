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

    public async Task<Producto?> ObtenerPorId(int id)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = "SELECT * FROM productos WHERE Id_producto = @Id";

        return await conn.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
    }

    public async Task ActualizarProducto(Producto producto)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = @"UPDATE productos 
                    SET nombre = @Nombre,
                        precio = @Precio
                    WHERE Id_producto = @Id_producto";

        await conn.ExecuteAsync(sql, producto);
    }

    public async Task<Producto> CrearProducto(Producto producto)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO productos (nombre, precio) 
                    VALUES (@Nombre, @Precio)";

        var id = await conn.ExecuteScalarAsync<int>(sql, producto);
        producto.Id_producto = id;
        return producto;
    }

    public async Task EliminarProducto(int id)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = "DELETE FROM productos WHERE id_producto = @Id";

        await conn.ExecuteAsync(sql, new { Id = id });
    }
    public async Task CrearProductosLoteAsync(List<Producto> productos)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO productos (nombre, precio) VALUES (@Nombre, @Precio);";

        // Itera cada producto y ejecuta el insert
        foreach (var producto in productos)
        {
            await conn.ExecuteAsync(sql, producto);
        }
    }
}