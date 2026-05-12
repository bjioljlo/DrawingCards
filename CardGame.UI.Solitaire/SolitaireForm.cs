using CardGameSolitaire = CardGame.Solitaire;

namespace CardGame.UI.Solitaire;

public class SolitaireForm : Form
{
    private CardGameSolitaire.KlondikeGame? _game;
    private global::CardGame.Card? _selectedCard;
    private int? _selectedTableauCol;
    private int? _selectedCardIndex;

    private const int CardW = 70;
    private const int CardH = 100;
    private const int Overlap = 28;
    private const int TopH = 140;
    private const int FndStartX = 280;
    private const int FndSpacing = 75;
    private const int StatusH = 50;

    private readonly Rectangle _stockRect = new(20, 45, 70, 100);
    private readonly Rectangle _wasteRect = new(105, 45, 70, 100);
    private readonly Rectangle[] _fndRects;
    private Rectangle[] _tabRects = new Rectangle[7];

    private Button _btnNew = null!;
    private Label _lblMoves = null!;
    private Label _lblState = null!;
    private Panel _canvas = null!;

    public SolitaireForm()
    {
        Text = "Klondike Solitaire";
        MinimumSize = new Size(700, 550);
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        _fndRects = new Rectangle[4];
        for (int i = 0; i < 4; i++)
            _fndRects[i] = new Rectangle(FndStartX + i * FndSpacing, 45, 70, 100);

        // Canvas fills everything except status bar
        _canvas = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.DarkGreen
        };
        _canvas.Paint += Canvas_Paint;
        _canvas.MouseClick += Canvas_MouseClick;
        Controls.Add(_canvas);

        // Status bar
        var status = new Panel { Dock = DockStyle.Bottom, Height = StatusH, BackColor = Color.DimGray };
        _btnNew = new Button { Text = "新遊戲", Location = new Point(10, 8), Size = new Size(100, 34),
            FlatStyle = FlatStyle.Flat, BackColor = Color.WhiteSmoke };
        _btnNew.Click += (_, _) => StartNewGame();
        status.Controls.Add(_btnNew);

        _lblMoves = new Label { Text = "移動次數: 0", Location = new Point(140, 12), Size = new Size(120, 30),
            ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        status.Controls.Add(_lblMoves);

        _lblState = new Label { Text = "點擊「新遊戲」開始", Location = new Point(300, 12), Size = new Size(300, 30),
            ForeColor = Color.LightGreen, Font = new Font("Segoe UI", 11) };
        status.Controls.Add(_lblState);

        Controls.Add(status);
        Size = new Size(800, 680);
    }

    private void ComputeTabRects()
    {
        int w = _canvas.ClientSize.Width;
        int cw = Math.Min(80, (w - 30) / 7);
        int ch = _canvas.ClientSize.Height - TopH - 15;
        for (int i = 0; i < 7; i++)
            _tabRects[i] = new Rectangle(15 + i * cw, TopH + 5, CardW, Math.Max(ch, 100));
    }

