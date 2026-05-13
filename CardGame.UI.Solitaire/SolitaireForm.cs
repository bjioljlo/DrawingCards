using CardGameSolitaire = CardGame.Solitaire;

namespace CardGame.UI.Solitaire;

public class SolitaireForm : Form
{
    private CardGameSolitaire.KlondikeGame? _game;
    private readonly SolitaireCanvas _canvas;
    private Label _lblMoves = null!, _lblState = null!;

    public SolitaireForm()
    {
        Text = "Klondike Solitaire";
        MinimumSize = new Size(700, 550);
        StartPosition = FormStartPosition.CenterScreen;

        _canvas = new SolitaireCanvas { Dock = DockStyle.Fill };
        _canvas.GameClicked += OnCanvasClicked;
        Controls.Add(_canvas);

        var sp = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.DimGray };
        var btn = new Button
        {
            Text = "新遊戲",
            Location = new Point(10, 8),
            Size = new Size(100, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.WhiteSmoke
        };
        btn.Click += (_, _) => NewGame();
        sp.Controls.Add(btn);
        _lblMoves = new Label
        {
            Text = "移動次數: 0",
            Location = new Point(140, 12),
            Size = new Size(120, 30),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        sp.Controls.Add(_lblMoves);
        _lblState = new Label
        {
            Text = "點擊「新遊戲」開始",
            Location = new Point(300, 12),
            Size = new Size(300, 30),
            ForeColor = Color.LightGreen,
            Font = new Font("Segoe UI", 11)
        };
        sp.Controls.Add(_lblState);
        Controls.Add(sp);

        Size = new Size(800, 680);
    }

    private void OnCanvasClicked(object? sender, GameClickEventArgs e)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing) return;

        switch (e.Action)
        {
            case ClickAction.Stock:
                if (_game.Stock.IsEmpty && !_game.Waste.IsEmpty) _game.RecycleWaste();
                else _game.DrawFromStock();
                _canvas.ClearSel();
                break;
            case ClickAction.WasteToFoundation:
                _game.MoveFromWasteToFoundation(e.Index);
                _canvas.ClearSel();
                break;
            case ClickAction.FoundationFromTableau:
                if (_canvas.SelectedTableauCol.HasValue && _game.Foundations[e.Index].CanPlace(_canvas.SelectedCard!))
                {
                    _game.MoveToFoundation(_canvas.SelectedTableauCol.Value, e.Index);
                    _canvas.ClearSel();
                }
                break;
            case ClickAction.FoundationFromWaste:
                if (_game.Foundations[e.Index].CanPlace(_canvas.SelectedCard!))
                {
                    _game.MoveFromWasteToFoundation(e.Index);
                    _canvas.ClearSel();
                }
                break;
            case ClickAction.MoveTableauToTableau:
                _game.MoveCard(e.FromCol, e.Col, e.FromCardIndex);
                _canvas.ClearSel();
                break;
            case ClickAction.WasteToTableau:
                _game.MoveFromWasteToTableau(e.Col);
                _canvas.ClearSel();
                break;
        }
        _lblMoves.Text = $"移動次數: {_game.MovesCount}";
        if (_game.CheckWin()) { _lblState.Text = "\u2728 你贏了！ \u2728"; _lblState.ForeColor = Color.Gold; }
        _canvas.Refresh();
    }

    private void NewGame()
    {
        var d = new List<global::CardGame.Card>();
        foreach (var s in new[] { "Spades", "Hearts", "Diamonds", "Clubs" })
            foreach (var r in new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" })
                d.Add(new(s, r, $"{r} of {s}", "", "", r switch { "A" => 1, "J" => 11, "Q" => 12, "K" => 13, _ => int.Parse(r) }));
        _game = new CardGameSolitaire.KlondikeGame(d);
        _game.StartGame();
        _canvas.SetGame(_game);
        _lblState.Text = "遊戲進行中"; _lblState.ForeColor = Color.LightGreen;
        _lblMoves.Text = "移動次數: 0";
        _canvas.Refresh();
    }
}

internal enum ClickAction { None, Stock, WasteToFoundation, FoundationFromWaste, FoundationFromTableau, MoveTableauToTableau, WasteToTableau }

internal class GameClickEventArgs : EventArgs
{
    public ClickAction Action { get; init; }
    public int Col { get; init; }
    public int Index { get; init; }
    public int CardIndex { get; init; }
    public int FromCol { get; init; }
    public int FromCardIndex { get; init; }
}

