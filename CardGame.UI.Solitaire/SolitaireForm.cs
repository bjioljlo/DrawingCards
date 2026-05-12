using CardGameSolitaire = CardGame.Solitaire;

namespace CardGame.UI.Solitaire;

public class SolitaireForm : Form
{
    private CardGameSolitaire.KlondikeGame? _game;
    private global::CardGame.Card? _selectedCard;
    private int? _selectedTableauIndex;
    private int? _selectedCardIndex;

    // Layout constants
    private const int CardWidth = 70;
    private const int CardHeight = 100;
    private const int CardOverlap = 25;
    private const int ColumnSpacing = 85;
    private const int TopPanelHeight = 130;
    private const int FoundationSpacing = 75;
    private const int StatusBarHeight = 50;

    // UI elements
    private Panel _topPanel = null!;
    private Panel _tableauPanel = null!;
    private Panel _statusPanel = null!;
    private Button _btnNewGame = null!;
    private Label _lblMoves = null!;
    private Label _lblState = null!;
    private PictureBox _picStock = null!;
    private PictureBox _picWaste = null!;
    private readonly PictureBox[] _foundationBoxes = new PictureBox[4];

    public SolitaireForm()
    {
        Text = "Klondike Solitaire";
        Size = new Size(800, 680);
        MinimumSize = new Size(700, 550);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.DarkGreen;
        DoubleBuffered = true;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Top panel: Stock, Waste, Foundations
        _topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = TopPanelHeight,
            BackColor = Color.ForestGreen
        };
        Controls.Add(_topPanel);

        // Stock pile
        _picStock = new PictureBox
        {
            Size = new Size(CardWidth, CardHeight),
            Location = new Point(20, 15),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.DarkBlue,
            Cursor = Cursors.Hand,
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        _picStock.Paint += PicStock_Paint;
        _picStock.Click += PicStock_Click;
        _topPanel.Controls.Add(_picStock);

        // Waste pile
        _picWaste = new PictureBox
        {
            Size = new Size(CardWidth, CardHeight),
            Location = new Point(100, 15),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.Gray,
            Cursor = Cursors.Hand,
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        _picWaste.Paint += PicWaste_Paint;
        _picWaste.Click += PicWaste_Click;
        _topPanel.Controls.Add(_picWaste);

        // Foundation piles (4)
        int foundationStartX = 300;
        for (int i = 0; i < 4; i++)
        {
            _foundationBoxes[i] = new PictureBox
            {
                Size = new Size(CardWidth, CardHeight),
                Location = new Point(foundationStartX + i * FoundationSpacing, 15),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkSlateGray,
                Cursor = Cursors.Hand,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            int fi = i;
            _foundationBoxes[i].Paint += (s, e) => PicFoundation_Paint(s, e, fi);
            _foundationBoxes[i].Click += (s, e) => PicFoundation_Click(fi);
            _topPanel.Controls.Add(_foundationBoxes[i]);
        }

        // Tableau panel (7 columns)
        _tableauPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.DarkGreen,
            Padding = new Padding(10, 5, 10, StatusBarHeight + 5)
        };
        Controls.Add(_tableauPanel);

        // Create tableau column panels
        for (int col = 0; col < 7; col++)
        {
            var colPanel = new Panel
            {
                Location = new Point(10 + col * (CardWidth + 15), 5),
                Size = new Size(CardWidth + 10, _tableauPanel.Height - 20),
                BackColor = Color.Transparent
            };
            colPanel.Resize += (s, e) => UpdateTableauLayout();
            _tableauPanel.Controls.Add(colPanel);
        }

        // Status bar at bottom
        _statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = StatusBarHeight,
            BackColor = Color.DimGray
        };
        Controls.Add(_statusPanel);

        _btnNewGame = new Button
        {
            Text = "新遊戲",
            Location = new Point(10, 8),
            Size = new Size(100, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.WhiteSmoke
        };
        _btnNewGame.Click += BtnNewGame_Click;
        _statusPanel.Controls.Add(_btnNewGame);

        _lblMoves = new Label
        {
            Text = "移動次數: 0",
            Location = new Point(140, 12),
            Size = new Size(120, 30),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        _statusPanel.Controls.Add(_lblMoves);

        _lblState = new Label
        {
            Text = "點擊「新遊戲」開始",
            Location = new Point(300, 12),
            Size = new Size(250, 30),
            ForeColor = Color.LightGreen,
            Font = new Font("Segoe UI", 11)
        };
        _statusPanel.Controls.Add(_lblState);
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
        _selectedTableauIndex = null;
        _selectedCardIndex = null;
        UpdateAllUI();
        _lblState.Text = "遊戲進行中";
        _lblState.ForeColor = Color.LightGreen;
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

    // ---- Stock ----
    private void PicStock_Click(object? sender, EventArgs e)
    {
    if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing)
            return;

        if (_game.Stock.IsEmpty && !_game.Waste.IsEmpty)
        {
            _game.RecycleWaste();
            UpdateAllUI();
            return;
        }

        _game.DrawFromStock();
        _selectedCard = null;
        _selectedTableauIndex = null;
        _selectedCardIndex = null;
        UpdateAllUI();
    }

    private void PicStock_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        if (_game == null || _game.Stock.IsEmpty)
        {
            if (_game?.Waste.IsEmpty == false)
            {
                g.Clear(Color.Goldenrod);
                using var font = new Font("Segoe UI", 8);
                g.DrawString("回收", font, Brushes.Black, 15, 40);
            }
            else
            {
                g.Clear(Color.DarkBlue);
                g.DrawString("牌庫", new Font("Segoe UI", 9), Brushes.White, 15, 40);
            }
        }
        else
        {
            g.Clear(Color.DarkBlue);
            using var font = new Font("Segoe UI", 14, FontStyle.Bold);
            var count = _game.Stock.RemainingCards;
            g.DrawString(count.ToString(), font, Brushes.White, 25, 35);
        }
    }

    // ---- Waste ----
    private void PicWaste_Click(object? sender, EventArgs e)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing || _game.Waste.IsEmpty)
            return;

