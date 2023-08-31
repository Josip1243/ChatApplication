using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using Optional;

namespace ChatApplicationServer.Services
{
    public interface IUserService
    {
        Option<User> GetUser(UserCredentials userCredentials);
        Option<User> GetUser(string username);
        Option<UserDTO> GetUserDTO(int userId);
        string GetMyName();
    }
}
