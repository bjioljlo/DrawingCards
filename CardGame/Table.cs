namespace CardGame
{
    public class Table
    {
        private readonly List<Card> _cards;
        public int MaxCards { get; }

        public Table(int maxCards)
        {
            if (maxCards <= 0)
                throw new ArgumentException("MaxCards must be greater than zero.");
            MaxCards = maxCards;
            _cards = [];
        }

        public bool AddCard(Card card)
        {
            if (_cards.Count >= MaxCards || card == null)
                return false;
            _cards.Add(card);
            return true;
        }

        public bool RemoveCard(Card card)
        {
            return _cards.Remove(card);
        }

        public bool SwapCards(int indexA, int indexB)
        {
            if (indexA < 0 || indexB < 0 || indexA >= _cards.Count || indexB >= _cards.Count || indexA == indexB)
                return false;
            (_cards[indexB], _cards[indexA]) = (_cards[indexA], _cards[indexB]);
            return true;
        }

        public IReadOnlyList<Card> GetCards()
        {
            return _cards.AsReadOnly();
        }
    }
}
