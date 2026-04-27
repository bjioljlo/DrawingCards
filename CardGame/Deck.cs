namespace CardGame
{

    public class Deck
    {
        private readonly List<Card> _cards;
        private readonly List<Card> _discardPile;
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
        public IReadOnlyList<Card> DiscardPile => _discardPile.AsReadOnly();

        public int Count => _cards.Count;
        public int TotalCards { get; }

        // 從 JSON 檔案建立 Deck
        public static Deck FromJson(string jsonFilePath)
        {
            var cards = CardLoader.LoadCardsFromJson(jsonFilePath);
            return new Deck(cards);
        }

        public Deck()
        {
            var cards = CardLoader.LoadCardsFromJson("CardGame/Template.json");
            _cards = [.. cards];
            _discardPile = [];
            TotalCards = _cards.Count;
        }

        // 測試用：允許自訂牌堆內容
        public Deck(List<Card> cards)
        {
            _cards = [.. cards];
            _discardPile = [];
            TotalCards = _cards.Count;
        }

        // 洗牌功能
        public void Shuffle()
        {
            var rng = new Random();
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0)
            {
                // If deck is empty, try to refill from discard pile
                if (_discardPile.Count > 0)
                {
                    // Shuffle discard pile and move to deck
                    var rng = new Random();
                    int n = _discardPile.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = rng.Next(n + 1);
                        (_discardPile[n], _discardPile[k]) = (_discardPile[k], _discardPile[n]);
                    }
                    _cards.AddRange(_discardPile);
                    _discardPile.Clear();
                }
            }

            if (_cards.Count == 0)
                throw new InvalidOperationException("Deck and discard pile are empty.");

            // Draw from the top (last card)
            var card = _cards[^1];
            _cards.RemoveAt(_cards.Count - 1);
            return card;
        }

        public void DiscardCard(Card? card)
        {
            ArgumentNullException.ThrowIfNull(card);
            _cards.Remove(card);
            _discardPile.Add(card);
        }

        /// <summary>
        /// 一次丟棄多張卡片
        /// </summary>
        /// <param name="cards">要丟棄的卡片清單</param>
        /// <exception cref="ArgumentNullException">卡片清單為 null 時擲出</exception>
        public void DiscardCards(IEnumerable<Card> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);
            
            foreach (var card in cards)
            {
                ArgumentNullException.ThrowIfNull(card);
                _cards.Remove(card);
                _discardPile.Add(card);
            }
        }

        /// <summary>
        /// 一次抽多張卡片
        /// </summary>
        /// <param name="count">要抽的卡片數量</param>
        /// <returns>抽出的卡片清單</returns>
        /// <exception cref="ArgumentOutOfRangeException">數量小於 1 時擲出</exception>
        /// <exception cref="InvalidOperationException">牌堆與棄牌區加起來不足以抽取時擲出</exception>
        public List<Card> DrawCards(int count)
        {
            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count), "抽取數量必須大於 0");

            var drawnCards = new List<Card>();
            for (int i = 0; i < count; i++)
            {
                drawnCards.Add(DrawCard());
            }
            return drawnCards;
        }

        public void ClearDeck()
        {
            _cards.Clear();
            _discardPile.Clear();
        }
    }
}
