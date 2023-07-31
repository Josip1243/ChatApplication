using Azure.Core;
using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplicationServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepositoryMock _userRepositoryMock;

        public AuthService(IConfiguration configuration, UserRepositoryMock userRepositoryMock)
        {
            _configuration = configuration;
            _userRepositoryMock = userRepositoryMock;
        }

        public bool RegisterUser(UserCredentials request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new()
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            return _userRepositoryMock.RegisterUser(user);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Auth:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddSeconds(100),
                Created = DateTime.Now
            };

            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;

            _userRepositoryMock.UpdateUser(user);

            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipleFromToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("my top secret key");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");
            return principal;

        }
    }
}
