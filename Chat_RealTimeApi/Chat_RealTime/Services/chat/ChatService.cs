using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;

namespace Chat_RealTime.Services.chat
{
    public class ChatService:IChatService
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<Chat> _chats;
        private readonly IChatCacheService _cache;

        public ChatService(MongoDBContext contextMessage, MongoDBContext contextChat,IChatCacheService cache)
        {
            _messages = contextMessage.Message;
            _chats = contextChat.Chat;
            _cache = cache;
        }


        public async Task SaveMesageAsync(Message input)
        {
            await _messages.InsertOneAsync(input);
        }
        //public async Task<LinkedList<Message>> GetMessagesAsync() {
        //    return await _messages.Find();
        //}
        public async Task<Chat> CreateChatRoom(CreateChatRoom input)
        {
            var roomChat = new Chat
            {
                Name = input.Name,
                Participants = input.Participants,
                Type = input.ChatType,
            };
            await _chats.InsertOneAsync(roomChat);
            return roomChat;
        }
        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
            return await _chats.Find(c => c.Participants.Contains(userId)).ToListAsync();
        }
        public async Task<Chat> CreateOrGetChat(CreateOrGetChatInput input)
        {
            var existringChat = await _chats.Find(c => c.Type == ChatType.Private &&
            c.Participants.Contains(input.CurrentUserId) &&
            c.Participants.Contains(input.TargetUserId)).FirstOrDefaultAsync();

            if (existringChat != null)
            {
                return existringChat;
            }

            var newChat = new Chat
            {
                Name = "",
                Type = ChatType.Private,
                Participants = new List<string> { input.CurrentUserId, input.TargetUserId },
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _chats.InsertOneAsync(newChat);

            return newChat;
            
        }
        //public async Task<Message> SendMessage (SendMessageInput input)
        //{
        //    var message = new Message
        //    {
        //        ChatId = input.ChatId,
        //        SenderId = input.SenderId,
        //        Content = input.Content,
        //        IsRead = false,
        //        CreatedAt = DateTime.UtcNow,
        //    };
        //    await _messages.InsertOneAsync(message);
        //    await _chats.UpdateOneAsync(
        //            Builders<Chat>.Filter.Eq(c => c.Id, input.ChatId),
        //            Builders<Chat>.Update.Set(c => c.UpdatedAt, DateTime.UtcNow)
        //        );

        //    return message;

        //}
        public async Task<List<Message>> GetMessage (string ChatId )
        {
            var CacheMessage = await _cache.GetChatMessageCache(ChatId);
            if (CacheMessage.Any())
            {
                return CacheMessage;
            }
            //var skip = (page - 1) * limit;
            var mesages = await _messages.Find(c => c.ChatId == ChatId)
                .Sort(Builders<Message>.Sort.Descending(m => m.CreatedAt))
                //.Skip(skip)
                //.Limit(limit)
                .ToListAsync();

            mesages.Reverse();
            if (mesages.Any()) 
            {
                await _cache.CacheMessage(ChatId, mesages);
             };

            return mesages;
        }
        //public async Task<Message> SendMessageImage(SendMessageInput input)
        //{
        //    if (input.ImageFile == null || input.ImageFile.Length == 0)
        //    {
        //        return BadRequest("không có ảnh đc gửi!");
        //    }
        //    var messageImg = new Message
        //    {
        //        ChatId = input.ChatId,
        //        SenderId = input.SenderId,
        //        Content = "Đã gửi 1 ảnh",
        //        TypeMessage = MessageType.Image,

        //    }
        //}
    }
}
