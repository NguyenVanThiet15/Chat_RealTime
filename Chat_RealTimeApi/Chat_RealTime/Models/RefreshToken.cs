using MongoDB.Bson.Serialization.Attributes;

namespace Chat_RealTime.Models
{
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Token { get; set; } = "";
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
