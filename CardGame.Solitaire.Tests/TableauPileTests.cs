namespace CardGame.Solitaire.Tests;

public class TableauPileTests
{
    private static Card MakeCard(string suit, string rank)
        => new Card(suit, rank, $"{rank} of {suit}", "", "", SuitColor.RankToNumber(rank));

    private static Card K(string suit) => MakeCard(suit, "K");
    private static Card Q(string suit) => MakeCard(suit, "Q");
    private static Card J(string suit) => MakeCard(suit, "J");

    [Fact]
    public void Constructor_ShouldBeEmpty()
    {
        var pile = new TableauPile(0);
        Assert.True(pile.IsEmpty);
        Assert.Null(pile.TopCard);
    }

    [Fact]
    public void Place_KOnEmpty_ShouldSucceed()
    {
        var pile = new TableauPile(0);
        Assert.True(pile.CanPlace(K("Spades")));
        pile.Place(K("Spades"));
        Assert.False(pile.IsEmpty);
        Assert.Equal("K", pile.TopCard!.Rank);
    }

    [Fact]
    public void Place_NonKingOnEmpty_ShouldBeRejected()
    {
        var pile = new TableauPile(0);
        Assert.False(pile.CanPlace(Q("Hearts")));
    }

    [Fact]
    public void Place_RedOnBlackDescending_ShouldSucceed()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        Assert.True(pile.CanPlace(Q("Hearts")));
        pile.Place(Q("Hearts"));
        Assert.Equal("Q", pile.TopCard!.Rank);
    }

    [Fact]
    public void Place_SameColorOnExisting_ShouldBeRejected()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        Assert.False(pile.CanPlace(Q("Clubs")));
    }

    [Fact]
    public void Place_NonDescendingOnExisting_ShouldBeRejected()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        Assert.False(pile.CanPlace(J("Hearts")));
    }

    [Fact]
    public void Place_AscendingOnExisting_ShouldBeRejected()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        Assert.False(pile.CanPlace(MakeCard("Hearts", "A")));
    }

    [Fact]
    public void TakeFrom_ShouldReturnCardsFromIndex()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        pile.Place(Q("Hearts"));
        pile.Place(J("Spades"));
        var taken = pile.TakeFrom(1);
        Assert.Equal(2, taken.Count);
        Assert.Equal("Q", taken[0].Rank);
        Assert.Equal("J", taken[1].Rank);
        Assert.Equal(1, pile.Cards.Count);
    }

    [Fact]
    public void GetMovableCards_ShouldReturnCards_WhenAllFaceUp()
    {
        var pile = new TableauPile(0);
        pile.Place(K("Spades"));
        pile.Place(Q("Hearts"));
        var movable = pile.GetMovableCards(0);
        Assert.NotNull(movable);
        Assert.Equal(2, movable.Count);
    }

    [Fact]
    public void CanPlace_MultipleCards_ShouldValidateSequence()
    {
        var pile = new TableauPile(0);
        var cards = new List<Card> { K("Spades"), Q("Hearts"), J("Spades") };
        Assert.True(pile.CanPlace(cards));
        pile.Place(cards);
        Assert.Equal(3, pile.Cards.Count);
    }

    [Fact]
    public void CanPlace_MultipleCards_InvalidSequence_ShouldBeRejected()
    {
        var pile = new TableauPile(0);
        var cards = new List<Card> { K("Spades"), Q("Clubs"), J("Spades") };
        Assert.False(pile.CanPlace(cards));
    }

    [Fact]
    public void FaceUpCount_ShouldReturnCorrectCount()
    {
        var pile = new TableauPile(0);
        pile.AddFaceDown(MakeCard("Spades", "K"));
        pile.AddFaceDown(MakeCard("Hearts", "Q"));
        pile.AddFaceUp(MakeCard("Spades", "J"));
        Assert.Equal(1, pile.FaceUpCount);
    }

    [Fact]
    public void TakeFrom_ShouldAutoFlipFaceDownCard()
    {
        var pile = new TableauPile(0);
        pile.AddFaceDown(MakeCard("Spades", "K"));
        pile.AddFaceUp(MakeCard("Hearts", "Q"));
        pile.TakeFrom(1); // removes Q
        // K should auto-flip
        Assert.Equal(1, pile.FaceUpCount);
    }
}