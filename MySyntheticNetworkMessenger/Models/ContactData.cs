namespace MySyntheticNetworkMessenger.Models
{
    public class ContactData
    {
        public int ChatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool ManKvinna { get; set; }
        public Era Era { get; set; }
        public int Age { get; set; }
        public int Politeness { get; set; }
        public int Formality { get; set; }
        public int Humor { get; set; }
        public int Confidence { get; set; }
        public int Goofiness { get; set; }
        public int ShortAnswers { get; set; }
        public string InstructionTemplate { get; set; } = string.Empty;
    }
}
