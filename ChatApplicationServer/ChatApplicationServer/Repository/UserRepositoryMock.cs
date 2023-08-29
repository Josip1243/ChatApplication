using ChatApplicationServer.Models;
using ChatApplicationServer.Models2;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace ChatApplicationServer.Repository
{
    public class UserRepositoryMock
    {
        private ChatAppContext _appContext;

        public UserRepositoryMock(ChatAppContext appContext)
        {
            _appContext = appContext;
        }

        public Option<User> GetUser(string username)
        {
            return _appContext.Users.FirstOrDefault(u => u.Username == username).SomeNotNull();
        }

        public void UpdateUser(User user)
        {
            var userToUpdate = _appContext.Users.FirstOrDefault(u => u.Id == user.Id);
            userToUpdate = user;
            _appContext.SaveChanges();
        }

        public List<User> GetUsers()
        {
            return _appContext.Users.ToList();
        }

        public bool RegisterUser(User user)
        {
            _appContext.Users.Add(user);
            _appContext.SaveChanges();
            return true;
        }
    }
}
