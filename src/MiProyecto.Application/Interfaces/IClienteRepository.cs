using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerTodos();
    Task CrearCliente(Cliente cliente);
}