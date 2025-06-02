using MongoDB.Bson.Serialization.Attributes;

namespace Chat_RealTime.Controllers.user.Dtos
{
    public class ResgisterDto
    {
        public string UserName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }
}
