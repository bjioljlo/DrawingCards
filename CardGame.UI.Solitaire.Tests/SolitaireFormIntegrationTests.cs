using System.Reflection;

namespace CardGame.UI.Solitaire.Tests;

public class SolitaireFormIntegrationTests
{
    private static void RunSta(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { exception = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(5));
        if (exception != null) throw exception;
    }

    [STAThread]
    [Fact]
    public void SolitaireForm_Constructor_ShouldNotThrow()
    {
        RunSta(() =>
        {
            using var form = new SolitaireForm();
            form.CreateControl();

            var canvasField = typeof(SolitaireForm).GetField("_canvas",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(canvasField);
            var canvas = canvasField.GetValue(form) as Control;
            Assert.NotNull(canvas);

            var lblMovesField = typeof(SolitaireForm).GetField("_lblMoves",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(lblMovesField);
            var lblMoves = lblMovesField.GetValue(form) as Label;
            Assert.NotNull(lblMoves);

            var lblStateField = typeof(SolitaireForm).GetField("_lblState",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(lblStateField);
            var lblState = lblStateField.GetValue(form) as Label;
            Assert.NotNull(lblState);
            Assert.Contains("開始", lblState.Text);
        });
    }

    [STAThread]
    [Fact]
    public void StartNewGame_ShouldCreateGameAndSetState()
    {
        RunSta(() =>
        {
            using var form = new SolitaireForm();
            form.CreateControl();

            var newGameMethod = typeof(SolitaireForm).GetMethod("NewGame",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(newGameMethod);
            newGameMethod.Invoke(form, null);

            var gameField = typeof(SolitaireForm).GetField("_game",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(gameField);
            var game = gameField.GetValue(form) as CardGame.Solitaire.KlondikeGame;
            Assert.NotNull(game);
            Assert.Equal(CardGame.Solitaire.KlondikeGame.GameState.Playing, game.State);

            Assert.Equal(7, game.Tableaus.Count);
            Assert.Equal(1, game.Tableaus[0].Cards.Count);
            Assert.Equal(1, game.Tableaus[0].FaceUpCount);
            Assert.Equal(7, game.Tableaus[6].Cards.Count);
            Assert.Equal(1, game.Tableaus[6].FaceUpCount);
            Assert.Equal(24, game.Stock.RemainingCards);
            foreach (var f in game.Foundations)
                Assert.True(f.IsEmpty);

            var lblStateField = typeof(SolitaireForm).GetField("_lblState",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(lblStateField);
            var lblState = lblStateField.GetValue(form) as Label;
            Assert.NotNull(lblState);
            Assert.Equal("遊戲進行中", lblState.Text);

            var lblMovesField = typeof(SolitaireForm).GetField("_lblMoves",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(lblMovesField);
            var lblMoves = lblMovesField.GetValue(form) as Label;
            Assert.NotNull(lblMoves);
            Assert.Equal("移動次數: 0", lblMoves.Text);
        });
    }

    [STAThread]
    [Fact]
    public void DrawFromStock_ShouldUpdateUI()
    {
        RunSta(() =>
        {
            using var form = new SolitaireForm();
            form.CreateControl();

            var newGameMethod = typeof(SolitaireForm).GetMethod("NewGame",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(newGameMethod);
            newGameMethod.Invoke(form, null);

            var gameField = typeof(SolitaireForm).GetField("_game",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(gameField);
            var game = gameField.GetValue(form) as CardGame.Solitaire.KlondikeGame;
            Assert.NotNull(game);

            int stockBefore = game.Stock.RemainingCards;
            Assert.True(stockBefore > 0);

            // Simulate a Stock click via canvas MouseClick
            var canvasField = typeof(SolitaireForm).GetField("_canvas",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(canvasField);
            var canvas = canvasField.GetValue(form) as Control;
            Assert.NotNull(canvas);

            // Click at stock rect center (20 + 35 = 55, 45 + 50 = 95)
            canvas.InvokeMouse(new MouseEventArgs(MouseButtons.Left, 1, 55, 95, 0));

            Assert.Equal(stockBefore - 1, game.Stock.RemainingCards);
            Assert.NotNull(game.Waste.TopCard);
        });
    }
}

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