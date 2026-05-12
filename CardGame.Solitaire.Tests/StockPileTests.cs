namespace CardGame.Solitaire.Tests;

public class StockPileTests
{
    private static Card MakeCard(string suit, string rank)
        => new Card(suit, rank, $"{rank} of {suit}", "", "", SuitColor.RankToNumber(rank));

    [Fact]
    public void Constructor_ShouldSetRemainingCards()
    {
        var cards = new List<Card> { MakeCard("Spades", "A") };
        var stock = new StockPile(cards);
        Assert.Equal(1, stock.RemainingCards);
        Assert.False(stock.IsEmpty);
    }

    [Fact]
    public void Draw_ShouldReturnTopCard()
    {
        var card = MakeCard("Spades", "A");
        var stock = new StockPile(new List<Card> { card });
        var drawn = stock.Draw();
        Assert.NotNull(drawn);
        Assert.Equal("A", drawn.Rank);
        Assert.True(stock.IsEmpty);
    }

    [Fact]
    public void Draw_WhenEmpty_ShouldReturnNull()
    {
        var stock = new StockPile(new List<Card>());
        Assert.Null(stock.Draw());
    }

    [Fact]
    public void ResetFromWaste_ShouldMoveAllCardsBack()
    {
        var waste = new WastePile();
        waste.Add(MakeCard("Spades", "A"));
        waste.Add(MakeCard("Hearts", "2"));
        var stock = new StockPile(new List<Card>());
        stock.ResetFromWaste(waste);
        Assert.Equal(2, stock.RemainingCards);
        Assert.True(waste.IsEmpty);
    }
}