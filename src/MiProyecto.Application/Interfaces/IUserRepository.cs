using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User> GetByUsernameAsync(string username);
}
