namespace CardGame.Solitaire;

public class StockPile
{
    private readonly Stack<Card> _cards;

    public int RemainingCards => _cards.Count;
    public bool IsEmpty => _cards.Count == 0;

    public StockPile(IEnumerable<Card> cards)
    {
        _cards = new Stack<Card>(cards);
    }

    public Card? Draw()
    {
        return _cards.Count > 0 ? _cards.Pop() : null;
    }

    public void ResetFromWaste(WastePile waste)
    {
        var recycled = waste.TakeAll();
        // TakeAll 已經返回反向順序的牌，直接 Push 即可保持原有出牌順序
        foreach (var card in recycled)
            _cards.Push(card);
    }
}