using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MO.Auth.Data;
using MO.Auth.Interfaces;
using static MO.Auth.Interfaces.IUsersApi;

namespace MO.Auth.Controllers;

[Route("[controller]")]
public class UsersController : ControllerBase, IUsersApi
{
    private readonly UserManager<AuthUser> userManager;
    private readonly IMapper mapper;

    public UsersController(UserManager<AuthUser> userManager, IMapper mapper)
    {
        this.userManager = userManager;
        this.mapper = mapper;
    }

    [HttpGet(GetByIdEndpoint)]
    [ProducesResponseType(typeof(GetByIdResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GetByIdResponse>> GetById([FromRoute]Guid id)
    {
        var guidStr = id.ToString();
        var authUser = await userManager.Users.FirstOrDefaultAsync(u => u.Id == guidStr);
        if (authUser == null) 
            return NotFound();
        return new GetByIdResponse(mapper.Map<UserDto>(authUser));
    }


    [HttpGet(GetByUsernameEndpoint)]
    [ProducesResponseType(typeof(GetByUsernameResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GetByUsernameResponse>> GetByUsername(string username)
    {
        var authUser = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (authUser is null)
            return NotFound();

        return new GetByUsernameResponse(new UserDto(Guid.Parse(authUser.Id), authUser.UserName, authUser.Email, authUser.EmailConfirmed));
    }

    [HttpGet(GetAllUsersEndpoint)]
    [ProducesResponseType(typeof(GetAllUsersResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GetAllUsersResponse>> GetAllUsers()
    {
        var users = await userManager.Users.Select(u => mapper.Map<UserDto>(u)).ToListAsync();
        if (!users.Any())
            return NotFound("There are no users in the database.");
        return new GetAllUsersResponse(users);
    }
}