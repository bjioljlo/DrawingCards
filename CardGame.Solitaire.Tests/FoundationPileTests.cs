namespace CardGame.Solitaire.Tests;

public class FoundationPileTests
{
    private static Card MakeCard(string suit, string rank)
        => new Card(suit, rank, $"{rank} of {suit}", "", "", SuitColor.RankToNumber(rank));

    [Fact]
    public void Constructor_ShouldBeEmpty()
    {
        var pile = new FoundationPile();
        Assert.True(pile.IsEmpty);
        Assert.Null(pile.TopCard);
        Assert.False(pile.IsComplete);
    }

    [Fact]
    public void Place_AceOnEmpty_ShouldSucceed()
    {
        var pile = new FoundationPile();
        var ace = MakeCard("Spades", "A");
        Assert.True(pile.CanPlace(ace));
        pile.Place(ace);
        Assert.Equal("A", pile.TopCard!.Rank);
    }

    [Fact]
    public void Place_NonAceOnEmpty_ShouldBeRejected()
    {
        var pile = new FoundationPile();
        Assert.False(pile.CanPlace(MakeCard("Spades", "2")));
    }

    [Fact]
    public void Place_SameSuitAscending_ShouldSucceed()
    {
        var pile = new FoundationPile();
        pile.Place(MakeCard("Spades", "A"));
        Assert.True(pile.CanPlace(MakeCard("Spades", "2")));
        pile.Place(MakeCard("Spades", "2"));
        Assert.Equal("2", pile.TopCard!.Rank);
    }

    [Fact]
    public void Place_DifferentSuit_ShouldBeRejected()
    {
        var pile = new FoundationPile();
        pile.Place(MakeCard("Spades", "A"));
        Assert.False(pile.CanPlace(MakeCard("Hearts", "2")));
    }

    [Fact]
    public void Place_NonContinuous_ShouldBeRejected()
    {
        var pile = new FoundationPile();
        pile.Place(MakeCard("Spades", "A"));
        Assert.False(pile.CanPlace(MakeCard("Spades", "3")));
    }

    [Fact]
    public void IsComplete_WhenTopIsKing_ShouldReturnTrue()
    {
        var pile = new FoundationPile();
        var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        foreach (var rank in ranks)
            pile.Place(MakeCard("Spades", rank));
        Assert.True(pile.IsComplete);
    }

    [Fact]
    public void IsComplete_WhenNotKing_ShouldReturnFalse()
    {
        var pile = new FoundationPile();
        pile.Place(MakeCard("Spades", "A"));
        pile.Place(MakeCard("Spades", "2"));
        Assert.False(pile.IsComplete);
    }
}