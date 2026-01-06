using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Models;
using YourFilms.Services;
using YourFilms.Services.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace YourFilms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LoginUser _loginUserService;

        public UserController(LoginUser loginUserService)
        {
            _loginUserService = loginUserService;
        }
        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<string>> Login( [FromBody] LoginRequestDTO request)
        {
            var token = await _loginUserService.handle(request);
            if (token != null)
            {
                return Ok(token);
            }
            return Unauthorized("Wrong username or password.");
        }

        // GET api/<UserController>/me
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new
            {
                Id = userId,
                Username = username,
                Email = email
            });
        }
    }
}
