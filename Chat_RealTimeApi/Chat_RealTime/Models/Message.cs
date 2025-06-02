using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Chat_RealTime.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string ChatId { get; set; } = "";

        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; } = "";//người gửi

        public string Content { get; set; } = "";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }
        public MessageType TypeMessage { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? MineType { get; set; }
    }
    public enum MessageType
    {
        Text = 0,
        Image = 1,
        File = 2
    }
}
