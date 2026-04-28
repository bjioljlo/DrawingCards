namespace CardGame
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Hand Hand { get; set; }
        public int Score { get; set; }

        public DiscardPile DiscardPile { get; }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
            Hand = new Hand();
            Score = 0;
            DiscardPile = new DiscardPile();
        }

        public void DrawCard(Deck deck)
        {
            ArgumentNullException.ThrowIfNull(deck);
            var card = deck.DrawCard();
            Hand.Add(card);
        }

        /// <summary>
        /// 從牌組抽多張牌加入手牌
        /// </summary>
        /// <param name="deck">牌組</param>
        /// <param name="count">要抽取的張數</param>
        /// <exception cref="ArgumentNullException">當 deck 為 null 時擲出</exception>
        /// <exception cref="ArgumentOutOfRangeException">當 count 小於等於 0 時擲出</exception>
        public void DrawCards(Deck deck, int count)
        {
            ArgumentNullException.ThrowIfNull(deck);
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than zero.");
            
            var cards = deck.DrawCards(count);
            Hand.AddRange(cards);
        }

        /// <summary>
        /// 從手牌打出多張牌並放到桌面
        /// </summary>
        /// <param name="cards">要打出的卡牌集合</param>
        /// <param name="table">桌面物件</param>
        /// <returns>全部成功時回傳 true，任何失敗回傳 false</returns>
        /// <exception cref="ArgumentNullException">當 cards 或 table 為 null 時擲出</exception>
        public bool PlayCards(IEnumerable<Card> cards, Table table)
        {
            ArgumentNullException.ThrowIfNull(cards);
            ArgumentNullException.ThrowIfNull(table);
            var cardsToPlay = cards.ToList();
            
            // 先驗證所有牌都在手上且桌面可容納
            foreach (var card in cardsToPlay)
            {
                if (!Hand.Cards.Contains(card))
                    return false;
            }
            
            if (table.GetCards().Count + cardsToPlay.Count > table.MaxCards)
                return false;

            // 全部驗證通過才開始出牌
            foreach (var card in cardsToPlay)
            {
                table.AddCard(card);
                Hand.Discard(card);
            }
            
            return true;
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
