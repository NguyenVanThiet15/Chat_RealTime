namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class SendMessageImg
    {
        public string SenderId { get; set; } = "";
        public IFormFile File { get; set; }
    }
}
