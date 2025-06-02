namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class MessageWithSender
    {
        public string Id { get; set; } = "";
        public string ChatId { get; set; } = "";
        public string SenderId { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string Content { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
