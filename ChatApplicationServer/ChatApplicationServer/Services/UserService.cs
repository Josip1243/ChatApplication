using AutoMapper;
using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Optional;
using System.Security.Claims;

namespace ChatApplicationServer.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private Mapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public Option<User> GetUser(UserCredentials user)
        {
            return _userRepository.GetUser(user.Username);
        }
        public Option<User> GetUser(string username)
        {
            return _userRepository.GetUser(username);
        }

        public Option<UserDTO> GetUserDTO(int userId)
        {
            return Option.Some(_mapper.Map<UserDTO>(_userRepository.GetUsers().SingleOrDefault(user => user.Id == userId)));
        }

        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }

        
    }
}
