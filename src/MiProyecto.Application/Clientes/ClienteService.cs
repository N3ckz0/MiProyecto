using MySqlConnector;
using Dapper;
using MiProyecto.Domain;
using Microsoft.Extensions.Configuration;

namespace MiProyecto.Aplicacion.Clientes;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodos();
    Task CrearCliente(Cliente cliente);
}

public class ClienteService : IClienteService
{
    private readonly string _connectionString;

    public ClienteService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new ArgumentNullException("DefaultConnection missing");
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodos()
    {
        using var conn = new MySqlConnection(_connectionString);
        var clientes = await conn.QueryAsync<Cliente>("SELECT * FROM cliente");
        var resultado = clientes
        .Where(c => c.Id > 0)
        .OrderBy(c => c.Id);
        return resultado;
    }

    public async Task CrearCliente(Cliente cliente)
    {
        using var conn = new MySqlConnection(_connectionString);
        var sql = @"INSERT INTO Clientes (Nombre, Rfc, Email, Password, Image) 
                    VALUES (@Nombre,@Rfc,@Email,@Password,@Image)";
        await conn.ExecuteAsync(sql, cliente);
    }
}