        if (_selectedCard != null && _selectedTableauIndex == null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_game.Foundations[i].CanPlace(_selectedCard))
                {
                    _game.MoveFromWasteToFoundation(i);
                    _selectedCard = null;
                    UpdateAllUI();
                    CheckWin();
                    return;
                }
            }
            _selectedCard = null;
            UpdateAllUI();
            return;
        }

        _selectedCard = _game.Waste.TopCard;
        _selectedTableauIndex = null;
        _selectedCardIndex = null;
        UpdateAllUI();
    }

    private void PicWaste_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        if (_game == null || _game.Waste.IsEmpty)
        {
            g.Clear(Color.Gray);
            g.DrawString("廢牌", new Font("Segoe UI", 9), Brushes.White, 15, 40);
        }
        else
        {
            DrawCard(g, _game.Waste.TopCard!, false, _selectedCard == _game.Waste.TopCard);
        }
    }

    // ---- Foundations ----
    private void PicFoundation_Paint(object? sender, PaintEventArgs e, int index)
    {
        var g = e.Graphics;
        if (_game == null || index >= _game.Foundations.Count)
        {
            g.Clear(Color.DarkSlateGray);
            return;
        }

        var foundation = _game.Foundations[index];
        if (foundation.IsEmpty)
        {
            g.Clear(Color.DarkSlateGray);
            var suitSymbol = index switch
            {
                0 => "\u2660",
                1 => "\u2665",
                2 => "\u2666",
                3 => "\u2663",
                _ => "?"
            };
            using var font = new Font("Segoe UI", 16);
            g.DrawString(suitSymbol, font, Brushes.Gray, 25, 35);
        }
        else
        {
            DrawCard(g, foundation.TopCard!, false, false);
        }
    }

    private void PicFoundation_Click(int index)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing)
            return;

        if (_selectedCard != null && _selectedTableauIndex == null)
        {
            if (_game.Foundations[index].CanPlace(_selectedCard))
            {
                _game.MoveFromWasteToFoundation(index);
                _selectedCard = null;
                UpdateAllUI();
                CheckWin();
            }
            return;
        }

        if (_selectedCard != null && _selectedTableauIndex.HasValue)
        {
            if (_game.Foundations[index].CanPlace(_selectedCard))
            {
                _game.MoveToFoundation(_selectedTableauIndex.Value, index);
                _selectedCard = null;
                _selectedTableauIndex = null;
                _selectedCardIndex = null;
                UpdateAllUI();
                CheckWin();
            }
        }
    }

    // ---- Tableau ----
    private void UpdateTableauUI()
    {
        for (int col = 0; col < 7; col++)
        {
            var panel = _tableauPanel.Controls[col] as Panel;
            if (panel == null) continue;

            panel.Controls.Clear();
            panel.Size = new Size(CardWidth + 10, _tableauPanel.Height - 20);

            if (_game == null || col >= _game.Tableaus.Count)
                continue;

            var pile = _game.Tableaus[col];
            if (pile.IsEmpty)
            {
                var emptyBox = new PictureBox
                {
                    Size = new Size(CardWidth, CardHeight),
                    Location = new Point(5, 5),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(0, 80, 0),
                    Cursor = Cursors.Hand
                };
                int ci = col;
                emptyBox.Click += (s, e) => TableauClicked(ci, -1);
                panel.Controls.Add(emptyBox);
                continue;
            }

            int yOffset = 5;
            for (int i = 0; i < pile.Cards.Count; i++)
            {
                bool isFaceUp = i >= pile.Cards.Count - pile.FaceUpCount;
                var card = pile.Cards[i];

                var cardBox = new PictureBox
                {
                    Size = new Size(CardWidth, CardHeight),
                    Location = new Point(5, yOffset),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = isFaceUp ? Color.White : Color.DarkBlue,
                    Cursor = Cursors.Hand,
                    Tag = (col, i)
                };

                int ci = col, ii = i;
                cardBox.Paint += (s, e) =>
                {
                    if (isFaceUp)
                        DrawCard(e.Graphics, card, true, _selectedCard == card && _selectedTableauIndex == ci);
                    else
                    {
                        e.Graphics.Clear(Color.DarkBlue);
                        using var font = new Font("Segoe UI", 7);
                        e.Graphics.DrawString("DC", font, Brushes.White, 25, 40);
                    }
                };
                cardBox.Click += (s, e) => TableauClicked(ci, ii);

                panel.Controls.Add(cardBox);
                yOffset += isFaceUp ? CardOverlap : 5;
            }
        }
    }

    private void TableauClicked(int colIndex, int cardIndex)
    {
        if (_game == null || _game.State != CardGameSolitaire.KlondikeGame.GameState.Playing)
            return;

        var pile = _game.Tableaus[colIndex];

        if (cardIndex == -1)
        {
            if (_selectedCard != null && _selectedTableauIndex == null)
            {
                if (pile.CanPlace(_selectedCard))
                {
                    _game.MoveFromWasteToTableau(colIndex);
                    _selectedCard = null;
                    UpdateAllUI();
                    CheckWin();
                }
                return;
            }

            if (_selectedCard != null && _selectedTableauIndex.HasValue)
            {
                if (pile.CanPlace(_selectedCard))
                {
                    _game.MoveCard(_selectedTableauIndex.Value, colIndex, _selectedCardIndex ?? 0);
                    _selectedCard = null;
                    _selectedTableauIndex = null;
                    _selectedCardIndex = null;
                    UpdateAllUI();
                }
                return;
            }
            return;
        }

        var card = pile.Cards[cardIndex];

        if (_selectedCard == null)
        {
            var movable = pile.GetMovableCards(cardIndex);
            if (movable != null)
            {
                _selectedCard = card;
                _selectedTableauIndex = colIndex;
                _selectedCardIndex = cardIndex;
                UpdateAllUI();
            }
            return;
        }

        if (_selectedTableauIndex == colIndex)
        {
            if (_selectedCard == card)
            {
                _selectedCard = null;
                _selectedTableauIndex = null;
                _selectedCardIndex = null;
                UpdateAllUI();
                return;
            }

            if (pile.CanPlace(_selectedCard))
            {
                _game.MoveCard(_selectedTableauIndex.Value, colIndex, _selectedCardIndex ?? 0);
                _selectedCard = null;
                _selectedTableauIndex = null;
                _selectedCardIndex = null;
                UpdateAllUI();
                CheckWin();
            }
            return;
        }

        var targetPile = _game.Tableaus[colIndex];
        if (targetPile.CanPlace(_selectedCard))
        {
            _game.MoveCard(_selectedTableauIndex!.Value, colIndex, _selectedCardIndex ?? 0);
            _selectedCard = null;
            _selectedTableauIndex = null;
            _selectedCardIndex = null;
            UpdateAllUI();
            CheckWin();
        }
        else
        {
            _selectedCard = null;
            _selectedTableauIndex = null;
            _selectedCardIndex = null;
            UpdateAllUI();
        }
    }

    // ---- Card Drawing ----
    private static void DrawCard(Graphics g, global::CardGame.Card card, bool showFull, bool selected)
    {
        g.Clear(selected ? Color.LightYellow : Color.White);

        var isRed = card.Suit == "Hearts" || card.Suit == "Diamonds";
        using var suitBrush = new SolidBrush(isRed ? Color.Red : Color.Black);

        using var rankFont = new Font("Segoe UI", 11, FontStyle.Bold);
        g.DrawString(card.Rank, rankFont, suitBrush, 4, 2);

        var symbol = card.Suit switch
        {
            "Spades" => "\u2660",
            "Hearts" => "\u2665",
            "Diamonds" => "\u2666",
            "Clubs" => "\u2663",
            _ => "?"
        };
        using var symbolFont = new Font("Segoe UI", 10);
        g.DrawString(symbol, symbolFont, suitBrush, 6, 20);

        using var centerFont = new Font("Segoe UI", 28);
        g.DrawString(symbol, centerFont, suitBrush, 18, 50);

        if (selected)
        {
            using var pen = new Pen(Color.Blue, 3);
            g.DrawRectangle(pen, 1, 1, CardWidth - 3, CardHeight - 3);
        }

        using var borderPen = new Pen(Color.Black, 1);
        g.DrawRectangle(borderPen, 0, 0, CardWidth - 1, CardHeight - 1);
    }

    // ---- Update ----
    private void UpdateAllUI()
    {
        if (_game == null) return;

        _picStock.Invalidate();
        _picWaste.Invalidate();

        for (int i = 0; i < 4; i++)
            _foundationBoxes[i].Invalidate();

        UpdateTableauUI();
        _lblMoves.Text = $"移動次數: {_game.MovesCount}";

        if (_game.State == CardGameSolitaire.KlondikeGame.GameState.Won)
        {
            _lblState.Text = "\u2728 你贏了！ \u2728";
            _lblState.ForeColor = Color.Gold;
        }
    }

    private void UpdateTableauLayout()
    {
        UpdateTableauUI();
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

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateTableauLayout();
    }
}