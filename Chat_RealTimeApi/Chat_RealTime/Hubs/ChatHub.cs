using System.Security.Claims;
using Chat_RealTime.Connection;
using Chat_RealTime.Models;
using Chat_RealTime.Services.chat;
using Chat_RealTime.Services.Redis;
using Chat_RealTime.Services.user;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Chat_RealTime.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
          private readonly IMongoCollection<Chat> _chats;
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<User> _user;
        private readonly IChatService _chatService;
        private readonly IUserConnection _connection;
        private readonly IChatRedisService _chatRedis;

        public ChatHub(MongoDBContext context,
            IChatService chatSercice,IUserConnection connection,IChatRedisService chatRedis)
        {
            _chats = context.Chat;
            _messages = context.Message;
            _chatService = chatSercice;
            _user = context.User;
            _connection = connection;
            _chatRedis = chatRedis;

        }
        public override async Task OnConnectedAsync()
        {
            // Lấy thông tin user từ JWT token
            var userId = Context.UserIdentifier;
            var userName = Context.User?.FindFirst("userName")?.Value;
            _connection.AddUser(userId, Context.ConnectionId);
            Console.WriteLine($"User {userName} ({userId}) đã kết nối SignalR");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //var userId = Context.UserIdentifier;
            //var userName = Context.User?.FindFirst("userName")?.Value;

            //Console.WriteLine($"User {userName} ({userId}) đã ngắt kết nối SignalR");
            _connection.RemoveUser(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task JoinChat(string chatId,string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            var chat = await _chats.Find(c=> c.Id == chatId).FirstOrDefaultAsync();

            if (chat == null) {
                await Clients.Caller.SendAsync("Error", "chat khong ton tai ");
            };

                var messages = await _chatService.GetMessage(chatId);
            await Clients.Caller.SendAsync("LoadMessages",messages);

            if (chat.Type == ChatType.Group)
            {
                var user = await _chats.Find(u => u.Id == userId).FirstOrDefaultAsync();
                await Clients.GroupExcept(chatId, Context.ConnectionId)
                    .SendAsync("UserJoined", userId, user?.Name ?? "Unknown");
            }
            ;
            await Clients.Groups(chatId).SendAsync("UserJoin", userId);
            Console.WriteLine($"user{userId} đã join chat{chatId}");

            await _chatRedis.SubscribeToMessageAsync(chatId, async (messages) =>
            {
                await Clients.Group(chatId).SendAsync("ReceiveMessage", messages);
            });
        }

        public async Task SendMessage(string chatId, string senderId, string content)
        {
            try
            {
                var connections = _connection.GetConnectionUser(senderId);
                var seder = await _user.Find(u => u.Id == senderId).FirstOrDefaultAsync();
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    Content = content,
                    IsRead = false,
                    SenderName = seder?.UserName ?? "Unknow",
                    CreatedAt = DateTime.UtcNow

                };
                await _messages.InsertOneAsync(message);
                await _chats.UpdateOneAsync(
                   Builders<Chat>.Filter.Eq(c=>c.Id,chatId),
                   Builders<Chat>.Update.Set(c=>c.UpdatedAt,DateTime.UtcNow));

                await _chatRedis.PublishMessageAsync(chatId, message);

                await Clients.Group(chatId).SendAsync("ReceiveMessage",message);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }
     
        public async Task Typing(string chatId,string userId, bool isTyping)
        {
            await Clients.GroupExcept(chatId,Context.ConnectionId).SendAsync("UserTyping", userId, isTyping);
        }      

     
    }
}
