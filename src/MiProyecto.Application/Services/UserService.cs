using MiProyecto.Domain.Entities;

namespace MiProyecto.Application.Interfaces;

public class UserService : IUsersService
{
	private readonly IUserRepository _repo;

	public UserService(IUserRepository repo)
	{
		_repo = repo;
	}

	public async Task<IEnumerable<User>> GetAll()
	{
		return await _repo.GetAll();
	}

	public async Task<User> GetByUsernameAsync(string username)
	{
		var user = await _repo.GetByUsernameAsync(username);
        if (user == null)
            throw new UnauthorizedAccessException("Usuario no encontrado");

        return null;
	}

}