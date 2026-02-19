using Microsoft.AspNetCore.Mvc;
using MiProyecto.Aplicacion.Saludo;

namespace MiProyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaludoController : ControllerBase
{
    private readonly ISaludoService _saludoService;

    public SaludoController(ISaludoService saludoService)
    {
        _saludoService = saludoService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var saludo = _saludoService.ObtenerSaludo();
        return Ok(saludo);
    }
}
