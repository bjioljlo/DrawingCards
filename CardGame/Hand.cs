namespace CardGame
{
    /// <summary>
    /// Represents a player's hand of cards.
    /// </summary>
    public class Hand
    {
        private readonly List<Card> _cards;

        /// <summary>
        /// Gets the cards currently in hand.
        /// </summary>
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

        public Hand()
        {
            _cards = [];
        }

        /// <summary>
        /// Adds a card to the hand.
        /// </summary>
        public void Add(Card? card)
        {
            ArgumentNullException.ThrowIfNull(card);
            _cards.Add(card);
        }

        /// <summary>
        /// Removes a specific card from the hand and returns it.
        /// Throws if the card is not in hand.
        /// </summary>
        public Card Discard(Card card)
        {
            ArgumentNullException.ThrowIfNull(card);
            if (!_cards.Remove(card))
                throw new InvalidOperationException("Card not found in hand.");
            return card;
        }

        /// <summary>
        /// Returns a list of the names of all cards in hand.
        /// </summary>
        public List<string> GetCardNames()
        {
            return _cards.Select(c => c.Name).ToList();
        }

        /// <summary>
        /// Returns the number of cards in hand.
        /// </summary>
        public int Count => _cards.Count;
    }
}