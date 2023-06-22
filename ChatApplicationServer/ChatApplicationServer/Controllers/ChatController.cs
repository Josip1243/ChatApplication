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
        private readonly UserRepositoryMock _userRepositoryMock;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChatService _chatService;
        private readonly ChatRepositoryMock _chatRepositoryMock;

        public ChatController(UserRepositoryMock userRepositoryMock, 
                              IHttpContextAccessor httpContextAccessor, ChatService chatService, ChatRepositoryMock chatRepositoryMock)
        {
            _userRepositoryMock = userRepositoryMock;
            _httpContextAccessor = httpContextAccessor;
            _chatService = chatService;
            _chatRepositoryMock = chatRepositoryMock;
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

        [HttpGet("addChat"), Authorize]
        public async Task<ActionResult<ChatDTO>> AddChat(string username)
        {
            var currentUser = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                currentUser = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

                var chat = _chatService.AddChat(currentUser, username);
                return Ok(chat);
            }
            return BadRequest();
        }

        [HttpDelete("removeChat/{chatId}"), Authorize]
        public async Task<ActionResult> RemoveChat(int chatId)
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("Id"));
            _chatRepositoryMock.RemoveChat(chatId, userId);

            return Ok();
        }
    }
}
