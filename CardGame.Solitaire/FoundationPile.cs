namespace CardGame.Solitaire;

public class FoundationPile
{
    private readonly List<Card> _cards = [];

    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();

    public Card? TopCard => _cards.Count > 0 ? _cards[^1] : null;

    public bool IsEmpty => _cards.Count == 0;

    public bool IsComplete => _cards.Count == 13 && TopCard?.Rank == "K";

    public bool CanPlace(Card card)
    {
        if (_cards.Count == 0)
            return card.Rank == "A";
        var top = _cards[^1];
        return top.Suit == card.Suit
            && SuitColor.RankToNumber(top.Rank) + 1 == SuitColor.RankToNumber(card.Rank);
    }

    public void Place(Card card)
    {
        _cards.Add(card);
    }
}