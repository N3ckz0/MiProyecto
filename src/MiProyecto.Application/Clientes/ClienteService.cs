using MiProyecto.Domain;

namespace MiProyecto.Application.Clientes;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodos()
    {
        return await _repository.ObtenerTodos();
    }

    public async Task CrearCliente(Cliente cliente)
    {
        await _repository.CrearCliente(cliente);
    }
}