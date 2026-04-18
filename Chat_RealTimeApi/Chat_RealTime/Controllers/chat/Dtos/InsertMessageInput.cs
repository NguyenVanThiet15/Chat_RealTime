namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class InsertMessageInput
    {
        public string ChatId { get; set; } = "";
        public string SenderId { get; set; } = "";
        public string SenderName { get; set; } = "";
    }
}
