using Microsoft.AspNetCore.Mvc;
using SimpleStore.Application.DTOs.Token;
using SimpleStore.Application.DTOs.User;
using SimpleStore.Application.Interfaces;

namespace SimpleStore.IdentityServer.Controllers
{
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            if (request.Password.Length < 6)
                return BadRequest("Password must be at least 6 characters long");

            if (request.Username.Length < 3)
                return BadRequest("Username must be at least 3 characters long");

            if (request.Email.Length < 5 || !request.Email.Contains("@"))
                return BadRequest("Invalid email address");

            var user = await authService.RegisterAsync(request);
            if (user == null)
                return BadRequest("User already exists");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            if (String.IsNullOrWhiteSpace(request.Login) || String.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Login and password are required");

            var result = await authService.LoginAsync(request);
            if (result == null)
                return BadRequest("Invalid credentials");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (request.Guid == Guid.Empty && String.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest();

            var result = await authService.RefreshTokenAsync(request);
            if (result == null
                || result.AccessToken is null
                || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }
    }
}
