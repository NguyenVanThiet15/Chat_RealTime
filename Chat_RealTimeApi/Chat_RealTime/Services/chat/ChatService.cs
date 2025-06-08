using System.Text.RegularExpressions;
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
        public ChatService(MongoDBContext contextMessage, MongoDBContext contextChat
            )
        {
            _messages = contextMessage.Message;
            _chats = contextChat.Chat;
        }


        public async Task SaveMesageAsync(Message input)
        {
            await _messages.InsertOneAsync(input);
        }
        public async Task<Chat> CreateChatRoom(CreateChatRoom input)
        {
            var listChatroom = await _chats.Find(x => x.Name.Trim().ToLower().Equals(input.NameRoom.Trim().ToLower())).FirstOrDefaultAsync();
            if (listChatroom != null)
            {
                return listChatroom;
            }
            else
            {
                var roomChat = new Chat
                {
                    Name = input.NameRoom,
                    Participants = input.NguoiThamGia,
                    Type = ChatType.Group,
                };
                await _chats.InsertOneAsync(roomChat);
                return roomChat;
            }
          
        }
        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
            return await _chats.Find(c => c.Participants.Contains(userId)&& c.Type == ChatType.Group  ).ToListAsync();
        }
        public async Task<Chat> CreateOrGetChat(CreateOrGetChatInput input)
        {
            if(! Enum.TryParse<ChatType>(input.ChatType, true, out var chatTypeInput))
            {
                chatTypeInput = ChatType.Private;
            }
            if (chatTypeInput == ChatType.Group)
            {
                var existringRoom = await _chats.Find(x => x.Type == ChatType.Group &&
                (x.Id == input.roomId || x.Name.Trim().ToLower().Equals(input.NameRoom.Trim().ToLower()))).FirstOrDefaultAsync();

                if (existringRoom != null)
                {
                    return existringRoom;
                }
                else
                {
                    var newChatRoom = new Chat
                    {
                        Name = input.NameRoom,
                        Type = ChatType.Group,
                        Id = input.roomId,
                        Participants = input.participant,
                        CreatedAt = DateTime.Now,
                        //        UpdatedAt = DateTime.Now,
                    };
                    await _chats.InsertOneAsync(newChatRoom);
                    return newChatRoom;
                }
            }
            else
            {
                var existringChat = await _chats.Find(c => c.Type == ChatType.Private &&
                c.Participants.Contains(input.NguoiGuiID) &&
                c.Participants.Contains(input.NguoiNhanId)).FirstOrDefaultAsync();
                if (existringChat != null)
                {
                    return existringChat;
                }
                else
                {
                    var newChat = new Chat
                    {
                        Name = "",
                        Type = ChatType.Private,
                        Participants = new List<string> { input.NguoiGuiID, input.NguoiNhanId },
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    await _chats.InsertOneAsync(newChat);

                    return newChat;
                }
            }
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

        
        public async Task<List<Message>> GetMessage (string ChatId )
        {
            var mesages = await _messages.Find(c => c.ChatId == ChatId)
                .Sort(Builders<Message>.Sort.Descending(m => m.CreatedAt))
                //.Skip(skip)
                //.Limit(limit)
                .ToListAsync();
            mesages.Reverse();
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
