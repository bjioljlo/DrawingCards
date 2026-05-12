namespace CardGame.Solitaire;

public class TableauPile
{
    private readonly List<(Card Card, bool IsFaceUp)> _cards = [];

    public int Index { get; }

    public IReadOnlyList<Card> Cards => _cards.Select(c => c.Card).ToList().AsReadOnly();

    public Card? TopCard => _cards.Count > 0 ? _cards[^1].Card : null;

    public bool IsEmpty => _cards.Count == 0;

    public int FaceUpCount => _cards.Count(c => c.IsFaceUp);

    public TableauPile(int index)
    {
        Index = index;
    }

    internal void AddFaceDown(Card card)
    {
        _cards.Add((card, false));
    }

    internal void AddFaceUp(Card card)
    {
        _cards.Add((card, true));
    }

    public bool CanPlace(Card card)
    {
        if (_cards.Count == 0)
            return card.Rank == "K";
        var top = _cards[^1].Card;
        return !SuitColor.IsSameColor(top.Suit, card.Suit)
            && SuitColor.RankToNumber(top.Rank) == SuitColor.RankToNumber(card.Rank) + 1;
    }

    public bool CanPlace(IReadOnlyList<Card> cards)
    {
        if (cards.Count == 0) return false;
        if (!CanPlace(cards[0])) return false;
        for (int i = 0; i < cards.Count - 1; i++)
        {
            var current = cards[i];
            var next = cards[i + 1];
            if (SuitColor.IsSameColor(current.Suit, next.Suit)) return false;
            if (SuitColor.RankToNumber(current.Rank) != SuitColor.RankToNumber(next.Rank) + 1) return false;
        }
        return true;
    }

    public void Place(Card card)
    {
        _cards.Add((card, true));
    }

    public void Place(IReadOnlyList<Card> cards)
    {
        foreach (var card in cards)
            _cards.Add((card, true));
    }

    public List<Card> TakeFrom(int index)
    {
        if (index < 0 || index >= _cards.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        var taken = _cards.Skip(index).Select(c => c.Card).ToList();
        _cards.RemoveRange(index, _cards.Count - index);

        if (_cards.Count > 0 && !_cards[^1].IsFaceUp)
        {
            var last = _cards[^1];
            _cards[^1] = (last.Card, true);
        }

        return taken;
    }

    public Card? GetTopCard()
    {
        return _cards.Count > 0 ? _cards[^1].Card : null;
    }

    public IReadOnlyList<Card>? GetMovableCards(int index)
    {
        if (index < 0 || index >= _cards.Count)
            return null;
        if (_cards.Skip(index).Any(c => !c.IsFaceUp))
            return null;
        var cards = _cards.Skip(index).Select(c => c.Card).ToList();
        for (int i = 0; i < cards.Count - 1; i++)
        {
            var current = cards[i];
            var next = cards[i + 1];
            if (SuitColor.IsSameColor(current.Suit, next.Suit)) return null;
            if (SuitColor.RankToNumber(current.Rank) != SuitColor.RankToNumber(next.Rank) + 1) return null;
        }
        return cards.AsReadOnly();
    }
}