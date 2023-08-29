using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Models2;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApplicationServer.Services
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(User user);
        bool RegisterUser(UserCredentials user);
        RefreshToken GenerateRefreshToken(User user);
        //void SetRefreshToken(RefreshToken refreshToken, HttpResponse response, User user);
        ClaimsPrincipal GetPrincipleFromToken(string token);
    }
}
