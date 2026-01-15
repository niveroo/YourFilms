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
        private readonly RegisterUser _registerUserService;

        public UserController(LoginUser loginUserService, RegisterUser registerUserService)
        {
            _loginUserService = loginUserService;
            _registerUserService = registerUserService;
        }
        // POST api/<UserController>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login( [FromBody] LoginRequestDTO request)
        {
            var token = await _loginUserService.LoginAsync(request);
            if (token != null)
            {
                return Ok(token);
            }
            return Unauthorized("Wrong username or password.");
        }

        // POST api/<UserController>/register
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterRequestDTO request)
        {
            var result = await _registerUserService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
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
