using Chat_RealTime.Models;

namespace Chat_RealTime.Services.user
{
    public interface IJwtServicer
    {
        public string GenerateToken(User input);
    }
}
