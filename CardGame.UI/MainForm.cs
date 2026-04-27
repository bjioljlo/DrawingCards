using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CardGame.UI
{
    public class MainForm : Form
    {
        // 遊戲邏輯物件
        private Player? player;
        private Deck? deck;
        private Table? table;
        private Level? level;

        // UI元件
        private FlowLayoutPanel pnlTable = null!; // 桌面區ji3
        private Label lblTitle = null!; // 標題
        private Button btnStartGame = null!; // 開始遊戲按鈕
        private Button btnEndGame = null!; // 結束遊戲按鈕
        private GroupBox grpPlayerInfo = null!; // 玩家資訊區
        private Label lblPlayerName = null!; // 玩家名稱標籤
        private Label lblPlayerScore = null!; // 玩家分數標籤
        private FlowLayoutPanel pnlPlayerHand = null!; // 玩家手牌區
        private PictureBox picDeck = null!; // 牌組圖片
        private PictureBox picDiscard = null!; // 棄牌堆圖片
        private TextBox txtGameMessage = null!; // 遊戲訊息區
        private Button btnDrawCard = null!; // 抽牌按鈕
        private Button btnPlayCard = null!; // 出牌按鈕
        private Button btnEndTurn = null!; // 結束回合按鈕
        private Button btnDiscardCard = null!; // 丟棄手牌按鈕
        private MenuStrip menuStrip = null!; // 菜單欄
        private ToolStripMenuItem menuGame = null!; // 遊戲菜單
        private ToolStripMenuItem menuNewGame = null!; // 新遊戲選項
        private ToolStripMenuItem menuExit = null!; // 結束選項
        private ToolStripMenuItem menuAbout = null!; // 關於選項
        private Button btnCheat = null!; // 作弊按鈕（測試用）

        // 遊戲階段顯示標籤
        private Label lblGamePhase = null!; // 顯示當前遊戲階段

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "Card Game";
            this.Size = new Size(900, 900);
            this.StartPosition = FormStartPosition.CenterScreen;

            // MenuStrip
            menuStrip = new MenuStrip();
            menuGame = new ToolStripMenuItem("遊戲");
            menuNewGame = new ToolStripMenuItem("新遊戲", null, MenuNewGame_Click);
            menuExit = new ToolStripMenuItem("結束", null, MenuExit_Click);
            menuAbout = new ToolStripMenuItem("關於", null, MenuAbout_Click);

            menuGame.DropDownItems.Add(menuNewGame);
            menuGame.DropDownItems.Add(menuExit);
            menuStrip.Items.Add(menuGame);
            menuStrip.Items.Add(menuAbout);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // TableLayoutPanel for main layout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0, 25, 0, 0) // 為MenuStrip預留空間
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.Controls.Add(mainLayout);

            // Left panel: Player info & controls
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.Controls.Add(leftPanel, 0, 0);

            // Right panel: Deck, discard, messages
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainLayout.Controls.Add(rightPanel, 1, 0);

            // Title and Phase Display
            var titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Margin = new Padding(0, 10, 0, 0)
            };
            leftPanel.Controls.Add(titlePanel);

            lblTitle = new Label
            {
                Text = "Card Game",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };
            titlePanel.Controls.Add(lblTitle);

            lblGamePhase = new Label
            {
                Text = "遊戲階段：未開始",
                Font = new Font("Segoe UI", 12),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };
            titlePanel.Controls.Add(lblGamePhase);

            // Start/End Game Buttons
            btnStartGame = new Button
            {
                Text = "開始遊戲",
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 10, 0, 0)
            };
            btnStartGame.Click += BtnStartGame_Click;
            leftPanel.Controls.Add(btnStartGame);

            btnEndGame = new Button
            {
                Text = "結束遊戲",
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 5, 0, 0)
            };
            btnEndGame.Click += BtnEndGame_Click;
            leftPanel.Controls.Add(btnEndGame);

            // Player Info GroupBox
            grpPlayerInfo = new GroupBox
            {
                Text = "玩家資訊",
                Dock = DockStyle.Top,
                Height = 120,
                Margin = new Padding(0, 10, 0, 0)
            };
            leftPanel.Controls.Add(grpPlayerInfo);

            lblPlayerName = new Label
            {
                Text = "玩家名稱：-",
                Dock = DockStyle.Top,
                Height = 25
            };
            grpPlayerInfo.Controls.Add(lblPlayerName);

            lblPlayerScore = new Label
            {
                Text = "分數：0",
                Dock = DockStyle.Top,
                Height = 25
            };
            grpPlayerInfo.Controls.Add(lblPlayerScore);

            // Player Hand Panel
            pnlPlayerHand = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 200, // 放大手牌顯示區，足夠顯示8張
                AutoScroll = true,
                Margin = new Padding(0, 10, 0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };
            leftPanel.Controls.Add(pnlPlayerHand);

            // Game Operation Buttons
            btnDrawCard = new Button
            {
                Text = "抽牌",
                Dock = DockStyle.Top,
                Height = 35,
                Margin = new Padding(0, 10, 0, 0)
            };
            btnDrawCard.Click += BtnDrawCard_Click;
            leftPanel.Controls.Add(btnDrawCard);

            btnPlayCard = new Button
            {
                Text = "出牌",
                Dock = DockStyle.Top,
                Height = 35,
                Margin = new Padding(0, 5, 0, 0)
            };
            btnPlayCard.Click += BtnPlayCard_Click;
            leftPanel.Controls.Add(btnPlayCard);

            btnDiscardCard = new Button
            {
                Text = "丟棄手牌",
                Dock = DockStyle.Top,
                Height = 35,
                Margin = new Padding(0, 5, 0, 0)
            };
            btnDiscardCard.Click += BtnDiscardCard_Click;
            leftPanel.Controls.Add(btnDiscardCard);

            btnEndTurn = new Button
            {
                Text = "結束回合",
                Dock = DockStyle.Top,
                Height = 35,
                Margin = new Padding(0, 5, 0, 0)
            };
            btnEndTurn.Click += BtnEndTurn_Click;
            leftPanel.Controls.Add(btnEndTurn);

            // Cheat Button (Optional)
            btnCheat = new Button
            {
                Text = "作弊（測試用）",
                Dock = DockStyle.Top,
                Height = 35,
                Margin = new Padding(0, 10, 0, 0),
                Visible = true // Set to false if not needed
            };
            btnCheat.Click += BtnCheat_Click;
            leftPanel.Controls.Add(btnCheat);

            // Deck and Discard Area
            var deckArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 20, 0, 0)
            };
            rightPanel.Controls.Add(deckArea);

            picDeck = new PictureBox
            {
                Size = new Size(80, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkGreen,
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };
            picDeck.Click += PicDeck_Click;
            picDeck.Paint += (s, e) =>
            {
                e.Graphics.DrawString("牌堆", new Font("Segoe UI", 12), Brushes.White, new PointF(10, 45));
            };
            deckArea.Controls.Add(picDeck);

            picDiscard = new PictureBox
            {
                Size = new Size(80, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Gray,
                Margin = new Padding(10),
                Cursor = Cursors.Hand // 改為可點擊
            };
            picDiscard.Paint += (s, e) =>
            {
                e.Graphics.DrawString("棄牌堆", new Font("Segoe UI", 12), Brushes.White, new PointF(10, 45));
            };
            picDiscard.Click += PicDiscard_Click;
            deckArea.Controls.Add(picDiscard);

            // Table Area (桌面區)
            pnlTable = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 100,
                AutoScroll = true,
                Margin = new Padding(0, 10, 0, 0),
                BorderStyle = BorderStyle.FixedSingle,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.LightGreen
            };
            // 將桌面區插入 deckArea 之後
            rightPanel.Controls.Add(deckArea);
            rightPanel.Controls.Add(pnlTable);

            // 桌面操作按鈕區
            var pnlTableOps = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 0)
            };
            var btnSwap = new Button
            {
                Text = "換位",
                Width = 60,
                Height = 30,
                Margin = new Padding(5)
            };
            btnSwap.Click += BtnSwap_Click;
            var btnDiscardFromTable = new Button
            {
                Text = "丟棄",
                Width = 60,
                Height = 30,
                Margin = new Padding(5)
            };
            btnDiscardFromTable.Click += BtnDiscardFromTable_Click;
            pnlTableOps.Controls.Add(btnSwap);
            pnlTableOps.Controls.Add(btnDiscardFromTable);
            rightPanel.Controls.Add(pnlTableOps);

            // Game Message Area
            txtGameMessage = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Bottom,
                Height = 100, // 縮小log顯示區
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.WhiteSmoke
            };
            rightPanel.Controls.Add(txtGameMessage);
        }

        // Event Handlers (empty for now)
        private void BtnStartGame_Click(object? sender, EventArgs e)
        {
            // 初始化遊戲物件
            try
            {
                deck = Deck.FromJson("DeckData/Template.json");
                deck.Shuffle(); // 遊戲開始時洗牌
                txtGameMessage.AppendText("載入牌組成功！\r\n");
            }
            catch (FileNotFoundException)
            {
                txtGameMessage.AppendText("找不到牌組檔案，使用空牌組\r\n");
                deck = new Deck(new List<Card>()); // 真正的空牌組，不會嘗試載入檔案
            }
            catch (Exception ex)
            {
                txtGameMessage.AppendText($"牌組載入失敗：{ex.Message}，使用空牌組\r\n");
                deck = new Deck(new List<Card>()); // 真正的空牌組，不會嘗試載入檔案
            }
            
            player = new Player(1, "玩家1");
            table = new Table(8); // 桌面最大8張，可依規則調整
            level = new Level(new List<Player> { player }, deck, null, 100);

            // 註冊階段變更事件
            level.PhaseChanged += OnPhaseChanged;
            level.VictoryConditionMet += OnVictoryConditionMet;

            // 開始遊戲
            level.StartGame();
            
            // 只有當牌組不是空的時候才發五張牌
            if (deck.Count > 0)
            {
                int cardsToDraw = Math.Min(5, deck.Count);
                for (int i = 0; i < cardsToDraw; i++)
                {
                    player.DrawCard(deck);
                }
                txtGameMessage.AppendText($"遊戲開始！已發{cardsToDraw}張牌到您的手牌\r\n");
            }
            else
            {
                txtGameMessage.AppendText("遊戲開始！牌組是空的，無法發牌\r\n");
            }
            
            UpdatePlayerInfo();
            UpdatePlayerHandUI();
            UpdateDiscardPileUI();
            UpdateTableUI();
        }

        private void BtnEndGame_Click(object? sender, EventArgs e)
        {
            txtGameMessage.AppendText("結束遊戲！\r\n");
        }

        // 抽牌按鈕事件處理
        private void BtnDrawCard_Click(object? sender, EventArgs e)
        {
            if (player == null || deck == null || level == null)
            {
                txtGameMessage.AppendText("請先開始遊戲！\r\n");
                return;
            }
            try
            {
                if (level.CurrentPhase == Level.GamePhase.Drawing)
                {
                    player.DrawCard(deck);
                    txtGameMessage.AppendText($"抽到：{player.Hand.Cards[^1].Name}\r\n");
                    UpdatePlayerHandUI();
                    UpdatePlayerInfo();
                    level.NextPhase(); // 抽完牌後進入行動階段
                }
                else
                {
                    txtGameMessage.AppendText("現在不是抽牌階段！\r\n");
                }
            }
            catch (Exception ex)
            {
                txtGameMessage.AppendText($"抽牌失敗：{ex.Message}\r\n");
            }
        }

        private void BtnPlayCard_Click(object? sender, EventArgs e)
        {
            if (player == null || table == null || level == null)
            {
                txtGameMessage.AppendText("請先開始遊戲！\r\n");
                return;
            }
            if (player.Hand.Count == 0)
            {
                txtGameMessage.AppendText("手牌為空，無法出牌！\r\n");
                return;
            }
            if (level.CurrentPhase != Level.GamePhase.Action)
            {
                txtGameMessage.AppendText("現在不是行動階段！\r\n");
                return;
            }
            // 多選出牌
            var selectedCards = new List<Card>();
            foreach (Control ctrl in pnlPlayerHand.Controls)
            {
                if (ctrl is Button btn && btn.BackColor == Color.LightBlue && btn.Tag is Card card)
                {
                    selectedCards.Add(card);
                }
            }
            if (selectedCards.Count == 0)
            {
                txtGameMessage.AppendText("請先選擇要出的牌！\r\n");
                return;
            }
            var played = new List<Card>();
            foreach (var card in selectedCards)
            {
                if (player.PlayCard(card, table))
                    played.Add(card);
            }
            txtGameMessage.AppendText($"出牌至桌面：{string.Join(", ", played.Select(c => c.Name))}\r\n");
            UpdatePlayerHandUI();
            UpdatePlayerInfo();
            UpdateTableUI();
            // 出牌後仍停留在行動階段，玩家可繼續操作或手動結束回合
        }

        private void BtnDiscardCard_Click(object? sender, EventArgs e)
        {
            if (player == null || deck == null)
            {
                txtGameMessage.AppendText("請先開始遊戲！\r\n");
                return;
            }
            if (player.Hand.Count == 0)
            {
                txtGameMessage.AppendText("手牌為空，無法丟棄！\r\n");
                return;
            }
            // 多選丟棄
            var selectedCards = new List<Card>();
            foreach (Control ctrl in pnlPlayerHand.Controls)
            {
                if (ctrl is Button btn && btn.BackColor == Color.LightBlue && btn.Tag is Card card)
                {
                    selectedCards.Add(card);
                }
            }
            if (selectedCards.Count == 0)
            {
                txtGameMessage.AppendText("請先選擇要丟棄的牌！\r\n");
                return;
            }
            var discarded = new List<Card>();
            foreach (var card in selectedCards)
            {
                player.DiscardCard(card); // 丟棄牌到玩家的棄牌堆
                discarded.Add(card);

            }
            txtGameMessage.AppendText($"丟棄至棄牌堆：{string.Join(", ", discarded.Select(c => c.Name))}\r\n");
            UpdatePlayerHandUI();
            UpdatePlayerInfo();
            UpdateDiscardPileUI();
        }

        private void BtnEndTurn_Click(object? sender, EventArgs e)
        {
            if (level == null)
            {
                txtGameMessage.AppendText("請先開始遊戲！\r\n");
                return;
            }
            
            if (level.CurrentPhase != Level.GamePhase.Action)
            {
                txtGameMessage.AppendText("現在不是行動階段，無法結束回合！\r\n");
                return;
            }
            
            txtGameMessage.AppendText("結束回合！\r\n");
            
            // 從行動階段跳到下一回合抽牌階段
            while (level.CurrentPhase != Level.GamePhase.Drawing)
            {
                level.NextPhase();
            }
        }


        private void BtnCheat_Click(object? sender, EventArgs e)
        {
            txtGameMessage.AppendText("作弊！（測試用）\r\n");
        }

        private void PicDeck_Click(object? sender, EventArgs e)
        {
            BtnDrawCard_Click(sender, e);
        }

        private void MenuNewGame_Click(object? sender, EventArgs e)
        {
            txtGameMessage.AppendText("新遊戲開始！\r\n");
        }

        private void MenuExit_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuAbout_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Card Game\nWindows Forms Demo", "關於");
        }
        // 棄牌堆點擊事件，彈出棄牌顯示視窗
        private void PicDiscard_Click(object? sender, EventArgs e)
        {
            if (player == null) return;
            var form = new DiscardPileForm(player.DiscardPile);
            form.ShowDialog(this);
        }

        // UI 更新輔助方法
        private void UpdatePlayerInfo()
        {
            if (player != null)
            {
                lblPlayerName.Text = $"玩家名稱：{player.Name}";
                lblPlayerScore.Text = $"分數：{player.Score}";
            }
        }

        // 階段變更事件處理
        private void OnPhaseChanged(Level.GamePhase phase)
        {
            // 更新階段顯示
            lblGamePhase.Text = $"遊戲階段：{phase}";

            // 根據階段控制按鈕狀態
            switch (phase)
            {
                case Level.GamePhase.NotStarted:
                    EnableButtons(false, false, false, false, false);
                    break;
                case Level.GamePhase.Drawing:
                    EnableButtons(true, false, false, false, false); // 抽牌階段：僅可抽牌
                    break;
                case Level.GamePhase.Action:
                    EnableButtons(true, true, true, true, true); // 行動階段：所有按鈕可用（包含結束回合）
                    break;
                case Level.GamePhase.Scoring:
                    EnableButtons(false, false, false, false, false); // 計分階段：不可手動結束回合
                    break;
                case Level.GamePhase.GameOver:
                    EnableButtons(false);
                    txtGameMessage.AppendText("遊戲結束！\r\n");
                    break;
            }
        }

        // 勝利條件達成事件處理
        private void OnVictoryConditionMet(Level.VictoryCondition condition)
        {
            txtGameMessage.AppendText($"勝利條件達成：{condition}\r\n");
        }

        // 控制按鈕狀態
        private void EnableButtons(bool draw, bool play = false, bool discard = false, bool endTurn = false, bool cheat = false)
        {
            btnDrawCard.Enabled = draw;
            btnPlayCard.Enabled = play;
            btnDiscardCard.Enabled = discard;
            btnEndTurn.Enabled = endTurn;
            btnCheat.Enabled = cheat;
        }

        private void UpdateTableUI()
        {
            pnlTable.Controls.Clear();
            if (table == null) return;
            foreach (var card in table.GetCards())
            {
                var btn = new Button
                {
                    Text = card.Name,
                    Width = 50,
                    Height = 70,
                    Tag = card,
                    BackColor = Color.LightYellow
                };
                btn.Click += (s, e) =>
                {
                    // 切換選取狀態
                    btn.BackColor = btn.BackColor == Color.Orange ? Color.LightYellow : Color.Orange;
                };
                pnlTable.Controls.Add(btn);
            }
        }

        // 桌面操作：換位
        private void BtnSwap_Click(object? sender, EventArgs e)
        {
            if (table == null) return;
            // 取得選取的兩張牌
            var selected = new List<int>();
            for (int i = 0; i < pnlTable.Controls.Count; i++)
            {
                if (pnlTable.Controls[i] is Button btn && btn.BackColor == Color.Orange)
                    selected.Add(i);
            }
            if (selected.Count != 2)
            {
                MessageBox.Show("請選擇桌面上兩張牌進行換位。", "提示");
                return;
            }
            table.SwapCards(selected[0], selected[1]);
            UpdateTableUI();
        }

        // 桌面操作：丟棄
        private void BtnDiscardFromTable_Click(object? sender, EventArgs e)
        {
            if (table == null) return;
            var toRemove = new List<Card>();
            foreach (Control ctrl in pnlTable.Controls)
            {
                if (ctrl is Button btn && btn.BackColor == Color.Orange && btn.Tag is Card card)
                    toRemove.Add(card);
            }
            if (toRemove.Count == 0)
            {
                MessageBox.Show("請選擇要丟棄的桌面牌。", "提示");
                return;
            }
            foreach (var card in toRemove)
            {
                if (table.RemoveCard(card) && player != null)
                {
                    player.AddCardToDiscardPile(card); // 丟棄牌到玩家的棄牌堆
                }
            }

            UpdateTableUI();
        }

        private void UpdatePlayerHandUI()
        {
            pnlPlayerHand.Controls.Clear();
            if (player == null) return;
            foreach (var card in player.Hand.Cards)
            {
                var btn = new Button
                {
                    Text = card.Name,
                    Width = 50, // 縮小按鈕寬度，讓一行可顯示更多張牌
                    Height = 70, // 縮小按鈕高度
                    Tag = card,
                    BackColor = Color.White
                };
                btn.Click += (s, e) =>
                {
                    // 切換選取狀態
                    btn.BackColor = btn.BackColor == Color.LightBlue ? Color.White : Color.LightBlue;
                };
                pnlPlayerHand.Controls.Add(btn);
            }
        }

        private void UpdateDiscardPileUI()
        {
            if (player == null) return;
            picDiscard.Invalidate();
            picDiscard.Paint -= PicDiscard_Paint;
            picDiscard.Paint += PicDiscard_Paint;
        }

        private void PicDiscard_Paint(object? sender, PaintEventArgs e)
        {
            if (player == null) return;
            var g = e.Graphics;
            g.Clear(Color.Gray);
            g.DrawString("棄牌堆", new Font("Segoe UI", 12), Brushes.White, new PointF(10, 45));
            if (player.DiscardPile.Count > 0)
            {
                var lastCard = player.DiscardPile[^1];
                g.DrawString(lastCard.Name, new Font("Segoe UI", 9), Brushes.Yellow, new PointF(10, 70));
            }
        }
    }
}