internal class SolitaireCanvas : Control
{
    private CardGameSolitaire.KlondikeGame? _game;
    public global::CardGame.Card? SelectedCard { get; private set; }
    public int? SelectedTableauCol { get; private set; }
    private int? _selectedCardIndex;

    private const int CW = 70, CH = 100, OL = 28, TH = 140;
    private readonly Rectangle _sr = new(20, 45, 70, 100);
    private readonly Rectangle _wr = new(105, 45, 70, 100);
    private readonly Rectangle[] _fr;
    private Rectangle[] _tr = new Rectangle[7];

    public event EventHandler<GameClickEventArgs>? GameClicked;

    public SolitaireCanvas()
    {
        DoubleBuffered = true;
        BackColor = Color.DarkGreen;
        _fr = new Rectangle[4];
        for (int i = 0; i < 4; i++)
            _fr[i] = new(280 + i * 75, 45, 70, 100);
    }

    public void SetGame(CardGameSolitaire.KlondikeGame game) { _game = game; }
    public void ClearSel() { SelectedCard = null; SelectedTableauCol = null; _selectedCardIndex = null; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;

        // Top bar
        using (var b = new SolidBrush(Color.ForestGreen))
            g.FillRectangle(b, 0, 0, Width, TH);
        using var tf = new Font("Segoe UI", 14, FontStyle.Bold);
        g.DrawString("Klondike Solitaire", tf, Brushes.White, 10, 5);

        // Layout columns
        int cw = Math.Min(80, (Width - 30) / 7);
        for (int i = 0; i < 7; i++)
            _tr[i] = new(15 + i * cw, TH + 5, CW, Height - TH - 5);

        // Stock
        g.FillRectangle(Brushes.DarkBlue, _sr);
        g.DrawRectangle(Pens.Black, _sr);

        // Waste
        var wcard = _game?.Waste.TopCard;
        if (wcard != null)
            DrawCard(g, wcard, _wr, SelectedCard == wcard && SelectedTableauCol == null);
        else
            DrawSlot(g, _wr, "廢牌");

        // Recycle hint
        if (_game != null && _game.Stock.IsEmpty && !_game.Waste.IsEmpty)
            using (var f = new Font("Segoe UI", 8))
                g.DrawString("回收", f, Brushes.Black, _sr.X + 14, _sr.Y + 40);

        // Foundations
        for (int i = 0; i < 4; i++)
        {
            var top = _game?.Foundations[i].TopCard;
            if (top != null)
                DrawCard(g, top, _fr[i], false);
            else
            {
                DrawSlot(g, _fr[i], "");
                var s = i switch { 0 => "\u2660", 1 => "\u2665", 2 => "\u2666", 3 => "\u2663", _ => "?" };
                using var f = new Font("Segoe UI", 16);
                g.DrawString(s, f, Brushes.Gray, _fr[i].X + 25, _fr[i].Y + 35);
            }
        }

        // Tableau
        if (_game == null) return;
        for (int c = 0; c < 7; c++)
        {
            var r = _tr[c];
            var pile = _game.Tableaus[c];
            if (pile.IsEmpty) { DrawSlot(g, new(r.X, r.Y, CW, CH), ""); continue; }

            int y = r.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool up = i >= pile.Cards.Count - pile.FaceUpCount;
                if (up)
                {
                    DrawCard(g, pile.Cards[i], new Rectangle(r.X, y, CW, CH), SelectedCard == pile.Cards[i] && SelectedTableauCol == c);
                    y += OL;
                }
                else
                {
                    DrawDown(g, new Rectangle(r.X, y, CW, 20));
                    y += 5;
                }
            }
        }
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing) return;
        var p = e.Location;

        // Stock
        if (_sr.Contains(p))
        {
            GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.Stock });
            return;
        }

        // Waste -> select or put to foundation
        if (_wr.Contains(p) && !_game.Waste.IsEmpty)
        {
            if (SelectedCard != null && SelectedTableauCol == null)
            {
                // Try each foundation
                for (int i = 0; i < 4; i++)
                {
                    if (_game.Foundations[i].CanPlace(SelectedCard))
                    {
                        GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.WasteToFoundation, Index = i });
                        Invalidate();
                        return;
                    }
                }
                ClearSel(); Invalidate();
                return;
            }
            SelectedCard = _game.Waste.TopCard;
            SelectedTableauCol = null;
            _selectedCardIndex = null;
            Invalidate();
            return;
        }

        // Foundations
        for (int i = 0; i < 4; i++)
        {
            if (!_fr[i].Contains(p)) continue;
            if (SelectedCard == null) return;
            if (SelectedTableauCol == null)
                GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.WasteToFoundation, Index = i });
            else
                GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.FoundationFromTableau, Index = i });
            return;
        }

        // Tableau
        for (int c = 0; c < 7; c++)
        {
            var cr = _tr[c];
            if (p.X < cr.X || p.X > cr.X + CW) continue;
            var pile = _game.Tableaus[c];

            // Find clicked card by Y position
            int ci = -1, y = cr.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool up = i >= pile.Cards.Count - pile.FaceUpCount;
                int h = up ? CH : 20;
                if (p.Y >= y && p.Y <= y + h) { ci = i; break; }
                y += up ? OL : 5;
            }

            if (ci == -1)
            {
                // Empty column / below cards
                if (SelectedCard == null) return;
                if (SelectedTableauCol == null)
                    GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.WasteToTableau, Col = c });
                else
                    GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.MoveTableauToTableau, Col = c, FromCol = SelectedTableauCol.Value, FromCardIndex = _selectedCardIndex ?? 0 });
                return;
            }

            // Clicked a card
            if (SelectedCard == null)
            {
                // Only selectable if the cascade from here is valid
                var movable = pile.GetMovableCards(ci);
                if (movable != null)
                {
                    SelectedCard = pile.Cards[ci];
                    SelectedTableauCol = c;
                    _selectedCardIndex = ci;
                    Invalidate();
                }
                return;
            }

            if (SelectedTableauCol == c)
            {
                if (SelectedCard == pile.Cards[ci]) { ClearSel(); Invalidate(); return; }
                if (pile.CanPlace(SelectedCard))
                    GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.MoveTableauToTableau, Col = c, FromCol = c, FromCardIndex = _selectedCardIndex ?? 0 });
                return;
            }

            if (pile.CanPlace(SelectedCard))
            {
                if (SelectedTableauCol.HasValue)
                    GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.MoveTableauToTableau, Col = c, FromCol = SelectedTableauCol.Value, FromCardIndex = _selectedCardIndex ?? 0 });
                else
                    GameClicked?.Invoke(this, new GameClickEventArgs { Action = ClickAction.WasteToTableau, Col = c });
            }
            else
            { ClearSel(); Invalidate(); }
            return;
        }
    }

    private static void DrawSlot(Graphics g, Rectangle r, string s)
    {
        using var p = new Pen(Color.FromArgb(0, 100, 0), 2); g.DrawRectangle(p, r);
        if (s != "") { using var f = new Font("Segoe UI", 8); g.DrawString(s, f, Brushes.DarkGray, r.X + 15, r.Y + 40); }
    }
    private static void DrawDown(Graphics g, Rectangle r)
    {
        g.FillRectangle(Brushes.DarkBlue, r); g.DrawRectangle(Pens.Black, r);
        using var p = new Pen(Color.Navy); g.DrawLine(p, r.Left + 2, r.Top + 2, r.Right - 2, r.Bottom - 2);
        g.DrawLine(p, r.Right - 2, r.Top + 2, r.Left + 2, r.Bottom - 2);
    }
    private static void DrawCard(Graphics g, global::CardGame.Card c, Rectangle r, bool sel)
    {
        g.FillRectangle(new SolidBrush(sel ? Color.LightYellow : Color.White), r);
        bool red = c.Suit is "Hearts" or "Diamonds";
        using var sb = new SolidBrush(red ? Color.Red : Color.Black);
        using var bp = new Pen(sel ? Color.Blue : Color.Black, sel ? 3 : 1); g.DrawRectangle(bp, r);
        using var rf = new Font("Segoe UI", 11, FontStyle.Bold); g.DrawString(c.Rank, rf, sb, r.X + 4, r.Y + 2);
        string sym = c.Suit switch { "Spades" => "\u2660", "Hearts" => "\u2665", "Diamonds" => "\u2666", "Clubs" => "\u2663", _ => "?" };
        using var sf = new Font("Segoe UI", 10); g.DrawString(sym, sf, sb, r.X + 6, r.Y + 20);
        using var cf = new Font("Segoe UI", 28); g.DrawString(sym, cf, sb, r.X + 18, r.Y + 50);
    }
}