namespace CardGame.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void Player_Initialization_ShouldSetProperties()
        {
            var player = new Player(1, "Alice");
            Assert.Equal(1, player.Id);
            Assert.Equal("Alice", player.Name);
            Assert.NotNull(player.Hand);
            Assert.Equal(0, player.Score);
        }

        [Fact]
        public void DrawCard_ShouldAddCardToHand()
        {
            var player = new Player(1, "Bob");
            var deck = new Deck(new List<Card> { new Card("S", "A", "SA", "1") });
            var initialHandCount = player.Hand.Count;
            player.DrawCard(deck);
            Assert.True(player.Hand.Count > initialHandCount);
        }

        [Fact]
        public void PlayCard_ShouldMoveCardFromHandToTable()
        {
            var player = new Player(1, "Carol");
            var deck = new Deck(new List<Card> { new Card("S", "A", "SA", "1") });
            var table = new Table(5);
            player.DrawCard(deck);
            var card = player.Hand.Cards[0];
            var result = player.PlayCard(card, table);
            Assert.True(result);
            Assert.DoesNotContain(card, player.Hand.Cards);
            Assert.Contains(card, table.GetCards());
        }

        [Fact]
        public void DiscardCard_ShouldMoveCardFromHandToDiscardPile()
        {
            var player = new Player(1, "Dave");
            var deck = new Deck(new List<Card> { new Card("S", "A", "SA", "1") });
            player.DrawCard(deck);
            var card = player.Hand.Cards[0];
            player.DiscardCard(card);
            Assert.DoesNotContain(card, player.Hand.Cards);
            Assert.Contains(card, player.DiscardPile);
        }

        [Fact]
        public void CalculateScore_ShouldReturnCorrectScore()
        {
            var player = new Player(1, "Eve");
            var cards = new List<Card>
            {
                new Card("S", "A", "SA", "10"),
                new Card("H", "K", "HK", "5"),
                new Card("D", "Q", "DQ", "3")
            };
            var deck = new Deck(cards);

            // 抽取所有卡牌
            foreach (var card in cards)
            {
                player.DrawCard(deck);
            }

            int expectedScore = cards.Sum(c => c.Score);
            int actualScore = player.CalculateScore();

            Assert.Equal(expectedScore, actualScore);
        }

        [Fact]
        public void DiscardAll_ShouldMoveAllHandCardsToDiscardPile()
        {
            var player = new Player(1, "Frank");
            var deck = new Deck(new List<Card> {
                new Card("S", "A", "SA", "1"),
                new Card("H", "2", "H2", "2"),
                new Card("C", "3", "C3", "3")
            });
            // 抽三張牌
            player.DrawCard(deck);
            player.DrawCard(deck);
            player.DrawCard(deck);
            var cards = player.Hand.Cards.ToList();
            player.DiscardAll();
            Assert.Empty(player.Hand.Cards);
            Assert.Equal(3, player.DiscardPile.Count);
            foreach (var card in cards)
            {
                Assert.Contains(card, player.DiscardPile);
            }
        }

        [Fact]
        public void DiscardCards_ShouldMoveSpecifiedCardsToDiscardPile()
        {
            var player = new Player(1, "Grace");
            var deck = new Deck(new List<Card> {
                new Card("S", "A", "SA", "1"),
                new Card("H", "2", "H2", "2"),
                new Card("C", "3", "C3", "3"),
                new Card("D", "4", "D4", "4"),
                new Card("S", "5", "S5", "5")
            });
            // 抽五張牌
            for (int i = 0; i < 5; i++) player.DrawCard(deck);
            var toDiscard = player.Hand.Cards.Take(2).ToList();
            player.DiscardCards(toDiscard);
            foreach (var card in toDiscard)
            {
                Assert.DoesNotContain(card, player.Hand.Cards);
                Assert.Contains(card, player.DiscardPile);
            }
        }

        [Fact]
        public void DiscardCards_ShouldHandleEmptyList()
        {
            var player = new Player(1, "Henry");
            var emptyList = new List<Card>();

            player.DiscardCards(emptyList);

            Assert.Empty(player.DiscardPile);
        }

        [Fact]
        public void DiscardCards_ShouldThrowException_WhenListIsNull()
        {
            var player = new Player(1, "Ian");

            Assert.Throws<ArgumentNullException>(() => player.DiscardCards(null));
        }

        [Fact]
        public void DiscardCards_ShouldThrowException_WhenCardNotInHand()
        {
            var player = new Player(1, "Jack");
            var invalidCard = new Card("S", "A", "SA", "1");

            Assert.Throws<InvalidOperationException>(() =>
                player.DiscardCards(new List<Card> { invalidCard }));
        }
    }
}
