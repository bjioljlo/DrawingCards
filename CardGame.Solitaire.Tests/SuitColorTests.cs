namespace CardGame.Solitaire.Tests;

public class SuitColorTests
{
    [Theory]
    [InlineData("Spades", false)]
    [InlineData("Clubs", false)]
    [InlineData("Hearts", true)]
    [InlineData("Diamonds", true)]
    public void IsRed_ShouldReturnCorrectColor(string suit, bool expected)
    {
        var result = SuitColor.IsRed(suit);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Spades", "Clubs")]
    [InlineData("Hearts", "Diamonds")]
    public void IsSameColor_ShouldReturnTrue_ForSameColor(string suitA, string suitB)
    {
        Assert.True(SuitColor.IsSameColor(suitA, suitB));
    }

    [Theory]
    [InlineData("Spades", "Hearts")]
    [InlineData("Clubs", "Diamonds")]
    public void IsSameColor_ShouldReturnFalse_ForDifferentColors(string suitA, string suitB)
    {
        Assert.False(SuitColor.IsSameColor(suitA, suitB));
    }

    [Theory]
    [InlineData("A", 1)]
    [InlineData("2", 2)]
    [InlineData("3", 3)]
    [InlineData("4", 4)]
    [InlineData("5", 5)]
    [InlineData("6", 6)]
    [InlineData("7", 7)]
    [InlineData("8", 8)]
    [InlineData("9", 9)]
    [InlineData("10", 10)]
    [InlineData("J", 11)]
    [InlineData("Q", 12)]
    [InlineData("K", 13)]
    public void RankToNumber_ShouldReturnCorrectValue(string rank, int expected)
    {
        var result = SuitColor.RankToNumber(rank);
        Assert.Equal(expected, result);
    }
}