
using Chat_RealTime.Models;

namespace Chat_RealTime.Services.Redis
{
    public interface IChatRedisService
    {
        Task PublishMessageAsync(string Channel, Message input);
        Task SubscribeToMessageAsync(string Channel, Action<Message> onMessageReceived);
    }
}
