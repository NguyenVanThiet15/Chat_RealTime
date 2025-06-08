namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class CreateOrGetChatInput
    {
        public string NguoiGuiID { get; set; } = "";
        public string NguoiNhanId { get; set; } = "";
        public string NameRoom { get; set; } = "";
        public string roomId { get; set; } = "";
        public List<string> participant { get; set; } = new List<string>();
        public string ChatType { get; set; } = "";
    }
}
