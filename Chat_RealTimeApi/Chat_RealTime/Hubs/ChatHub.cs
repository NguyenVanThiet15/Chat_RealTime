using System.Security.Claims;
using Chat_RealTime.Models;
using Chat_RealTime.Services.chat;
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
        private readonly IChatCacheService _cache;
        private readonly IChatService _chatService;

   

        public ChatHub(MongoDBContext context,IChatCacheService cache,IChatService chatSercice)
        {
            _chats = context.Chat;
            _messages = context.Message;
            _cache = cache;
            _chatService = chatSercice;

        }
        public override async Task OnConnectedAsync()
        {
            // Lấy thông tin user từ JWT token
            var userId = Context.UserIdentifier;
            var userName = Context.User?.FindFirst("userName")?.Value;

            Console.WriteLine($"User {userName} ({userId}) đã kết nối SignalR");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            var userName = Context.User?.FindFirst("userName")?.Value;

            Console.WriteLine($"User {userName} ({userId}) đã ngắt kết nối SignalR");

            await base.OnDisconnectedAsync(exception);
        }
        public async Task JoinChat(string chatId,string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            var caCaCheMessageJoinChat = await _cache.GetChatMessageCache(chatId);
            if (caCaCheMessageJoinChat.Any())
            {
                await Clients.Caller.SendAsync("LoadMessages", caCaCheMessageJoinChat);
            }
            else {
                var messages = await _chatService.GetMessage(chatId);
                if (messages.Any()) {
                    await _cache.CacheMessage(chatId, messages);
                }
                await Clients.Caller.SendAsync("LoadMessages",messages);
            }
            await Clients.Groups(chatId).SendAsync("UserJoin", userId);

        }

        public async Task SendMessage(string chatId, string senderId, string content
            //,MessageType messageType,string? filename = null, string? fileUrl= null,
            //long? fileSize= null,string? mimeType = null
            )
        {
            try
            {
                var message = new Message
                {
                    ChatId = chatId,
                    SenderId = senderId,
                    Content = content,
                    IsRead = false,
                    //TypeMessage = messageType,
                    //FileName = filename,
                    //FileUrl = fileUrl,
                    //FileSize = fileSize,
                    //MineType = mimeType,
                    CreatedAt = DateTime.UtcNow

                };
                await _messages.InsertOneAsync(message);

               var caCheMessage = await _cache.GetChatMessageCache(chatId);
                caCheMessage.Insert(0,message);
                await _cache.CacheMessage(chatId,caCheMessage);

                await _chats.UpdateOneAsync(
                   Builders<Chat>.Filter.Eq(c=>c.Id,chatId),
                   Builders<Chat>.Update.Set(c=>c.UpdatedAt,DateTime.UtcNow));


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
