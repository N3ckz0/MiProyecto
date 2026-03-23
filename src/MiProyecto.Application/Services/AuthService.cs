using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class AuthService : IAuthService
{
    public async Task<string> LoginAsync(string username, string password)
    {
        // Validar usuario (mock por ahora)
        if (username == "admin" && password == "1234")
        {
            return GenerateJwtToken(username);
        }

        throw new UnauthorizedAccessException();
    }

    private string GenerateJwtToken(string username)
    {
        // Claims: información que quieres poner en el token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin") // Puedes cambiar roles más adelante
        };

        // Clave secreta (debe ser la misma que en Program.cs)
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("MI_CLAVE_SUPER_SECRETA_1234567890123456"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Crear el token
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        // Devolver el token como string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}