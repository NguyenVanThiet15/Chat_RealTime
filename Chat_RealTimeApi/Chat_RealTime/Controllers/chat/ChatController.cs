using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Hubs;
using Chat_RealTime.Models;
using Chat_RealTime.Services.chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

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

        private readonly IMongoCollection<Chat> _chats;
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<User> _user;


        public ChatController(IChatService chatService, IWebHostEnvironment environment, IHubContext<ChatHub> hubContext,
            MongoDBContext context)
        {
            _chatService = chatService;
            _enviroment = environment;
            _hubContext = hubContext;
            _chats = context.Chat;
            _messages = context.Message;
            _user = context.User;
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
        public async Task<IActionResult> GetMessage(string ChatId, int skip)
        {
            try
            {
                var getMessage = await _chatService.GetMessage(ChatId,skip);
                return Ok(getMessage);
            } catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("{chatId}/getMessageImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendMessageImg(string chatId, [FromForm] SendMessageImg input)
        {
            try
            {
                if (input.File == null || input.File.Length == 0)
                {
                    return BadRequest("file không hợp lệ!");
                  
                }
                var fileUrl = await _chatService.UploadFIleAsync(input.File);

                var seder = await _user.Find(u => u.Id == input.SenderId).FirstOrDefaultAsync();

                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = input.SenderId,
                    Content = fileUrl,
                    Type = MessageType.Image,
                    IsRead = false,
                    SenderName = seder?.UserName ?? "Unknow",
                    CreatedAt = DateTime.UtcNow

                };
                await _messages.InsertOneAsync(message);
                await _chats.UpdateOneAsync(
                   Builders<Chat>.Filter.Eq(c => c.Id, chatId),
                   Builders<Chat>.Update.Set(c => c.UpdatedAt, DateTime.UtcNow));

                // Gửi message real-time qua SignalR cho tất cả user trong group
                await _hubContext.Clients.Group(chatId).SendAsync("ReceiveMessage", message);

                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

           
            
        }
        [HttpPost("InsertDataMessage")]
        public async Task<IActionResult> InsertdataMessage(InsertMessageInput input)
        {
          await _chatService.InsertdataMessage(input);
            return Ok();
        }

    }
}
