using CardGameSolitaire = CardGame.Solitaire;

namespace CardGame.UI.Solitaire;

public class SolitaireForm : Form
{
    private CardGameSolitaire.KlondikeGame? _game;
    private global::CardGame.Card? _selectedCard;
    private int? _selectedTableauCol;
    private int? _selectedCardIndex;

    // Layout constants
    private const int CardWidth = 70;
    private const int CardHeight = 100;
    private const int CardOverlap = 28;
    private const int ColumnWidth = 80;
    private const int TopPanelHeight = 140;
    private const int FoundationStartX = 280;
    private const int FoundationSpacing = 75;
    private const int StatusBarHeight = 50;

    // UI zones (set on layout)
    private Rectangle[] _tableauColumns = new Rectangle[7];
    private Rectangle[] _foundationRects = new Rectangle[4];
    private Rectangle _stockRect;
    private Rectangle _wasteRect;

    // UI elements
    private Panel _topPanel = null!;
    private Panel _tableauPanel = null!;
    private Button _btnNewGame = null!;
    private Label _lblMoves = null!;
    private Label _lblState = null!;

    public SolitaireForm()
    {
        Text = "Klondike Solitaire";
        MinimumSize = new Size(700, 550);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.DarkGreen;
        DoubleBuffered = true;

        InitializeComponents();
        // Layout foundationRects now so they're ready for Paint
        for (int i = 0; i < 4; i++)
            _foundationRects[i] = new Rectangle(FoundationStartX + i * FoundationSpacing, 40, CardWidth, CardHeight);
        Size = new Size(800, 680);
    }