    private void Canvas_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.DarkGreen);

        // Top bar
        using (var tb = new SolidBrush(Color.ForestGreen))
            g.FillRectangle(tb, 0, 0, _canvas.ClientSize.Width, TopH);
        using (var tf = new Font("Segoe UI", 14, FontStyle.Bold))
            g.DrawString("Klondike Solitaire", tf, Brushes.White, 10, 5);

        ComputeTabRects();

        // Stock
        DrawStock(g, _stockRect);

        // Waste
        if (_game != null && !_game.Waste.IsEmpty)
            DrawCard(g, _game.Waste.TopCard!, _wasteRect,
                _selectedCard != null && _selectedTableauCol == null && _selectedCard == _game.Waste.TopCard);
        else
            DrawSlot(g, _wasteRect, "廢牌");

        // Recycle hint
        if (_game != null && _game.Stock.IsEmpty && !_game.Waste.IsEmpty)
            using (var f = new Font("Segoe UI", 8))
                g.DrawString("回收", f, Brushes.Black, _stockRect.X + 14, _stockRect.Y + 40);

        // Foundations
        for (int i = 0; i < 4; i++)
        {
            if (_game != null && i < _game.Foundations.Count && !_game.Foundations[i].IsEmpty)
                DrawCard(g, _game.Foundations[i].TopCard!, _fndRects[i], false);
            else
            {
                DrawSlot(g, _fndRects[i], "");
                var sym = i switch { 0 => "\u2660", 1 => "\u2665", 2 => "\u2666", 3 => "\u2663", _ => "?" };
                using var f = new Font("Segoe UI", 16);
                g.DrawString(sym, f, Brushes.Gray, _fndRects[i].X + 25, _fndRects[i].Y + 35);
            }
        }

        // Tableau
        if (_game == null) return;
        for (int col = 0; col < 7; col++)
        {
            var r = _tabRects[col];
            var pile = _game.Tableaus[col];
            if (pile.IsEmpty) { DrawSlot(g, new(r.X, r.Y, CardW, CardH), ""); continue; }

            int y = r.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool up = i >= pile.Cards.Count - pile.FaceUpCount;
                var cr = new Rectangle(r.X, y, CardW, up ? CardH : 20);
                if (up) DrawCard(g, pile.Cards[i], cr, _selectedCard == pile.Cards[i] && _selectedTableauCol == col);
                else DrawDown(g, cr);
                y += up ? Overlap : 5;
            }
        }
    }

    private void Canvas_MouseClick(object? sender, MouseEventArgs e)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing) return;
        var p = e.Location;

        // Stock
        if (_stockRect.Contains(p))
        {
            if (_game.Stock.IsEmpty && !_game.Waste.IsEmpty) _game.RecycleWaste();
            else _game.DrawFromStock();
            ClearSel(); _canvas.Invalidate(); return;
        }

        // Waste
        if (_wasteRect.Contains(p) && !_game.Waste.IsEmpty)
        {
            if (_selectedCard != null && _selectedTableauCol == null)
            {
                for (int i = 0; i < 4; i++)
                    if (_game.Foundations[i].CanPlace(_selectedCard))
                    { _game.MoveFromWasteToFoundation(i); ClearSel(); _canvas.Invalidate(); CheckWin(); return; }
                ClearSel(); _canvas.Invalidate(); return;
            }
            _selectedCard = _game.Waste.TopCard; _selectedTableauCol = null; _selectedCardIndex = null;
            _canvas.Invalidate(); return;
        }

        // Foundations
        for (int i = 0; i < 4; i++)
        {
            if (!_fndRects[i].Contains(p)) continue;
            if (_selectedCard != null && _selectedTableauCol == null && _game.Foundations[i].CanPlace(_selectedCard))
            { _game.MoveFromWasteToFoundation(i); ClearSel(); _canvas.Invalidate(); CheckWin(); return; }
            if (_selectedCard != null && _selectedTableauCol.HasValue && _game.Foundations[i].CanPlace(_selectedCard))
            { _game.MoveToFoundation(_selectedTableauCol.Value, i); ClearSel(); _canvas.Invalidate(); CheckWin(); return; }
            return;
        }

        // Tableau
        for (int col = 0; col < 7; col++)
        {
            var cr = _tabRects[col];
            if (p.X < cr.X || p.X > cr.X + CardW) continue;
            var pile = _game.Tableaus[col];

            int ci = -1, y = cr.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool up = i >= pile.Cards.Count - pile.FaceUpCount;
                int bot = y + (up ? CardH : 20);
                if (p.Y >= y && p.Y <= bot) { ci = i; break; }
                y += up ? Overlap : 5;
            }

            if (ci == -1)
            {
                if (_selectedCard == null) return;
                if (_selectedTableauCol == null) { if (pile.CanPlace(_selectedCard)) { _game.MoveFromWasteToTableau(col); ClearSel(); _canvas.Invalidate(); CheckWin(); } }
                else { if (pile.CanPlace(_selectedCard)) { _game.MoveCard(_selectedTableauCol.Value, col, _selectedCardIndex ?? 0); ClearSel(); _canvas.Invalidate(); } }
                return;
            }

            var card = pile.Cards[ci];
            if (_selectedCard == null)
            {
                if (pile.GetMovableCards(ci) != null) { _selectedCard = card; _selectedTableauCol = col; _selectedCardIndex = ci; _canvas.Invalidate(); }
                return;
            }

            if (_selectedTableauCol == col)
            {
                if (_selectedCard == card) { ClearSel(); _canvas.Invalidate(); return; }
                if (pile.CanPlace(_selectedCard)) { _game.MoveCard(col, col, _selectedCardIndex ?? 0); ClearSel(); _canvas.Invalidate(); CheckWin(); }
                return;
            }

            var tgt = _game.Tableaus[col];
            if (tgt.CanPlace(_selectedCard)) { _game.MoveCard(_selectedTableauCol!.Value, col, _selectedCardIndex ?? 0); ClearSel(); _canvas.Invalidate(); CheckWin(); }
            else { ClearSel(); _canvas.Invalidate(); }
            return;
        }
    }

    private void ClearSel() { _selectedCard = null; _selectedTableauCol = null; _selectedCardIndex = null; }

    // Drawing
    private static void DrawStock(Graphics g, Rectangle r)
    { g.FillRectangle(Brushes.DarkBlue, r); g.DrawRectangle(Pens.Black, r); }
    private static void DrawSlot(Graphics g, Rectangle r, string s)
    { using var p = new Pen(Color.FromArgb(0, 100, 0), 2); g.DrawRectangle(p, r);
        if (s != "") { using var f = new Font("Segoe UI", 8); g.DrawString(s, f, Brushes.DarkGray, r.X + 15, r.Y + 40); } }
    private static void DrawDown(Graphics g, Rectangle r)
    { g.FillRectangle(Brushes.DarkBlue, r); g.DrawRectangle(Pens.Black, r);
        using var p = new Pen(Color.Navy); g.DrawLine(p, r.Left + 2, r.Top + 2, r.Right - 2, r.Bottom - 2);
        g.DrawLine(p, r.Right - 2, r.Top + 2, r.Left + 2, r.Bottom - 2); }
    private static void DrawCard(Graphics g, global::CardGame.Card c, Rectangle r, bool sel)
    { g.FillRectangle(new SolidBrush(sel ? Color.LightYellow : Color.White), r);
        var red = c.Suit is "Hearts" or "Diamonds";
        using var sb = new SolidBrush(red ? Color.Red : Color.Black);
        using var bp = new Pen(sel ? Color.Blue : Color.Black, sel ? 3 : 1); g.DrawRectangle(bp, r);
        using var rf = new Font("Segoe UI", 11, FontStyle.Bold); g.DrawString(c.Rank, rf, sb, r.X + 4, r.Y + 2);
        var sym = c.Suit switch { "Spades" => "\u2660", "Hearts" => "\u2665", "Diamonds" => "\u2666", "Clubs" => "\u2663", _ => "?" };
        using var sf = new Font("Segoe UI", 10); g.DrawString(sym, sf, sb, r.X + 6, r.Y + 20);
        using var cf = new Font("Segoe UI", 28); g.DrawString(sym, cf, sb, r.X + 18, r.Y + 50); }

    // Game
    private void StartNewGame()
    {
        var deck = new List<global::CardGame.Card>();
        foreach (var s in new[] { "Spades", "Hearts", "Diamonds", "Clubs" })
            foreach (var r in new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" })
                deck.Add(new(s, r, $"{r} of {s}", "", "", r switch { "A" => 1, "J" => 11, "Q" => 12, "K" => 13, _ => int.Parse(r) }));
        _game = new CardGameSolitaire.KlondikeGame(deck);
        _game.StartGame();
        ClearSel();
        _lblState.Text = "遊戲進行中"; _lblState.ForeColor = Color.LightGreen;
        _lblMoves.Text = $"移動次數: {_game.MovesCount}";
        _canvas.Refresh();
    }
    private void CheckWin()
    {
        if (_game == null) return;
        _lblMoves.Text = $"移動次數: {_game.MovesCount}";
        if (_game.CheckWin()) { _lblState.Text = "\u2728 你贏了！ \u2728"; _lblState.ForeColor = Color.Gold; }
    }
}