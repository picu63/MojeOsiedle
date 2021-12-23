using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MO.Auth.Data;

namespace MO.Auth.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AuthUser> userManager;
    private readonly SignInManager<AuthUser> signInManager;
    private readonly UsersDbContext context;

    public AuthController(UserManager<AuthUser> userManager, SignInManager<AuthUser> signInManager, UsersDbContext context)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.context = context;
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
    public async Task<IActionResult> Register(Login login)
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

public record Login(string Username, string Password);