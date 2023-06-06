using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using ChatApplicationServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApplicationServer.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly UserRepositoryMock _userRepositoryMock;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChatService _chatService;

        public ChatController(IConfiguration configuration, IUserService userService, IAuthService authService, UserRepositoryMock userRepositoryMock, IHttpContextAccessor httpContextAccessor, ChatService chatService)
        {
            _configuration = configuration;
            _userService = userService;
            _authService = authService;
            _userRepositoryMock = userRepositoryMock;
            _httpContextAccessor = httpContextAccessor;
            _chatService = chatService;
        }

        [HttpGet("getAllChats"), Authorize]
        public async Task<ActionResult<IEnumerable<ChatNameDTO>>> GetAllChats()
        {
            var currentUser = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                currentUser = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            return Ok(_chatService.GetAllChats(currentUser));
            }
            return BadRequest();
        }

        [HttpGet("getChat"), Authorize]
        public async Task<ActionResult<IEnumerable<ChatDTO>>> GetChat(int chatId)
        {
            return Ok(_chatService.GetChat(chatId));
        }

        [HttpPost("addChat"), Authorize]
        public async Task<ActionResult<ChatDTO>> AddChat(string username)
        {
            var currentUser = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                currentUser = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                var chat = _chatService.AddChat(currentUser, username);
                return chat;
            }
            return BadRequest();
        }
    }
}
