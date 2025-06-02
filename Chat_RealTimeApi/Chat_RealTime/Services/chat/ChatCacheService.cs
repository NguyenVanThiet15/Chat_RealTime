
using System.Text.Json;
using Chat_RealTime.Controllers.chat.Dtos;
using Chat_RealTime.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Chat_RealTime.Services.chat
{
    public interface IChatCacheService
    {
        public Task<List<Message>> GetChatMessageCache(string chatId);
        public Task CacheMessage(string chatId, List<Message> input);
        public  Task RemoveChatCacheAsync(string chatId);



    }
    public class ChatCacheService : IChatCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<ChatCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOption;
        public ChatCacheService(IDistributedCache cache,ILogger<ChatCacheService> logger)
        {
            _cache = cache;
            _jsonOption = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _logger = logger;
        }
        public async Task<List<Message>> GetChatMessageCache(string chatId)
        {
            try
            {
                var chatKey = $"chat{chatId}:message";
                var cacheData = await _cache.GetStringAsync(chatKey);
                if (!string.IsNullOrEmpty(cacheData)) {
                    var message = JsonSerializer.Deserialize<List<Message>>(cacheData, _jsonOption);
                    return message.OrderByDescending(x=>x.CreatedAt).Take(50).ToList();
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Lỗi khi nhận tin nhắn được lưu trong bộ nhớ đệm để trò chuyện {ChatId}", chatId);
            }
            return new List<Message>();
        }
        public async Task CacheMessage (string chatId, List<Message> input)
        {
            try
            {
                var chatKey = $"chat{chatId}:message";

                var sortMessage = input.OrderByDescending(m=>m.CreatedAt).Take(100).ToList();
                var serializedData = JsonSerializer.Serialize(sortMessage, _jsonOption);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                };
                await _cache.SetStringAsync(chatKey, serializedData, options);  

            }
            catch (Exception ex) {
                _logger.LogError(ex, "Lỗi khi lưu trữ tin nhắn cho trò chuyện {ChatId}", chatId);
            }
        }
        public async Task RemoveChatCacheAsync(string chatId)
        {
            try
            {
                var cacheKey = $"chat:{chatId}:messages";
                await _cache.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bộ nhớ đệm cho trò chuyện {ChatId}", chatId);
            }
        }



    }
}
