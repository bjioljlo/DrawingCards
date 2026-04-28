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
        /// 加入多張牌到手牌
        /// </summary>
        /// <param name="cards">要加入的卡牌集合</param>
        /// <exception cref="ArgumentNullException">當 cards 為 null 時擲出</exception>
        public void AddRange(IEnumerable<Card> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            foreach (var card in cards)
            {
                ArgumentNullException.ThrowIfNull(card);
                _cards.Add(card);
            }
        }

        /// <summary>
        /// 移除多張指定牌
        /// </summary>
        /// <param name="cards">要移除的卡牌集合</param>
        /// <exception cref="ArgumentNullException">當 cards 為 null 時擲出</exception>
        /// <exception cref="InvalidOperationException">當任何卡牌不在手牌中時擲出</exception>
        public void DiscardRange(IEnumerable<Card> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            var cardsToDiscard = cards.ToList();
            
            // 先確認所有卡牌都存在才開始移除
            foreach (var card in cardsToDiscard)
            {
                if (!_cards.Contains(card))
                    throw new InvalidOperationException($"Card not found in hand: {card}");
            }

            foreach (var card in cardsToDiscard)
            {
                _cards.Remove(card);
            }
        }

        /// <summary>
        /// 取得所有手牌內容
        /// </summary>
        /// <returns>所有手牌的唯讀複本</returns>
        public IReadOnlyList<Card> GetAll()
        {
            return _cards.ToList().AsReadOnly();
        }

        /// <summary>
        /// Returns the number of cards in hand.
        /// </summary>
        public int Count => _cards.Count;
    }
}
