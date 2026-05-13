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

    [STAThread]
    [Fact]
    public void SelectingWasteCard_ThenClickingTableau_ShouldMoveWithoutNullReference()
    {
        RunSta(() =>
        {
            using var form = new SolitaireForm();
            form.CreateControl();

            var game = new CardGame.Solitaire.KlondikeGame(new List<CardGame.Card>())!;
            var gameField = typeof(SolitaireForm).GetField("_game",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(gameField);

            var waste = new CardGame.Solitaire.WastePile();
            waste.Add(new CardGame.Card("Hearts", "5", "5 of Hearts", "", "", 5));

            var tableau = new CardGame.Solitaire.TableauPile(0);
            var addFaceUpMethod = typeof(CardGame.Solitaire.TableauPile).GetMethod("AddFaceUp",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(addFaceUpMethod);
            addFaceUpMethod!.Invoke(tableau, new object[] { new CardGame.Card("Clubs", "6", "6 of Clubs", "", "", 6) });

            var tableaus = new List<CardGame.Solitaire.TableauPile> { tableau };
            for (int i = 1; i < 7; i++)
                tableaus.Add(new CardGame.Solitaire.TableauPile(i));

            var foundations = new List<CardGame.Solitaire.FoundationPile>();
            for (int i = 0; i < 4; i++)
                foundations.Add(new CardGame.Solitaire.FoundationPile());

            gameField.SetValue(form, game);
            var canvasField = typeof(SolitaireForm).GetField("_canvas", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(canvasField);
            var canvas = canvasField.GetValue(form) as Control;
            Assert.NotNull(canvas);
            var canvasType = canvas!.GetType();

            const BindingFlags props = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            game.GetType().GetProperty("State", props)!
                .SetValue(game, CardGame.Solitaire.KlondikeGame.GameState.Playing);
            game.GetType().GetProperty("Stock", props)!
                .SetValue(game, new CardGame.Solitaire.StockPile(Enumerable.Empty<CardGame.Card>()));
            game.GetType().GetProperty("Waste", props)!
                .SetValue(game, waste);
            game.GetType().GetProperty("Tableaus", props)!
                .SetValue(game, tableaus.AsReadOnly());
            game.GetType().GetProperty("Foundations", props)!
                .SetValue(game, foundations.AsReadOnly());

            var setGameMethod = canvasType.GetMethod("SetGame",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(setGameMethod);
            setGameMethod!.Invoke(canvas, new object[] { game });

            var selectedCardField = canvasType.GetField("<SelectedCard>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            var selectedTableauColField = canvasType.GetField("<SelectedTableauCol>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            var selectedCardIndexField = canvasType.GetField("_selectedCardIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(selectedCardField);
            Assert.NotNull(selectedTableauColField);
            Assert.NotNull(selectedCardIndexField);

            selectedCardField!.SetValue(canvas, waste.TopCard);
            selectedTableauColField!.SetValue(canvas, null);
            selectedCardIndexField!.SetValue(canvas, null);

            canvas.InvokeMouse(new MouseEventArgs(MouseButtons.Left, 1, 55, 180, 0));

            Assert.True(game.Waste.IsEmpty);
            Assert.Equal("5", game.Tableaus[0].TopCard?.Rank);
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