namespace MySyntheticNetworkMessenger.Models
{
    public class ChatGptSettings
    {
        public int MaxTokens { get; set; }
        public int Temperature { get; set; }
        public int TopP { get; set; }
        public string ApiKey { get; set; }
        public string Model { get; set; }
        public string EndPoint { get; set; }
    }

    public class AppSettings
    {
        public ChatGptSettings ChatGptSettings { get; set; }
    }
}
