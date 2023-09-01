using ChatApplicationServer.Models;
using Optional;

namespace ChatApplicationServer.Repository
{
    public interface IUserRepository
    {
        Option<User> GetUser(string username);
        List<User> GetUsers();
        bool RegisterUser(User user);
        void UpdateUser(User user);
    }
}