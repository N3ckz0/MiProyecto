using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodos();
    Task CrearCliente(Cliente cliente);
}