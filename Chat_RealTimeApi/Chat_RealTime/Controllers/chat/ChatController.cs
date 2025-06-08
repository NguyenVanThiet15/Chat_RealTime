using System.Security.Claims;
using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Hubs;
using Chat_RealTime.Models;
using Chat_RealTime.Services.chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat_RealTime.Controllers.chat
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IWebHostEnvironment environment, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _enviroment = environment;
            _hubContext = hubContext;
        }
        [HttpPost("createChat")]
        public async Task<IActionResult> CreateChatRoom(CreateChatRoom input)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ " });
                }
                input.UserId = userId;
                if (!input.NguoiThamGia.Contains(input.UserId))
                {
                    input.NguoiThamGia.Add(input.UserId);
                }

                var chat = await _chatService.CreateChatRoom(input);
                return Ok(chat);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }

        }
        [HttpGet("getListChatRoom")]
        public async Task<IActionResult> GetChatRoom()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var listChatRoom = await _chatService.GetUserChatsAsync(userId);
                return Ok(listChatRoom);

            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("createOrGetChat")]
        public async Task<IActionResult> CreateOrGetChat(CreateOrGetChatInput input)
        {
            try
            {
                var getChat = await _chatService.CreateOrGetChat(input);
                return Ok(getChat);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{chatId}/getMessage")]
        public async Task<IActionResult> GetMessage(string ChatId)
        {
            try
            {
                var getMessage = await _chatService.GetMessage(ChatId);
                return Ok(getMessage);
            } catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
       
        
    }
}
