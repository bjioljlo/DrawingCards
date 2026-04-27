namespace CardGame.Tests
{
    public class TableTests
    {
        private Card CreateCard(string id = "1", string suit = "S", string rank = "A")
        {
            return new Card(suit, rank, $"{suit}{rank}", id);
        }

        [Fact]
        public void AddCard_ShouldAddCard_WhenBelowMax()
        {
            var table = new Table(2);
            var card1 = CreateCard("1");
            var card2 = CreateCard("2");
            Assert.True(table.AddCard(card1));
            Assert.True(table.AddCard(card2));
            Assert.Equal(2, table.GetCards().Count);
        }

        [Fact]
        public void AddCard_ShouldFail_WhenAtMax()
        {
            var table = new Table(1);
            var card1 = CreateCard("1");
            var card2 = CreateCard("2");
            Assert.True(table.AddCard(card1));
            Assert.False(table.AddCard(card2));
        }

        [Fact]
        public void RemoveCard_ShouldRemoveCard_IfExists()
        {
            var table = new Table(2);
            var card1 = CreateCard("1");
            table.AddCard(card1);
            Assert.True(table.RemoveCard(card1));
            Assert.Empty(table.GetCards());
        }

        [Fact]
        public void RemoveCard_ShouldReturnFalse_IfNotExists()
        {
            var table = new Table(2);
            var card1 = CreateCard("1");
            Assert.False(table.RemoveCard(card1));
        }

        [Fact]
        public void SwapCards_ShouldSwap_WhenValidIndexes()
        {
            var table = new Table(3);
            var card1 = CreateCard("1");
            var card2 = CreateCard("2");
            table.AddCard(card1);
            table.AddCard(card2);
            Assert.True(table.SwapCards(0, 1));
            Assert.Equal(card2, table.GetCards()[0]);
            Assert.Equal(card1, table.GetCards()[1]);
        }

        [Fact]
        public void SwapCards_ShouldFail_WhenInvalidIndexes()
        {
            var table = new Table(2);
            var card1 = CreateCard("1");
            table.AddCard(card1);
            Assert.False(table.SwapCards(0, 1));
            Assert.False(table.SwapCards(-1, 0));
            Assert.False(table.SwapCards(0, 0));
        }

        [Fact]
        public void GetCards_ShouldReturnReadOnlyList()
        {
            var table = new Table(2);
            var card1 = CreateCard("1");
            table.AddCard(card1);
            var cards = table.GetCards();
            Assert.Single(cards);
            Assert.Equal(card1, cards[0]);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMaxCardsIsZeroOrNegative()
        {
            Assert.Throws<System.ArgumentException>(() => new Table(0));
            Assert.Throws<System.ArgumentException>(() => new Table(-1));
        }
    }
}
