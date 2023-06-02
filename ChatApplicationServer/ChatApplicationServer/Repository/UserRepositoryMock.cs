using ChatApplicationServer.Models;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace ChatApplicationServer.Repository
{
    public class UserRepositoryMock
    {
        List<User> users;

        public UserRepositoryMock()
        {
            users = new List<User>()
            {
            };
        }

        public Option<User> GetUser(string username)
        {
            return users.FirstOrDefault(u => u.Username == username).SomeNotNull();
        }

        public void UpdateUser(User user)
        {
            users.Remove(users.FirstOrDefault(u => u.Username == user.Username));
            users.Add(user);
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public bool RegisterUser(User user)
        {
            users.Add(user);
            return true;
        }
    }
}
