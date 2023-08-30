using AutoMapper;
using ChatApplicationServer.DTO;
using ChatApplicationServer.Models2;
using ChatApplicationServer.Repository;
using Optional;
using System.Security.Claims;

namespace ChatApplicationServer.Services
{
    public class UserService : IUserService
    {
        // Uncoment when using DB
        //private ChatContext context;

        // For testing only (before DB setup)
        private UserRepositoryMock _userRepository;
        private Mapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(/*ChatContext context,*/ UserRepositoryMock userRepository, IHttpContextAccessor httpContextAccessor)
        {
            // Uncoment when using DB
            //this.context = context;
            _userRepository = userRepository;
            _mapper = MapperConfig.InitializeAutoMapper();
            _httpContextAccessor = httpContextAccessor;
        }


        public Option<User> GetUser(UserCredentials user)
        {
            // Retrieves user from DB
            // Uncomment when using DB
            //return Option.Some(context.Users.SingleOrDefault(user => user.Username == userCredentials.Username && user.Password == userCredentials.Password));
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
