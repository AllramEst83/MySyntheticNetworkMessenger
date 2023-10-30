namespace MySyntheticNetworkMessenger.Models
{
    public class ChatMessage
    {
        public string Message { get; set; } = string.Empty;
        public int userId { get; set; }

        public MessageType MessageType { get; set; }
    }
}
