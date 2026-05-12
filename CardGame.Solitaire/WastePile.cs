namespace CardGame.Solitaire;

public class WastePile
{
    private readonly List<Card> _cards = [];

    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
    public Card? TopCard => _cards.Count > 0 ? _cards[^1] : null;
    public bool IsEmpty => _cards.Count == 0;

    public void Add(Card card)
    {
        _cards.Add(card);
    }

    public Card TakeTop()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Waste pile is empty");
        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    public List<Card> TakeAll()
    {
        var all = new List<Card>(_cards);
        all.Reverse();
        _cards.Clear();
        return all;
    }
}