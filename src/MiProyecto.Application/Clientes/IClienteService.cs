using MiProyecto.Domain;

namespace MiProyecto.Application.Clientes;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodos();
    Task CrearCliente(Cliente cliente);
}