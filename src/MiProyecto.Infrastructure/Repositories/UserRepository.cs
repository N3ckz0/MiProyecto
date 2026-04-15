using MySqlConnector;
using Dapper;
using MiProyecto.Domain.Entities;
using MiProyecto.Application.Interfaces;
using Microsoft.Extensions.Configuration;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = @"SELECT * FROM users";
        var user = await connection.QueryAsync<User>(sql);
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = @"SELECT id, username, password_hash AS PasswordHash, role
                       FROM users
                       WHERE username = @Username
                       LIMIT 1";

        var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        return user;
    }
}