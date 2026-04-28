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

        /// <summary>
        /// 目前桌面上的牌數
        /// </summary>
        public int CurrentCards => _cards.Count;

        /// <summary>
        /// 桌面是否已滿
        /// </summary>
        public bool IsFull => _cards.Count >= MaxCards;

        /// <summary>
        /// 清空桌面所有牌
        /// </summary>
        public void Clear()
        {
            _cards.Clear();
        }

        /// <summary>
        /// 驗證牌組是否可放置到桌面
        /// </summary>
        /// <param name="cards">要放置的卡牌集合</param>
        /// <returns>可放置時回傳 true，否則回傳 false</returns>
        public bool IsValidMove(IEnumerable<Card> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            var cardList = cards.ToList();
            
            if (cardList.Count == 0)
                return false;
            
            if (_cards.Count + cardList.Count > MaxCards)
                return false;
            
            if (cardList.Any(c => c == null))
                return false;

            return true;
        }
    }
}
