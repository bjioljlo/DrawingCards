using System.Reflection;

namespace CardGame.UI.Solitaire.Tests;

public class SolitaireFormIntegrationTests
{
    [STAThread]
    [Fact]
    public void SolitaireForm_Constructor_ShouldNotThrow()
    {
        // WinForms requires STA thread — run in a separate apartment
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var form = new SolitaireForm();
                form.CreateControl();

                // Verify key controls exist via reflection
                var topPanelField = typeof(SolitaireForm).GetField("_topPanel",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(topPanelField);
                var topPanel = topPanelField.GetValue(form) as Control;
                Assert.NotNull(topPanel);
                Assert.True(topPanel.Height > 0);

                var tableauPanelField = typeof(SolitaireForm).GetField("_tableauPanel",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(tableauPanelField);
                var tableauPanel = tableauPanelField.GetValue(form) as Control;
                Assert.NotNull(tableauPanel);
                Assert.True(tableauPanel.Height > 0);

                var btnField = typeof(SolitaireForm).GetField("_btnNewGame",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(btnField);
                var btn = btnField.GetValue(form) as Button;
                Assert.NotNull(btn);
                Assert.Equal("新遊戲", btn.Text);

                var lblStateField = typeof(SolitaireForm).GetField("_lblState",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(lblStateField);
                var lblState = lblStateField.GetValue(form) as Label;
                Assert.NotNull(lblState);
                Assert.Contains("開始", lblState.Text);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(5));

        if (exception != null)
            throw exception;
    }

    [STAThread]
    [Fact]
    public void StartNewGame_ShouldCreateGameAndSetState()
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var form = new SolitaireForm();
                form.CreateControl();

                // Call StartNewGame via reflection (it's private)
                var startGameMethod = typeof(SolitaireForm).GetMethod("StartNewGame",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(startGameMethod);

                startGameMethod.Invoke(form, null);

                // Verify the game was created
                var gameField = typeof(SolitaireForm).GetField("_game",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(gameField);
                var game = gameField.GetValue(form) as CardGame.Solitaire.KlondikeGame;
                Assert.NotNull(game);
                Assert.Equal(CardGame.Solitaire.KlondikeGame.GameState.Playing, game.State);

                // Verify tableau has cards
                Assert.Equal(7, game.Tableaus.Count);
                // Column 0 should have 1 card (face-up top)
                Assert.Equal(1, game.Tableaus[0].Cards.Count);
                Assert.Equal(1, game.Tableaus[0].FaceUpCount);
                // Column 6 should have 7 cards (1 face-up, 6 face-down)
                Assert.Equal(7, game.Tableaus[6].Cards.Count);
                Assert.Equal(1, game.Tableaus[6].FaceUpCount);

                // Verify stock has 24 remaining cards (52 - 28 dealt)
                Assert.Equal(24, game.Stock.RemainingCards);

                // Verify foundations are empty
                foreach (var f in game.Foundations)
                    Assert.True(f.IsEmpty);

                // Verify state label changed
                var lblStateField = typeof(SolitaireForm).GetField("_lblState",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(lblStateField);
                var lblState = lblStateField.GetValue(form) as Label;
                Assert.NotNull(lblState);
                Assert.Equal("遊戲進行中", lblState.Text);

                // Verify moves label
                var lblMovesField = typeof(SolitaireForm).GetField("_lblMoves",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(lblMovesField);
                var lblMoves = lblMovesField.GetValue(form) as Label;
                Assert.NotNull(lblMoves);
                Assert.Equal("移動次數: 0", lblMoves.Text);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(5));

        if (exception != null)
            throw exception;
    }

    [STAThread]
    [Fact]
    public void DrawFromStock_ShouldUpdateUI()
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var form = new SolitaireForm();
                form.CreateControl();

                var startGameMethod = typeof(SolitaireForm).GetMethod("StartNewGame",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(startGameMethod);
                startGameMethod.Invoke(form, null);

                var gameField = typeof(SolitaireForm).GetField("_game",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(gameField);
                var game = gameField.GetValue(form) as CardGame.Solitaire.KlondikeGame;
                Assert.NotNull(game);

                int stockBefore = game.Stock.RemainingCards;
                Assert.True(stockBefore > 0);

                // Simulate clicking stock: invoke _topPanel's click handler at _stockRect
                var topPanelField = typeof(SolitaireForm).GetField("_topPanel",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(topPanelField);
                var topPanel = topPanelField.GetValue(form) as Panel;
                Assert.NotNull(topPanel);

                // Get _stockRect to know where to click
                var stockRectField = typeof(SolitaireForm).GetField("_stockRect",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(stockRectField);
                var stockRect = (Rectangle)stockRectField.GetValue(form)!;

                // Click center of stock area
                topPanel.InvokeMouse(new MouseEventArgs(MouseButtons.Left, 1,
                    stockRect.X + stockRect.Width / 2,
                    stockRect.Y + stockRect.Height / 2, 0));

                // Verify cards moved from stock to waste
                Assert.Equal(stockBefore - 1, game.Stock.RemainingCards);
                Assert.NotNull(game.Waste.TopCard);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(5));

        if (exception != null)
            throw exception;
    }
}

/// <summary>
/// Extension to simulate mouse events on controls.
/// </summary>
internal static class ControlExtensions
{
    public static void InvokeMouse(this Control control, MouseEventArgs e)
    {
        var onMouseClick = typeof(Control).GetMethod("OnMouseClick",
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (onMouseClick != null)
            onMouseClick.Invoke(control, [e]);
    }
}