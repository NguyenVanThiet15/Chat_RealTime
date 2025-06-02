using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Hubs;
using Chat_RealTime.Models;
using Chat_RealTime.Services.chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat_RealTime.Controllers.chat
{
    [ApiController]
    [Route("api/[controller]")]
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
                //var userId = User.FindFirst("Id")?.Value;
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
        public async Task<IActionResult> GetChatRoom(string userId)
        {
            try
            {
                //var userId = User.FindFirst("Id")?.Value;
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

        //[HttpPost("senMessage")]
        //public async Task<IActionResult> SendMessage (SendMessageInput input)
        //{
        //    try
        //    {
        //        var sendMessage = await _chatService.SendMessage(input);
        //        return Ok(sendMessage);
        //    }catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
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
        //[HttpPost("SendMessageImg")]
        //public async Task<IActionResult> SendMessageImage(SendMessageInput input)
        //{
        //    try
        //    {
        //        if (input.ImageFile == null || input.ImageFile.Length == 0)
        //        {
        //            return BadRequest("không có ảnh đc gửi!");
        //        }
        //        var uploadPath = Path.Combine(_enviroment.WebRootPath, "uploads");
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }
        //        var fileName = Guid.NewGuid() + Path.GetExtension(input.ImageFile.FileName);
        //        var pathName = Path.Combine(uploadPath, fileName);
        //        using (var stream = new FileStream(pathName, FileMode.Create))
        //        {
        //            await input.ImageFile.CopyToAsync(stream);
        //        }
        //        var imageUrl = $"/uploads/{fileName}";
        //        var messageImg = new Message
        //        {
        //            ChatId = input.ChatId,
        //            SenderId = input.SenderId,
        //            Content = "Đã gửi 1 ảnh",
        //            TypeMessage = MessageType.Image,
        //            FileUrl = imageUrl,
        //            FileSize = input.ImageFile.Length,
        //            FileName = input.ImageFile.FileName

        //        };

        //        await _hubContext.Clients.Group(input.ChatId).SendAsync("ReceiveMessage", messageImg);
        //        return Ok(messageImg);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }

    //}
        
    }
}
