namespace CardGame.Tests
{
    public class HandTests
    {
        [Fact]
        public void Add_AddsCardToHand()
        {
            // Arrange
            var hand = new Hand();
            var card = new Card("Spades", "A", "Ace of Spades", "1");
            var initialCount = hand.Count;

            // Act
            hand.Add(card);

            // Assert
            Assert.Equal(initialCount + 1, hand.Count);
            Assert.Contains(card, hand.Cards);
            Assert.Equal(card, hand.Cards.Last());
        }

        [Fact]
        public void Add_ThrowsArgumentNullException_WhenCardIsNull()
        {
            var hand = new Hand();
            Assert.Throws<ArgumentNullException>(() => hand.Add(null));
        }

        [Fact]
        public void Add_AllowsDuplicateCards()
        {
            // Arrange
            var hand = new Hand();
            var card = new Card("Spades", "A", "Ace of Spades", "1");

            // Act
            hand.Add(card);
            hand.Add(card);

            // Assert
            Assert.Equal(2, hand.Count);
            Assert.Equal(2, hand.Cards.Count(c => c.ID == card.ID));
            Assert.Equal(2, hand.Cards.Count(c => c.Name == card.Name));
        }

        [Fact]
        public void Discard_RemovesCardFromHand()
        {
            var hand = new Hand();
            var card = new Card("Hearts", "K", "King of Hearts", "2", "Test card");
            hand.Add(card);

            var discarded = hand.Discard(card);

            Assert.Equal(card, discarded);
            Assert.Empty(hand.Cards);
        }

        [Fact]
        public void Discard_ThrowsIfCardNotInHand()
        {
            var hand = new Hand();
            var card = new Card("Clubs", "Q", "Queen of Clubs", "3", "Test card");

            Assert.Throws<InvalidOperationException>(() => hand.Discard(card));
        }

        [Fact]
        public void GetCardNames_ReturnsCorrectNames()
        {
            var hand = new Hand();
            var card1 = new Card("Diamonds", "10", "Ten of Diamonds", "4", "Test card");
            var card2 = new Card("Spades", "2", "Two of Spades", "5", "Test card");
            hand.Add(card1);
            hand.Add(card2);

            var names = hand.GetCardNames();

            Assert.Equal(2, names.Count);
            Assert.Contains("Ten of Diamonds", names);
            Assert.Contains("Two of Spades", names);
        }

        [Fact]
        public void Count_ReturnsNumberOfCardsInHand()
        {
            var hand = new Hand();
            Assert.Equal(0, hand.Count);

            hand.Add(new Card("Hearts", "5", "Five of Hearts", "6", "Test card"));
            Assert.Equal(1, hand.Count);

            hand.Add(new Card("Clubs", "7", "Seven of Clubs", "7", "Test card"));
            Assert.Equal(2, hand.Count);

            hand.Discard(hand.Cards[0]);
            Assert.Equal(1, hand.Count);
        }
    }
}