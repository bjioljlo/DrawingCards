namespace CardGame.Solitaire;

public class KlondikeGame
{
    public enum GameState { NotStarted, Playing, Won }

    public GameState State { get; private set; } = GameState.NotStarted;
    public StockPile Stock { get; private set; } = null!;
    public WastePile Waste { get; private set; } = null!;
    public IReadOnlyList<TableauPile> Tableaus { get; private set; } = null!;
    public IReadOnlyList<FoundationPile> Foundations { get; private set; } = null!;
    public int MovesCount { get; private set; }

    private readonly List<Card> _fullDeck;

    public KlondikeGame(List<Card> fullDeck)
    {
        _fullDeck = new List<Card>(fullDeck);
    }

    public void StartGame()
    {
        var deck = new List<Card>(_fullDeck);
        var rng = new Random();
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }

        var tableaus = new TableauPile[7];
        for (int i = 0; i < 7; i++)
            tableaus[i] = new TableauPile(i);

        int cardIndex = 0;
        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row <= col; row++)
            {
                var card = deck[cardIndex++];
                if (row == col)
                    tableaus[col].AddFaceUp(card);
                else
                    tableaus[col].AddFaceDown(card);
            }
        }

        var remaining = deck.Skip(cardIndex).ToList();
        Stock = new StockPile(remaining);
        Waste = new WastePile();
        Tableaus = tableaus.ToList().AsReadOnly();

        var foundations = new FoundationPile[4];
        for (int i = 0; i < 4; i++)
            foundations[i] = new FoundationPile();
        Foundations = foundations.ToList().AsReadOnly();

        State = GameState.Playing;
        MovesCount = 0;
    }

    public void DrawFromStock()
    {
        if (State != GameState.Playing) return;
        var card = Stock.Draw();
        if (card != null)
            Waste.Add(card);
    }

    public bool MoveCard(int fromTableau, int toTableau, int cardIndex)
    {
        if (State != GameState.Playing) return false;
        var fromPile = Tableaus[fromTableau];
        var toPile = Tableaus[toTableau];
        var movable = fromPile.GetMovableCards(cardIndex);
        if (movable == null) return false;
        if (!toPile.CanPlace(movable)) return false;

        for (int i = 0; i < movable.Count - 1; i++)
        {
            var cur = movable[i];
            var next = movable[i + 1];
            if (SuitColor.IsSameColor(cur.Suit, next.Suit)) return false;
            if (SuitColor.RankToNumber(cur.Rank) != SuitColor.RankToNumber(next.Rank) + 1) return false;
        }

        var taken = fromPile.TakeFrom(cardIndex);
        toPile.Place(taken);
        MovesCount++;
        return true;
    }

    public bool MoveFromWasteToTableau(int tableauIndex)
    {
        if (State != GameState.Playing || Waste.IsEmpty) return false;
        var card = Waste.TopCard!;
        var pile = Tableaus[tableauIndex];
        if (!pile.CanPlace(card)) return false;
        Waste.TakeTop();
        pile.Place(card);
        MovesCount++;
        return true;
    }

    public bool MoveFromWasteToFoundation(int foundationIndex)
    {
        if (State != GameState.Playing || Waste.IsEmpty) return false;
        var card = Waste.TopCard!;
        var pile = Foundations[foundationIndex];
        if (!pile.CanPlace(card)) return false;
        Waste.TakeTop();
        pile.Place(card);
        MovesCount++;
        return true;
    }

    public bool MoveToFoundation(int fromTableau, int foundationIndex)
    {
        if (State != GameState.Playing) return false;
        var fromPile = Tableaus[fromTableau];
        var card = fromPile.TopCard;
        if (card == null) return false;
        var toPile = Foundations[foundationIndex];
        if (!toPile.CanPlace(card)) return false;
        var taken = fromPile.TakeFrom(fromPile.Cards.Count - 1);
        toPile.Place(taken[0]);
        MovesCount++;
        return true;
    }

    public void RecycleWaste()
    {
        if (State != GameState.Playing) return;
        Stock.ResetFromWaste(Waste);
    }

    public bool CheckWin()
    {
        return Foundations.All(f => f.IsComplete);
    }

    public void Restart()
    {
        State = GameState.NotStarted;
        Stock = null!;
        Waste = null!;
        Tableaus = null!;
        Foundations = null!;
        MovesCount = 0;
    }
}