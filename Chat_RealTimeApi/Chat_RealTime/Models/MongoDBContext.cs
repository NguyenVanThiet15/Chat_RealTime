using MongoDB.Driver;

namespace Chat_RealTime.Models
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IConfiguration config) 
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        }
        public IMongoCollection<User> User => _database.GetCollection<User>("users");
        public IMongoCollection<Message> Message =>  _database.GetCollection<Message>("messages");
        public IMongoCollection<Chat> Chat => _database.GetCollection<Chat>("chats");
    }
}
