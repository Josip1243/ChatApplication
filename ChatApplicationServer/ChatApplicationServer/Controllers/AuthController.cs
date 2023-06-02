using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;
using System.IdentityModel.Tokens.Jwt;

namespace ChatApplicationServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration configuration, IUserService userService, IAuthService authService)
        {
            _configuration = configuration;
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserCredentials request)
        {
            _authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new()
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            if (_authService.RegisterUser(user))
            {
                return Ok(user);
            }
            return BadRequest("Registration failed.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserCredentials request)
        {
            var user = _userService.GetUser(request);

            if (user.HasValue)
            {
                if (user.ValueOrDefault().Username != request.Username ||
                !_authService.VerifyPasswordHash(request.Password, user.ValueOrDefault().PasswordHash, user.ValueOrDefault().PasswordSalt))
                {
                    return BadRequest("Wrong username or password.");
                }
                string token = _authService.CreateToken(user.ValueOrDefault());
                var refreshToken = _authService.GenerateRefreshToken();
                _authService.SetRefreshToken(refreshToken, Response, user.ValueOrDefault());
                return Ok(token);
            }
            return BadRequest("User does not exist.");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(string jwtToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
            string username = token.Claims.First(c => c.Type == "user").Value;

            // Dodati if has value
            var user = _userService.GetUser(username).ValueOrDefault();

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string newToken = _authService.CreateToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();
            _authService.SetRefreshToken(newRefreshToken, Response, user);

            return Ok(token);
        }

        [HttpGet("GetMe"), Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }
    }
}