    private void InitializeComponents()
    {
        // Top panel
        _topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = TopPanelHeight,
            BackColor = Color.ForestGreen
        };
        var topLabel = new Label
        {
            Text = "Klondike Solitaire",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 5),
            Size = new Size(200, 30)
        };
        _topPanel.Controls.Add(topLabel);

        _stockRect = new Rectangle(20, 40, CardWidth, CardHeight);
        _wasteRect = new Rectangle(105, 40, CardWidth, CardHeight);

        _topPanel.Paint += TopPanel_Paint;
        _topPanel.MouseClick += TopPanel_MouseClick;
        Controls.Add(_topPanel);

        // Tableau panel — store as field so we can invalidate it
        _tableauPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.DarkGreen
        };
        _tableauPanel.Paint += TableauPanel_Paint;
        _tableauPanel.MouseClick += TableauPanel_MouseClick;
        Controls.Add(_tableauPanel);

        // Status bar
        var statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = StatusBarHeight,
            BackColor = Color.DimGray
        };
        Controls.Add(statusPanel);

        _btnNewGame = new Button
        {
            Text = "新遊戲",
            Location = new Point(10, 8),
            Size = new Size(100, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.WhiteSmoke
        };
        _btnNewGame.Click += BtnNewGame_Click;
        statusPanel.Controls.Add(_btnNewGame);

        _lblMoves = new Label
        {
            Text = "移動次數: 0",
            Location = new Point(140, 12),
            Size = new Size(120, 30),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        statusPanel.Controls.Add(_lblMoves);

        _lblState = new Label
        {
            Text = "點擊「新遊戲」開始",
            Location = new Point(300, 12),
            Size = new Size(300, 30),
            ForeColor = Color.LightGreen,
            Font = new Font("Segoe UI", 11)
        };
        statusPanel.Controls.Add(_lblState);
    }

    private void ComputeTableauLayout()
    {
        if (_tableauPanel == null) return;
        int baseX = 15;
        int baseY = 10;
        int w = _tableauPanel.ClientSize.Width;
        int colWidth = Math.Min(ColumnWidth, (w - 30) / 7);

        for (int i = 0; i < 7; i++)
            _tableauColumns[i] = new Rectangle(baseX + i * colWidth, baseY, CardWidth, _tableauPanel.ClientSize.Height - baseY - 5);
    }

    // ==================== TOP PANEL ====================

    private void TopPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.ForestGreen);

        // Title
        using var titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
        g.DrawString("Klondike Solitaire", titleFont, Brushes.White, 10, 5);

        // Stock pile
        if (_game == null)
        {
            DrawStockSlot(g, _stockRect, false, 0);
        }
        else if (_game.Stock.IsEmpty && !_game.Waste.IsEmpty)
        {
            DrawStockSlot(g, _stockRect, false, 0);
            g.DrawString("回收", new Font("Segoe UI", 8), Brushes.Black, _stockRect.X + 15, _stockRect.Y + 40);
        }
        else if (_game.Stock.IsEmpty)
        {
            DrawStockSlot(g, _stockRect, false, 0);
            g.DrawString("牌庫", new Font("Segoe UI", 9), Brushes.White, _stockRect.X + 15, _stockRect.Y + 40);
        }
        else
        {
            DrawStockSlot(g, _stockRect, true, _game.Stock.RemainingCards);
        }

        // Waste pile
        if (_game != null && !_game.Waste.IsEmpty)
            DrawCard(g, _game.Waste.TopCard!, _wasteRect, false,
                _selectedCard != null && _selectedTableauCol == null && _selectedCard == _game.Waste.TopCard);
        else
            DrawEmptySlot(g, _wasteRect, "廢牌");

        // Foundation piles
        for (int i = 0; i < 4; i++)
        {
            if (_game != null && i < _game.Foundations.Count && !_game.Foundations[i].IsEmpty)
                DrawCard(g, _game.Foundations[i].TopCard!, _foundationRects[i], false, false);
            else
            {
                DrawEmptySlot(g, _foundationRects[i], "");
                var suitSymbol = i switch { 0 => "\u2660", 1 => "\u2665", 2 => "\u2666", 3 => "\u2663", _ => "?" };
                using var font = new Font("Segoe UI", 16);
                g.DrawString(suitSymbol, font, Brushes.Gray, _foundationRects[i].X + 25, _foundationRects[i].Y + 35);
            }
        }
    }

    private void TopPanel_MouseClick(object? sender, MouseEventArgs e)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing)
            return;

        // Stock click
        if (_stockRect.Contains(e.Location))
        {
            if (_game.Stock.IsEmpty && !_game.Waste.IsEmpty)
                _game.RecycleWaste();
            else
                _game.DrawFromStock();

            _selectedCard = null;
            _selectedTableauCol = null;
            _selectedCardIndex = null;
            InvalidatePanels();
            return;
        }

        // Waste click
        if (_wasteRect.Contains(e.Location) && !_game.Waste.IsEmpty)
        {
            if (_selectedCard != null && _selectedTableauCol == null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_game.Foundations[i].CanPlace(_selectedCard))
                    {
                        _game.MoveFromWasteToFoundation(i);
                        _selectedCard = null;
                        InvalidatePanels();
                        CheckWin();
                        return;
                    }
                }
                _selectedCard = null;
                InvalidatePanels();
                return;
            }

            _selectedCard = _game.Waste.TopCard;
            _selectedTableauCol = null;
            _selectedCardIndex = null;
            InvalidatePanels();
            return;
        }

        // Foundation click
        for (int i = 0; i < 4; i++)
        {
            if (!_foundationRects[i].Contains(e.Location)) continue;

            if (_selectedCard != null && _selectedTableauCol == null)
            {
                if (_game.Foundations[i].CanPlace(_selectedCard))
                {
                    _game.MoveFromWasteToFoundation(i);
                    _selectedCard = null;
                    InvalidatePanels();
                    CheckWin();
                }
                return;
            }

            if (_selectedCard != null && _selectedTableauCol.HasValue)
            {
                if (_game.Foundations[i].CanPlace(_selectedCard))
                {
                    _game.MoveToFoundation(_selectedTableauCol.Value, i);
                    _selectedCard = null;
                    _selectedTableauCol = null;
                    _selectedCardIndex = null;
                    InvalidatePanels();
                    CheckWin();
                }
            }
            return;
        }
    }

    // ==================== TABLEAU PANEL ====================

    private void TableauPanel_Paint(object? sender, PaintEventArgs e)
    {
        if (_game == null)
        {
            // Draw empty slots so the area isn't blank
            ComputeTableauLayout();
            var g = e.Graphics;
            g.Clear(Color.DarkGreen);
            for (int col = 0; col < 7; col++)
                DrawEmptySlot(g, new Rectangle(_tableauColumns[col].X, _tableauColumns[col].Y, CardWidth, CardHeight), "");
            return;
        }

        var g2 = e.Graphics;
        ComputeTableauLayout();
        g2.Clear(Color.DarkGreen);

        for (int col = 0; col < 7; col++)
        {
            var colRect = _tableauColumns[col];
            var pile = _game.Tableaus[col];

            if (pile.IsEmpty)
            {
                DrawEmptySlot(g2, new Rectangle(colRect.X, colRect.Y, CardWidth, CardHeight), "");
                continue;
            }

            int yOffset = colRect.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool isFaceUp = i >= pile.Cards.Count - pile.FaceUpCount;
                var card = pile.Cards[i];
                var cardRect = new Rectangle(colRect.X, yOffset, CardWidth, isFaceUp ? CardHeight : 20);

                if (isFaceUp)
                {
                    bool isSelected = _selectedCard == card && _selectedTableauCol == col;
                    DrawCard(g2, card, cardRect, true, isSelected);
                }
                else
                {
                    DrawFaceDown(g2, cardRect);
                }

                yOffset += isFaceUp ? CardOverlap : 5;
            }
        }
    }

    private void TableauPanel_MouseClick(object? sender, MouseEventArgs e)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing)
            return;

        ComputeTableauLayout();

        // Find which column was clicked
        for (int col = 0; col < 7; col++)
        {
            var colRect = _tableauColumns[col];
            if (e.X < colRect.X || e.X > colRect.X + CardWidth) continue;

            var pile = _game.Tableaus[col];

            // Find which card was clicked based on Y position
            int cardIndex = -1;
            int yOffset = colRect.Y;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool isFaceUp = i >= pile.Cards.Count - pile.FaceUpCount;
                int cardBottom = yOffset + (isFaceUp ? CardHeight : 20);

                if (e.Y >= yOffset && e.Y <= cardBottom)
                {
                    cardIndex = i;
                    break;
                }
                yOffset += isFaceUp ? CardOverlap : 5;
            }

            // Click below all cards in a column = treat as empty column click
            if (cardIndex == -1)
            {
                if (_selectedCard == null) return;

                if (_selectedTableauCol == null)
                {
                    // From waste to tableau
                    if (pile.CanPlace(_selectedCard))
                    {
                        _game.MoveFromWasteToTableau(col);
                        _selectedCard = null;
                        InvalidatePanels();
                        CheckWin();
                    }
                    return;
                }
                else
                {
                    // From another tableau
                    if (pile.CanPlace(_selectedCard))
                    {
                        _game.MoveCard(_selectedTableauCol.Value, col, _selectedCardIndex ?? 0);
                        _selectedCard = null;
                        _selectedTableauCol = null;
                        _selectedCardIndex = null;
                        InvalidatePanels();
                    }
                    return;
                }
            }

            var card = pile.Cards[cardIndex];

            // No card selected yet — select this one
            if (_selectedCard == null)
            {
                var movable = pile.GetMovableCards(cardIndex);
                if (movable != null)
                {
                    _selectedCard = card;
                    _selectedTableauCol = col;
                    _selectedCardIndex = cardIndex;
                    InvalidatePanels();
                }
                return;
            }

            // Same column — try to reorder or deselect
            if (_selectedTableauCol == col)
            {
                if (_selectedCard == card)
                {
                    _selectedCard = null;
                    _selectedTableauCol = null;
                    _selectedCardIndex = null;
                    InvalidatePanels();
                    return;
                }

                if (pile.CanPlace(_selectedCard))
                {
                    _game.MoveCard(col, col, _selectedCardIndex ?? 0);
                    _selectedCard = null;
                    _selectedTableauCol = null;
                    _selectedCardIndex = null;
                    InvalidatePanels();
                    CheckWin();
                }
                return;
            }

            // Different column — move cards
            var targetPile = _game.Tableaus[col];
            if (targetPile.CanPlace(_selectedCard))
            {
                _game.MoveCard(_selectedTableauCol!.Value, col, _selectedCardIndex ?? 0);
                _selectedCard = null;
                _selectedTableauCol = null;
                _selectedCardIndex = null;
                InvalidatePanels();
                CheckWin();
            }
            else
            {
                _selectedCard = null;
                _selectedTableauCol = null;
                _selectedCardIndex = null;
                InvalidatePanels();
            }
            return;
        }
    }

    // ==================== DRAWING HELPERS ====================

    private static void DrawEmptySlot(Graphics g, Rectangle rect, string label)
    {
        using var pen = new Pen(Color.FromArgb(0, 100, 0), 2);
        g.DrawRectangle(pen, rect);
        if (!string.IsNullOrEmpty(label))
        {
            using var font = new Font("Segoe UI", 8);
            g.DrawString(label, font, Brushes.DarkGray, rect.X + 15, rect.Y + 40);
        }
    }

    private static void DrawStockSlot(Graphics g, Rectangle rect, bool hasCards, int count)
    {
        using var pen = new Pen(Color.Black, 1);
        if (hasCards)
        {
            g.FillRectangle(Brushes.DarkBlue, rect);
            using var font = new Font("Segoe UI", 14, FontStyle.Bold);
            g.DrawString(count.ToString(), font, Brushes.White, rect.X + 25, rect.Y + 35);
        }
        else
        {
            g.FillRectangle(Brushes.DarkBlue, rect);
        }
        g.DrawRectangle(pen, rect);
    }

    private static void DrawFaceDown(Graphics g, Rectangle rect)
    {
        g.FillRectangle(Brushes.DarkBlue, rect);
        using var borderPen = new Pen(Color.Black, 1);
        g.DrawRectangle(borderPen, rect);

        // Pattern: cross-hatch
        using var patternPen = new Pen(Color.Navy, 1);
        g.DrawLine(patternPen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
        g.DrawLine(patternPen, rect.Right - 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
    }

    private static void DrawCard(Graphics g, global::CardGame.Card card, Rectangle rect, bool showFull, bool selected)
    {
        var fillColor = selected ? Color.LightYellow : Color.White;
        g.FillRectangle(new SolidBrush(fillColor), rect);

        var isRed = card.Suit == "Hearts" || card.Suit == "Diamonds";
        using var suitBrush = new SolidBrush(isRed ? Color.Red : Color.Black);
        using var borderPen = new Pen(selected ? Color.Blue : Color.Black, selected ? 3 : 1);
        g.DrawRectangle(borderPen, rect);

        if (!showFull) return;

        using var rankFont = new Font("Segoe UI", 11, FontStyle.Bold);
        g.DrawString(card.Rank, rankFont, suitBrush, rect.X + 4, rect.Y + 2);

        var symbol = card.Suit switch
        {
            "Spades" => "\u2660",
            "Hearts" => "\u2665",
            "Diamonds" => "\u2666",
            "Clubs" => "\u2663",
            _ => "?"
        };
        using var symbolFont = new Font("Segoe UI", 10);
        g.DrawString(symbol, symbolFont, suitBrush, rect.X + 6, rect.Y + 20);

        using var centerFont = new Font("Segoe UI", 28);
        g.DrawString(symbol, centerFont, suitBrush, rect.X + 18, rect.Y + 50);
    }

    // ==================== GAME ====================

    private void InvalidatePanels()
    {
        _topPanel.Refresh();
        _tableauPanel.Refresh();
        _lblMoves.Text = _game != null ? $"移動次數: {_game.MovesCount}" : "移動次數: 0";
    }

    private void BtnNewGame_Click(object? sender, EventArgs e)
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        var deck = CreateStandardDeck();
        _game = new CardGameSolitaire.KlondikeGame(deck);
        _game.StartGame();
        _selectedCard = null;
        _selectedTableauCol = null;
        _selectedCardIndex = null;
        _lblState.Text = "遊戲進行中";
        _lblState.ForeColor = Color.LightGreen;
        InvalidatePanels();
    }

    private static List<global::CardGame.Card> CreateStandardDeck()
    {
        var suits = new[] { "Spades", "Hearts", "Diamonds", "Clubs" };
        var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        var cards = new List<global::CardGame.Card>();
        foreach (var suit in suits)
        foreach (var rank in ranks)
        {
            var score = rank switch
            {
                "A" => 1, "J" => 11, "Q" => 12, "K" => 13,
                _ => int.Parse(rank)
            };
            cards.Add(new global::CardGame.Card(suit, rank, $"{rank} of {suit}", "", "", score));
        }
        return cards;
    }

    private void CheckWin()
    {
        if (_game == null) return;
        if (_game.CheckWin())
        {
            _lblState.Text = "\u2728 你贏了！ \u2728";
            _lblState.ForeColor = Color.Gold;
        }
    }
}