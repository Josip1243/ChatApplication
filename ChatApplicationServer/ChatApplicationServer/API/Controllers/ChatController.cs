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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatService _chatService;
        private readonly IChatRepository _chatRepository;

        public ChatController(IHttpContextAccessor httpContextAccessor, IChatService chatService, 
            IChatRepository chatRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatService = chatService;
            _chatRepository = chatRepository;
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
            var currentUsername = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var chat = _chatService.GetChat(chatId, currentUsername);

            if (chat != null)
                return Ok(chat);
            return NoContent();
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
            _chatRepository.RemoveChat(chatId, userId);

            return Ok();
        }
    }
}
