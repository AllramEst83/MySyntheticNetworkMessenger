namespace MySyntheticNetworkMessenger.Models
{
    public class ChatMessage
    {
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage()
        {
            Timestamp = DateTime.UtcNow;
        }
    }

}
