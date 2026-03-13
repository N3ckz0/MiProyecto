using MySqlConnector;
using Dapper;
using MiProyecto.Domain.Entities;
using MiProyecto.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MiProyecto.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public ClienteRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("DefaultConnection missing");
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodos()
    {
        using var conn = new MySqlConnection(_connectionString);
        var clientes = await conn.QueryAsync<Cliente>("SELECT * FROM cliente");

        return clientes
            .Where(c => c.Id > 0)
            .OrderBy(c => c.Id);
    }

    public async Task CrearCliente(Cliente cliente)
    {
        using var conn = new MySqlConnection(_connectionString);

        var sql = @"INSERT INTO Clientes (Nombre, Rfc, Email, Password, Image) 
                    VALUES (@Nombre,@Rfc,@Email,@Password,@Image)";

        await conn.ExecuteAsync(sql, cliente);
    }
}