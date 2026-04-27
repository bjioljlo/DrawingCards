namespace CardGame
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Hand Hand { get; set; }
        public int Score { get; set; }

        public List<Card> DiscardPile { get; set; }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
            Hand = new Hand();
            Score = 0;
            DiscardPile = [];
        }

        public void DrawCard(Deck deck)
        {
            ArgumentNullException.ThrowIfNull(deck);
            var card = deck.DrawCard();
            Hand.Add(card);
        }

        public bool PlayCard(Card card, Table table)
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(table);
            if (!Hand.Cards.Contains(card))
                return false;
            if (table.AddCard(card))
            {
                Hand.Discard(card);
                return true;
            }
            return false;
        }

        public void ShowHand()
        {
            foreach (var card in Hand.Cards)
            {
                Console.WriteLine(card.Name);
            }
        }

        public int CalculateScore()
        {
            return Hand.Cards.Sum(card => card.Score);
        }

        public void DiscardCard(Card card)
        {
            ArgumentNullException.ThrowIfNull(card);
            Hand.Discard(card);
            DiscardPile.Add(card);
        }

        // 全部丟棄手牌
        public void DiscardAll()
        {
            var cardsToDiscard = Hand.Cards.ToList();
            foreach (var card in cardsToDiscard)
            {
                Hand.Discard(card);
                DiscardPile.Add(card);
            }
        }

        // 丟棄指定多張牌
        public void DiscardCards(IEnumerable<Card>? cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            foreach (var card in cards.ToList())
            {
                if (!Hand.Cards.Contains(card))
                    throw new InvalidOperationException("Card not in hand.");
                Hand.Discard(card);
                DiscardPile.Add(card);
            }
        }

        public void AddCardToDiscardPile(Card card)
        {
            ArgumentNullException.ThrowIfNull(card);
            DiscardPile.Add(card);
        }
    }
}
