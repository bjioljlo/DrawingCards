namespace CardGame.Solitaire;

public static class SuitColor
{
    private static readonly HashSet<string> RedSuits = ["Hearts", "Diamonds"];

    public static bool IsRed(string suit) => RedSuits.Contains(suit);

    public static bool IsSameColor(string suitA, string suitB)
        => IsRed(suitA) == IsRed(suitB);

    public static int RankToNumber(string rank) => rank switch
    {
        "A" => 1,
        "J" => 11,
        "Q" => 12,
        "K" => 13,
        _ => int.TryParse(rank, out var n) ? n : throw new ArgumentException($"Invalid rank: {rank}")
    };
}