using Chat_RealTime.Models;

namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class CreateChatRoom
    {
        public string Name { get; set; } = "";
        public ChatType ChatType { get; set; }
        public List<string> Participants { get; set; } = new List<string>();
    }
}
