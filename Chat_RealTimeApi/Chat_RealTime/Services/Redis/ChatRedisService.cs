
using System.Text.Json;
using Chat_RealTime.Models;

using StackExchange.Redis;

namespace Chat_RealTime.Services.Redis
{
    public class ChatRedisService : IChatRedisService
    {
        private readonly IDatabase _database;
        private readonly ISubscriber _subscriber;
        private readonly IConnectionMultiplexer _redis;

        public ChatRedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _subscriber = redis.GetSubscriber();
        }
        public async Task PublishMessageAsync(string Channel, Message input)
        {
            var channel =  $"chat:{Channel}:messages" ;

            var json = JsonSerializer.Serialize(input);

            await _subscriber.PublishAsync(channel, json);
        }

        public async Task SubscribeToMessageAsync(string Channel, Action<Message> onMessageReceived)
        {
            var channel = $"chat:{Channel}:messages";
            await _subscriber.SubscribeAsync(channel, (ch, message) =>
            {
                try
                {
                    var messageObj = JsonSerializer.Deserialize<Message>(message);
                    onMessageReceived?.Invoke(messageObj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing message: {ex.Message}");
                }
            });
        }
    }
}
