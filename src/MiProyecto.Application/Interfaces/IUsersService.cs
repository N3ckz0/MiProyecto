using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IUsersService
{
	Task<IEnumerable<User>> GetAll();
	Task<User> GetByUsernameAsync(string username);
}