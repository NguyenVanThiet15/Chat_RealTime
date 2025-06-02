using Chat_RealTime.Controllers.user.Dtos;
using Chat_RealTime.Models;
using Google.Apis.Auth;
using MongoDB.Driver;

namespace Chat_RealTime.Services.user
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService (MongoDBContext context)
        {
           _users = context.User;
        }
        public async Task<User> GetByEmailAsync(string email) {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserbyId(string userId)
        {
            return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(ResgisterDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };

            await _users.InsertOneAsync(user);
            return user;
        }
        public bool VerifyPassword(string hash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        public async Task<List<User>> GetListUser(string userId) {
           return await _users.Find(u => u.Id != userId).ToListAsync();
        }
        public async Task<User> LoginGoogle (string TokenGoogle)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(TokenGoogle);
            var user = await _users.Find(u => u.GoogleId == payload.Subject).FirstOrDefaultAsync();
            if (user == null) {
                user = new User
                {
                    GoogleId = payload.Subject,
                    Email = payload.Email,
                    UserName = payload.Name,
                };
                await _users.InsertOneAsync(user);
            };
            return user;    
        }
    }
}
