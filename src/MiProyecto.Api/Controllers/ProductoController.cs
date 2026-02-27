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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var producto = await _ProductoService.ObtenerPorId(id);

        if (producto == null)
            return NotFound();

        return Ok(producto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Producto producto)
    {
        if (id != producto.Id_producto)
            return BadRequest();

        await _ProductoService.ActualizarProducto(producto);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Post(Producto Producto)
    {
        await _ProductoService.CrearProducto(Producto);
        return Ok(); // Solo confirma que se insertó
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ProductoService.EliminarProducto(id);
        return NoContent();
    }

}
