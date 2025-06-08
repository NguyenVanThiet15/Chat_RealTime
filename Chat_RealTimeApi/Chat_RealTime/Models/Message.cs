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
        public string SenderName { get; set; } = "";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

    }
}
