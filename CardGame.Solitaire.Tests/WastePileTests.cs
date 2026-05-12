namespace CardGame.Solitaire.Tests;

public class WastePileTests
{
    private static Card MakeCard(string suit, string rank)
        => new Card(suit, rank, $"{rank} of {suit}", "", "", SuitColor.RankToNumber(rank));

    [Fact]
    public void Constructor_ShouldBeEmpty()
    {
        var waste = new WastePile();
        Assert.True(waste.IsEmpty);
        Assert.Null(waste.TopCard);
    }

    [Fact]
    public void Add_ShouldAddToTop()
    {
        var waste = new WastePile();
        waste.Add(MakeCard("Spades", "A"));
        Assert.Equal("A", waste.TopCard!.Rank);
        waste.Add(MakeCard("Spades", "2"));
        Assert.Equal("2", waste.TopCard!.Rank);
    }

    [Fact]
    public void TakeTop_ShouldRemoveAndReturn()
    {
        var waste = new WastePile();
        waste.Add(MakeCard("Spades", "A"));
        waste.Add(MakeCard("Spades", "2"));
        var taken = waste.TakeTop();
        Assert.Equal("2", taken.Rank);
        Assert.Equal("A", waste.TopCard!.Rank);
    }

    [Fact]
    public void TakeAll_ShouldReturnAllCards()
    {
        var waste = new WastePile();
        waste.Add(MakeCard("Spades", "A"));
        waste.Add(MakeCard("Spades", "2"));
        var all = waste.TakeAll();
        Assert.Equal(2, all.Count);
        Assert.True(waste.IsEmpty);
    }

    [Fact]
    public void TakeTop_WhenEmpty_ShouldThrow()
    {
        var waste = new WastePile();
        Assert.Throws<InvalidOperationException>(() => waste.TakeTop());
    }
}