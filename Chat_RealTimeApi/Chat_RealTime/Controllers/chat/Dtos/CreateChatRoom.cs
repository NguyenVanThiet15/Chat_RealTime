using Chat_RealTime.Models;

namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class CreateChatRoom
    {
        public string NameRoom { get; set; } = "";
        //public ChatType ChatType { get; set; }
        public List<string> NguoiThamGia { get; set; } = new List<string>();
        public string UserId { get; set; } = "";
    }
}
