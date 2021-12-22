using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MO.Auth.Data;

namespace MO.Auth.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AuthUser> _userManager;
        private readonly SignInManager<AuthUser> _signInManager;
        private readonly UsersDbContext _context;

        public AuthController(UserManager<AuthUser> userManager, SignInManager<AuthUser> signInManager, UsersDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user == null) return BadRequest("User does not exist");

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

            if (!singInResult.Succeeded) return BadRequest("Invalid password");

            await _signInManager.SignInAsync(user, true);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Register(Login login)
        {
            if (_context.AppUsers.Any(u => u.Username == login.Username))
                return BadRequest("User Already Exists");
            var user = new AuthUser
            {
                UserName = login.Username,
                AppUser = new User() { Username = login.Username }
            };

            var result = await _userManager.CreateAsync(user, login.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            return await Login(login);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}

public record Login(string Username, string Password);