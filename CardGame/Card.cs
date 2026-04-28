namespace CardGame
{
    public class Card
    {
        public string Suit { get; }
        public string Rank { get; }
        public string Name { get; }
        public string ID { get; }
        public string Description { get; }
        public int Score { get; } // 新增分數屬性

        public Card(string suit, string rank, string name = "", string id = "", string description = "", int score = 0)
        {
            Suit = suit;
            Rank = rank;
            Name = name;
            ID = id;
            Description = description;
            Score = score; // 設定分數
        }

        public override bool Equals(object? obj)
        {
            if (obj is Card other)
                return Suit == other.Suit && Rank == other.Rank && ID == other.ID;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Rank, ID);
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? $"{Suit} {Rank}" : Name;
        }
    }
}
