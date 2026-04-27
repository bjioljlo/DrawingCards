namespace CardGame.Tests
{
    public class DeckTests
    {
        [Fact]
        public void DrawCard_RemovesCardFromDeck()
        {
            // Arrange
            var card1 = new Card("S", "A", "SA", "1");
            var card2 = new Card("H", "2", "H2", "2");
            var deck = new Deck(new List<Card> { card1, card2 });
            int initialCount = deck.Count;

            // Act
            Card drawnCard = deck.DrawCard();

            // Assert
            Assert.NotNull(drawnCard);
            Assert.Equal(initialCount - 1, deck.Count);
            Assert.DoesNotContain(drawnCard, deck.Cards);
        }

        [Fact]
        public void DrawCard_ThrowsException_WhenDeckIsEmpty()
        {
            // Arrange
            var deck = new Deck(new List<Card>());
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
        }

        [Fact]
        public void DrawCard_ReturnsUniqueCards_UntilDeckIsEmpty()
        {
            // Arrange
            var cards = new List<Card> {
                new Card("S", "A", "SA", "1"),
                new Card("H", "2", "H2", "2"),
                new Card("C", "3", "C3", "3")
            };
            var deck = new Deck(cards);
            var drawnCards = new HashSet<Card>();

            // Act
            while (deck.Count > 0)
            {
                var card = deck.DrawCard();
                Assert.DoesNotContain(card, drawnCards);
                drawnCards.Add(card);
            }

            // Assert
            Assert.Equal(cards.Count, drawnCards.Count);
        }

        [Fact]
        public void DiscardCard_MovesCardToDiscardPile()
        {
            // Arrange
            var card1 = new Card("S", "A", "SA", "1");
            var deck = new Deck(new List<Card> { card1 });
            var card = deck.DrawCard();

            // Act
            deck.DiscardCard(card);

            // Assert
            Assert.Contains(card, deck.DiscardPile);
            Assert.DoesNotContain(card, deck.Cards);
        }

        [Fact]
        public void DiscardCard_ThrowsException_WhenCardIsNull()
        {
            // Arrange
            var deck = new Deck(new List<Card>());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => deck.DiscardCard(null));
        }

        [Fact]
        public void DrawCard_FromEmptyDeck_RefillsFromDiscardPile()
        {
            // Arrange
            var cards = new List<Card> {
                new Card("S", "A", "SA", "1"),
                new Card("H", "2", "H2", "2")
            };
            var deck = new Deck(cards);

            // Draw all cards and discard them
            var drawnCards = new List<Card>();
            while (deck.Count > 0)
            {
                var card = deck.DrawCard();
                drawnCards.Add(card);
                deck.DiscardCard(card);
            }

            // Act - 現在牌堆和棄牌堆都應該是空的，但 DrawCard 應該能從棄牌堆重新填充
            var newCard = deck.DrawCard();

            // Assert
            Assert.NotNull(newCard);
            Assert.DoesNotContain(newCard, deck.Cards);
            Assert.DoesNotContain(newCard, deck.DiscardPile);
        }

        [Fact]
        public void DrawCard_ThrowsException_WhenDeckAndDiscardPileAreEmpty()
        {
            // Arrange
            var deck = new Deck(new List<Card>());
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => deck.DrawCard());
        }

        [Fact]
        public void Shuffle_ChangesCardOrder_ButKeepsContent()
        {
            // Arrange: 建立足夠多的卡片以避免隨機剛好回到原順序
            var cards = new List<Card>();
            for (int i = 1; i <= 10; i++)
            {
                cards.Add(new Card("Spades", i.ToString(), $"{i} of Spades", i.ToString()));
            }
            
            var deck = new Deck(cards);
            var originalOrder = deck.Cards.ToList();

            // Act
            deck.Shuffle();
            var shuffledOrder = deck.Cards.ToList();

            // Assert 1: 內容應完全一致, 沒有新增或遺失卡片
            Assert.Equal(originalOrder.Count, shuffledOrder.Count);
            Assert.All(originalOrder, c => Assert.Contains(c, shuffledOrder));

            // Assert 2: 至少要有超過一半的位置順序改變
            int changedPositions = 0;
            for (int i = 0; i < originalOrder.Count; i++)
            {
                if (originalOrder[i].ID != shuffledOrder[i].ID)
                    changedPositions++;
            }
            
            // 10張牌時統計上至少會有超過3張改變位置, 否則洗牌演算法有問題
            Assert.True(changedPositions >= 3, $"洗牌後僅有 {changedPositions} 張卡片改變位置, 小於預期最小值");
        }

        [Fact]
        public void DrawCards_WithValidCount_ReturnsCorrectNumberOfCards()
        {
            // Arrange
            var cards = new List<Card> {
                new Card("S", "A", "SA", "1"),
                new Card("H", "2", "H2", "2"),
                new Card("C", "3", "C3", "3")
            };
            var deck = new Deck(cards);
            int initialCount = deck.Count;
            
            // Act
            var drawnCards = deck.DrawCards(2);
            
            // Assert
            Assert.Equal(2, drawnCards.Count);
            Assert.Equal(initialCount - 2, deck.Count);
        }

        [Fact]
        public void DrawCards_WithZeroCount_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var deck = new Deck(new List<Card> { new Card("S", "A", "SA", "1") });
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => deck.DrawCards(0));
        }

        [Fact]
        public void DiscardCards_WithMultipleCards_MovesAllToDiscardPile()
        {
            // Arrange
            var card1 = new Card("S", "A", "SA", "1");
            var card2 = new Card("H", "2", "H2", "2");
            var card3 = new Card("C", "3", "C3", "3");
            var deck = new Deck(new List<Card> { card1, card2, card3 });
            
            // Act
            deck.DiscardCards(new List<Card> { card1, card2 });
            
            // Assert
            Assert.Equal(1, deck.Count);
            Assert.Equal(2, deck.DiscardPile.Count);
            Assert.Contains(card1, deck.DiscardPile);
            Assert.Contains(card2, deck.DiscardPile);
        }

        [Fact]
        public void DiscardCards_WithNullList_ThrowsArgumentNullException()
        {
            // Arrange
            var deck = new Deck(new List<Card> { new Card("S", "A", "SA", "1") });
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => deck.DiscardCards(null));
        }
    }
}
