using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MiProyecto.Application.Interfaces;
using MiProyecto.Domain.Entities;

namespace MiProyecto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUsersService _UsersService;

	public UsersController( IUsersService UserService )
	{
		_UsersService = UserService;
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var Users = await _UsersService.GetAll();
		return Ok(Users);
	}

}