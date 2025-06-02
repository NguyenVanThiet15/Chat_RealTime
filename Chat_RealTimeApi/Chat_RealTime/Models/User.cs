using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat_RealTime.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = "";
        [BsonElement("userName")]
        public string UserName { get; set; } = "";
        [BsonElement("email")]
        public string Email { get; set; } = "";
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = "";
        public string AvatarUrl { get; set; } = "";

        public bool IsOnline { get; set; }
        public string GoogleId { get; set; } = "";

        public DateTime LastActive { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Contacts { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
