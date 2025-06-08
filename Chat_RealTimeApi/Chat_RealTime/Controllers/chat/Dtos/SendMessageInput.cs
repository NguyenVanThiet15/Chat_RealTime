namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class SendMessageInput
    {
        public string SenderId { get; set; } = "";
        public string Content { get; set; } = "";
        public string ChatId { get; set; } = "";
        //public IFormFile ImageFile { get; set; }
    }
}
