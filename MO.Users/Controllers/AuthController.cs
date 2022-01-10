using System.ComponentModel.DataAnnotations;
using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MO.Auth.Data;
using MO.Auth.Messages;
using MO.Integration.MessagingBus;

namespace MO.Auth.Controllers;

//[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> logger;
    private readonly UserManager<AuthUser> userManager;
    private readonly SignInManager<AuthUser> signInManager;
    private readonly UsersDbContext context;
    private readonly IMessageBus messageBus;
    private readonly IMapper mapper;

    public AuthController(ILogger<AuthController> logger, UserManager<AuthUser> userManager, SignInManager<AuthUser> signInManager, UsersDbContext context, IMessageBus messageBus, IMapper mapper)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.context = context;
        this.messageBus = messageBus;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login login)
    {
        var user = await userManager.FindByNameAsync(login.Username);
        if (user == null) return BadRequest("User does not exist");

        var singInResult = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        if (!singInResult.Succeeded) return BadRequest("Invalid password");

        await signInManager.SignInAsync(user, true);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody]Login login)
    {
        if (context.AppUsers.Any(u => u.Username == login.Username))
            return BadRequest("User Already Exists");
        var user = new AuthUser
        {
            UserName = login.Username,
            AppUser = new User() { Username = login.Username }
        };

        var result = await userManager.CreateAsync(user, login.Password);
        if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
        var registeredUserGuid = await userManager.FindByNameAsync(login.Username);
        var newUserMessage = mapper.Map<NewUserMessage>(user);
        newUserMessage.MessageId = Guid.Parse(registeredUserGuid.Id);
        try
        {
            await messageBus.PublishMessage(newUserMessage, "newuserrequest"); // TODO move to configuration
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error with publishing message about new user.");
            throw;
        }

        return await Login(login);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok();
    }
}

public class Login
{
    public Login(string Username, string Password)
    {
        this.Username = Username;
        this.Password = Password;
    }

    public string Username { get; init; }
    [DataType(DataType.Password)]
    public string Password { get; init; }

    public void Deconstruct(out string Username, out string Password)
    {
        Username = this.Username;
        Password = this.Password;
    }
}