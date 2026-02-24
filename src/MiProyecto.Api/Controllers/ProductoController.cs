using Microsoft.AspNetCore.Mvc;
using MiProyecto.Application.Productos;
using MiProyecto.Domain;

namespace MiProyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IProductoService _ProductoService;

    public ProductoController(IProductoService ProductoService)
    {
        _ProductoService = ProductoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var Productos = await _ProductoService.ObtenerTodos();
        return Ok(Productos);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Producto Producto)
    {
        await _ProductoService.CrearProducto(Producto);
        return Ok(); // Solo confirma que se insertó
    }

}
