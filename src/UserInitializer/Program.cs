using System;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using BCrypt.Net;

class Program
{
    static async Task Main(string[] args)
    {
        // 🔑 Cambia esta conexión a tu base de datos MySQL
        string connectionString = "Server=localhost;Database=mna;Uid=root;Pwd=;";

        // Usuarios que quieres crear
        var users = new[]
        {
            new User { Username = "admin", PasswordHash = "1234", Role = "Admin" },
            new User { Username = "cliente", PasswordHash = "abcd", Role = "Cliente" }
        };

        using var connection = new MySqlConnection(connectionString);

        foreach (var user in users)
        {
            // Verificar si ya existe
            var exists = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT id FROM users WHERE username = @Username",
                new { user.Username });

            if (exists != null)
            {
                Console.WriteLine($"Usuario '{user.Username}' ya existe, saltando...");
                continue;
            }

            // Hashear la contraseña
            string hash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            // Insertar en la base de datos
            await connection.ExecuteAsync(
                "INSERT INTO users (username, password_hash, role) VALUES (@Username, @Hash, @Role)",
                new { Username = user.Username, Hash = hash, Role = user.Role });

            Console.WriteLine($"Usuario '{user.Username}' creado con éxito!");
        }

        Console.WriteLine("✅ Todos los usuarios inicializados.");
    }
}