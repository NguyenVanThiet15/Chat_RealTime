//using System.Collections.Concurrent;
//using System.Net.WebSockets;
//using System.Text;
//using Chat_RealTime.Controllers.chat.Dtos;
//using Chat_RealTime.Models;
//using Chat_RealTime.Services.chat;
//using Chat_RealTime.Services.Redis;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using MongoDB.Driver;
//using Newtonsoft.Json;

//namespace Chat_RealTime.Controllers.chat
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class ChatRedisController: ControllerBase
//    {
//        private readonly IChatRedisService _chatRedisService;
//        private readonly IMongoCollection<Message> _messages;
//        private readonly IMongoCollection<Chat> _chats;
//        private readonly IMongoCollection<User> _users;
//        private readonly IChatService _chatService;

//        private static readonly ConcurrentDictionary<string, List<WebSocket>> _chatConnection = new();
//         public ChatRedisController(IChatRedisService chatRedisService, MongoDBContext mongoDBContext,IChatService chatService)
//        {
//            _chatRedisService = chatRedisService;
//            _messages = mongoDBContext.Message;
//            _chats = mongoDBContext.Chat;
//            _users = mongoDBContext.User;
//            _chatService = chatService;
//        }
//        [HttpPost("join/{chatId}")]
//        public async Task<IActionResult> JoinChat (string chatId,userIdInput input)
//       {
//            try
//            {
//                var chat = await _chats.Find(c => c.Id == chatId).FirstOrDefaultAsync();
//                if (chat == null)
//                {
//                    return NotFound(" Chat không tồn tại ");
//                }
//                var user = await _users.Find(u => u.Id == input.UserId).FirstOrDefaultAsync();

//                await _chatRedisService.SubscribeToChatAsync(chatId, async (cId, message) =>
//                {
//                    await BroadcastToChat(cId, "ReceiveMessage", message);
//                });
//                await _chatRedisService.SubcribeToEnventAsync(chatId,
//                    async (cId, userId, userName) =>
//                    {
//                        await BroadcastToChat(cId, "UserJoin", new { UserId = userId, UserName = userName });
//                    },
//                  async (cId, userId, userName) =>
//                  {
//                      await BroadcastToChat(cId, "UserLeft", new { UserId = userId, UserName = userName });
//                  }
//                );


//                var messages = await _chatService.GetMessage(chatId);
//                await _chatRedisService.PublisUserJoinAsync(chatId, input.UserId, user?.UserName ?? "Unknow");

//                return Ok(new { Message = messages });
//            }
//            catch (Exception ex)
//            {
//                return Unauthorized(new { message = ex.Message });
//            }
           
//        }
//        [HttpPost("sendMessageRedis")]
//        public async Task<IActionResult> SendMessageRedis (SendMessageInput input)
//        {
//            try
//            {
//                var sender = await _users.Find(u => u.Id == input.SenderId).FirstOrDefaultAsync();

//                var message = new Message
//                {
//                    ChatId = input.ChatId,
//                    SenderId = input.SenderId,
//                    Content = input.Content,
//                    IsRead = false,
//                    SenderName = sender?.UserName ?? "Unknow",
//                    CreatedAt = DateTime.Now,
//                };
//                await _messages.InsertOneAsync(message);
//                await _chats.UpdateOneAsync(
//                   Builders<Chat>.Filter.Eq(c => c.Id, input.ChatId),
//                   Builders<Chat>.Update.Set(c => c.UpdatedAt, DateTime.UtcNow));

//                await _chatRedisService.PublisMessageAsync(input.ChatId, message);
//                return Ok(message);
//            }
//            catch (Exception ex)

//            {
//                return Unauthorized(new { message = ex.Message });
//            }

//        }


//        private async Task BroadcastToChat(string chatId, string eventName, object data)
//        {
//            if(_chatConnection.TryGetValue(chatId,out var sockets)){
//                var message = JsonConvert.SerializeObject(new { Envent = eventName, Data = data });
//                var buffer = Encoding.UTF8.GetBytes(message);

//                foreach(var socket in sockets.ToList())
//                {
//                    if(socket.State == WebSocketState.Open)
//                    {
//                        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
//                    }
//                }
//            }

//        }

//    }
   
//}
