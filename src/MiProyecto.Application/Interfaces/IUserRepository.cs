using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}