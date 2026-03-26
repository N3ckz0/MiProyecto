using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try{
            var token = await _authService.LoginAsync(dto.Username, dto.Password);
            return Ok(new { token });
        }
            catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message); // devuelve 401 y mensaje
        }
        catch (Exception ex)
        {
            // opcional: log del error real
            return StatusCode(500, "Error interno del servidor");
        }
    }
}