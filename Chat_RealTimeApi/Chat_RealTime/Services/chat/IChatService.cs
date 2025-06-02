using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Models;

namespace Chat_RealTime.Services.chat
{
    public interface IChatService
    {
        public  Task<Chat> CreateChatRoom(CreateChatRoom input);
        public Task<List<Chat>> GetUserChatsAsync(string userId);
        public Task<Chat> CreateOrGetChat(CreateOrGetChatInput input);
        //public Task<Message> SendMessage(SendMessageInput input);
        public Task<List<Message>> GetMessage(string ChatId);
    }
}
