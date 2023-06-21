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
                new User { Id = 1, Username = "Marko"}
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
            user.Id = users.Count() + 1;
            users.Add(user);
            return true;
        }
    }
}
