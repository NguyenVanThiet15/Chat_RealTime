namespace Chat_RealTime.Controllers.chat.Dtos
{
    public class CreateOrGetChatInput
    {
        public string CurrentUserId { get; set; } = "";
        public string TargetUserId { get; set; } = "";
    }
}
