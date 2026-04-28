namespace CardGame
{
    /// <summary>
    /// 棄牌區管理類別，負責管理遊戲過程中被棄掉的牌
    /// </summary>
    public class DiscardPile : IEnumerable<Card>
    {
        private readonly List<Card> _cards;

        /// <summary>
        /// 目前棄牌區的牌 (唯讀清單)
        /// </summary>
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

        /// <summary>
        /// 棄牌區張數
        /// </summary>
        public int Count => _cards.Count;

        public DiscardPile()
        {
            _cards = [];
        }

        /// <summary>
        /// 將單張牌加入棄牌區
        /// </summary>
        /// <param name="card">要加入的卡牌</param>
        /// <exception cref="ArgumentNullException">當 card 為 null 時擲出</exception>
        public void Add(Card card)
        {
            ArgumentNullException.ThrowIfNull(card);
            _cards.Add(card);
        }

        /// <summary>
        /// 將多張牌加入棄牌區
        /// </summary>
        /// <param name="cards">要加入的卡牌集合</param>
        /// <exception cref="ArgumentNullException">當 cards 為 null 時擲出</exception>
        public void AddRange(IEnumerable<Card> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            _cards.AddRange(cards);
        }

        /// <summary>
        /// 清空棄牌區所有牌
        /// </summary>
        public void Clear()
        {
            _cards.Clear();
        }

        /// <summary>
        /// 取得棄牌區最上面一張牌 (最後加入的牌)
        /// </summary>
        /// <returns>最上面的卡牌，若棄牌區為空則回傳 null</returns>
        public Card? GetLastCard()
        {
            return _cards.Count == 0 ? null : _cards[^1];
        }

        /// <summary>
        /// 取得所有棄牌內容
        /// </summary>
        /// <returns>所有卡牌的唯讀複本</returns>
        public IReadOnlyList<Card> GetAll()
        {
            return _cards.ToList().AsReadOnly();
        }

        /// <summary>
        /// 實作 IEnumerable<Card> 介面，支援 LINQ 查詢
        /// </summary>
        public IEnumerator<Card> GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 檢查是否包含指定卡牌
        /// </summary>
        public bool Contains(Card card)
        {
            return _cards.Contains(card);
        }
    }
}
