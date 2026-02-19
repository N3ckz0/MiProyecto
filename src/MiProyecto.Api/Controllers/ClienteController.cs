using Microsoft.AspNetCore.Mvc;
using MiProyecto.Aplicacion.Clientes;
using MiProyecto.Domain;

namespace MiProyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var clientes = await _clienteService.ObtenerTodos();
        return Ok(clientes);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Cliente cliente)
    {
        await _clienteService.CrearCliente(cliente);
        return Ok(); // Solo confirma que se insertó
    }

}
