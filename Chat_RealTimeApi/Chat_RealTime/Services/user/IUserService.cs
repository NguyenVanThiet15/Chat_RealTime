using Chat_RealTime.Controllers.user.Dtos;
using Chat_RealTime.Models;

namespace Chat_RealTime.Services.user
{
    public interface IUserService
    {
        public Task<User> GetByEmailAsync(string email);
       public  Task<User?> GetUserbyId(string userId);
        public  Task<User> CreateUserAsync(ResgisterDto dto);
        public bool VerifyPassword(string hash, string password);
        public Task<List<User>> GetListUser(string userId);
        public Task<User> LoginGoogle(string TokenGoogle);
    }
}
