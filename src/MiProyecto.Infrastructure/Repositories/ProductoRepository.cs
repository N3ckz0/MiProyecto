using MySqlConnector;
using Dapper;
using MiProyecto.Domain.Entities;
using MiProyecto.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MiProyecto.Infrastructure.Repositories;

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

        var productos = await conn.QueryAsync<Producto>("SELECT * FROM productos WHERE Id_producto > 0 ORDER BY Id_producto");

        return productos;
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
}