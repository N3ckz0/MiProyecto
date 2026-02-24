using MiProyecto.Domain;

namespace MiProyecto.Application.Clientes;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObtenerTodos();
    Task CrearCliente(Cliente cliente);
}