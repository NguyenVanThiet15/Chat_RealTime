using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat_RealTime.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        public ChatType Type { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Participants { get; set; } = new List<string>();

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Messages { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum ChatType
    {
        Private ,
        Group 
    }
}
