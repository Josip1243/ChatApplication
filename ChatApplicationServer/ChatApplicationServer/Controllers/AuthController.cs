using Azure;
using Azure.Core;
using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using ChatApplicationServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ChatApplicationServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly UserRepositoryMock _userRepositoryMock;
        private readonly ConnectionService _connectionService;

        public AuthController(IConfiguration configuration, IUserService userService, IAuthService authService, UserRepositoryMock userRepositoryMock)
        {
            _configuration = configuration;
            _userService = userService;
            _authService = authService;
            _userRepositoryMock = userRepositoryMock;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserCredentials request)
        {
            if (_userRepositoryMock.GetUser(request.Username).HasValue)
            {
                return BadRequest("User already exists.");
            }

            if (_authService.RegisterUser(request))
            {
                return Ok();
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
                var refreshToken = _authService.GenerateRefreshToken(user.ValueOrFailure());
                return Ok(new TokenDTO()
                {
                    AccessToken = token,
                    RefreshToken = refreshToken.Token
                });
            }
            return BadRequest("Wrong username or password.");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken(TokenDTO tokenDto)
        {
            if (tokenDto is null)
                return BadRequest("Invalid request.");

            var accessToken = tokenDto.AccessToken;
            var refreshToken = tokenDto.RefreshToken;
            var principle = _authService.GetPrincipleFromToken(accessToken);
            var username = principle.Identity.Name;
            var user = _userRepositoryMock.GetUser(username).ValueOrFailure();

            if (user is null || user.RefreshToken != refreshToken || user.TokenExpires <= DateTime.Now)
                return BadRequest("Invalid Request");

            var newAccessToken = _authService.CreateToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken(user);

            return Ok(new TokenDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
            });
        }

        [HttpGet("GetMe"), Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }
    }
}
