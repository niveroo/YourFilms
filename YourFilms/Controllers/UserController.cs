using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Models;
using YourFilms.Services;
using YourFilms.Services.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // DELETE api/<UserController>/5
        [HttpGet]
        [Authorize]
        public string GetHash(string password)
        {
            return Hash.getHashSha256(password);
        }

        [Authorize]
        [HttpGet("me")]
        public ActionResult<UserDto> GetMe()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (id is null)
                return Unauthorized();

            return Ok(new UserDto
            {
                Id = int.Parse(id),
                Username = username!,
                Email = email!,
                CreatedAt = DateTime.MinValue // если нужно — бери из БД
            });
        }

        [HttpGet("claims")]
        public IActionResult Claims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
