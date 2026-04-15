using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IUsersRepository
{
	Task<IEnumerable<User>> GetAll();
}