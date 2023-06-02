using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplicationServer.Services
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(User user);
        bool RegisterUser(User user);
        RefreshToken GenerateRefreshToken();
        void SetRefreshToken(RefreshToken refreshToken, HttpResponse response, User user);
    }
}
