using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourFilms.DTOs;
using YourFilms.Services.Authorization;
using YourFilms.Services.Interactions;

namespace YourFilms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LoginUser _loginUserService;
        private readonly RegisterUser _registerUserService;
        private readonly UserService _userService;

        public UserController(LoginUser loginUserService, RegisterUser registerUserService, UserService userService)
        {
            _loginUserService = loginUserService;
            _registerUserService = registerUserService;
            _userService = userService;
        }
        // POST api/<UserController>
        [HttpPost("Login")]
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
        [HttpPost("Register")]
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
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _userService.GetUserDetails(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
    }
}
