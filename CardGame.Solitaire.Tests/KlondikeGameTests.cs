namespace CardGame.Solitaire.Tests;

public class KlondikeGameTests
{
    private static Card MakeCard(string suit, string rank)
        => new Card(suit, rank, $"{rank} of {suit}", "", "", SuitColor.RankToNumber(rank));

    private static List<Card> CreateFullDeck()
    {
        var suits = new[] { "Spades", "Hearts", "Diamonds", "Clubs" };
        var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        var cards = new List<Card>();
        foreach (var suit in suits)
            foreach (var rank in ranks)
                cards.Add(MakeCard(suit, rank));
        return cards;
    }

    [Fact]
    public void StartGame_ShouldDeal7Tableaus()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        Assert.Equal(7, game.Tableaus.Count);
        Assert.Equal(4, game.Foundations.Count);
        for (int i = 0; i < 7; i++)
            Assert.Equal(i + 1, game.Tableaus[i].Cards.Count);
    }

    [Fact]
    public void StartGame_ShouldSetStateToPlaying()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        Assert.Equal(KlondikeGame.GameState.Playing, game.State);
    }

    [Fact]
    public void DrawFromStock_ShouldMoveCardToWaste()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        var stockCountBefore = game.Stock.RemainingCards;
        game.DrawFromStock();
        Assert.Equal(stockCountBefore - 1, game.Stock.RemainingCards);
        Assert.NotNull(game.Waste.TopCard);
    }

    [Fact]
    public void DrawFromStock_WhenStockEmpty_ShouldNotThrow()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        // Draw until stock is empty
        while (!game.Stock.IsEmpty)
            game.DrawFromStock();
        // Should not throw
        game.DrawFromStock();
        // Waste has cards from the previous draws, calling Draw again does nothing
        Assert.False(game.Waste.IsEmpty);
    }

    [Fact]
    public void MoveFromWasteToTableau_ShouldMoveCard()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        game.DrawFromStock();
        var wasteCard = game.Waste.TopCard!;

        for (int i = 0; i < 7; i++)
        {
            if (game.Tableaus[i].CanPlace(wasteCard))
            {
                var result = game.MoveFromWasteToTableau(i);
                Assert.True(result);
                Assert.True(game.Waste.IsEmpty || game.Waste.TopCard!.Rank != wasteCard.Rank);
                return;
            }
        }
    }

    [Fact]
    public void MoveFromWasteToFoundation_WithAce_ShouldSucceed()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();

        for (int i = 0; i < 30; i++)
        {
            game.DrawFromStock();
            if (game.Waste.TopCard?.Rank == "A")
            {
                for (int j = 0; j < 4; j++)
                {
                    if (game.Foundations[j].CanPlace(game.Waste.TopCard))
                    {
                        var result = game.MoveFromWasteToFoundation(j);
                        Assert.True(result);
                        return;
                    }
                }
            }
        }
    }

    [Fact]
    public void CheckWin_ShouldReturnFalse_WhenGameStarts()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        Assert.False(game.CheckWin());
    }

    [Fact]
    public void RecycleWaste_ShouldMoveAllCardsBackToStock()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        game.DrawFromStock();
        game.DrawFromStock();
        game.DrawFromStock();
        Assert.True(game.Waste.Cards.Count > 0);
        game.RecycleWaste();
        Assert.True(game.Waste.IsEmpty);
    }

    [Fact]
    public void RecycleWaste_ShouldPreserveCardOrder()
    {
        // 測試：回收後出牌順序應保持一致
        var deck = new List<Card>();
        // 至少需要 28 張用於 tableau 配置，再加至少 3 張用於測試
        for (int i = 0; i < 35; i++)
        {
            int suitIndex = i / 13;
            int rankIndex = i % 13;
            var suits = new[] { "Spades", "Hearts", "Diamonds", "Clubs" };
            var ranks = new[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            deck.Add(MakeCard(suits[suitIndex % 4], ranks[rankIndex]));
        }

        var game = new KlondikeGame(deck);
        game.StartGame();

        // 抽出前 3 張牌，記錄順序
        var firstRound = new List<string>();
        game.DrawFromStock();
        firstRound.Add(game.Waste.TopCard!.Rank);
        game.DrawFromStock();
        firstRound.Add(game.Waste.TopCard!.Rank);
        game.DrawFromStock();
        firstRound.Add(game.Waste.TopCard!.Rank);

        // 回收
        game.RecycleWaste();

        // 再抽相同數量的牌
        var secondRound = new List<string>();
        game.DrawFromStock();
        secondRound.Add(game.Waste.TopCard!.Rank);
        game.DrawFromStock();
        secondRound.Add(game.Waste.TopCard!.Rank);
        game.DrawFromStock();
        secondRound.Add(game.Waste.TopCard!.Rank);

        // 驗證：第二輪應該與第一輪順序相同
        Assert.Equal(firstRound, secondRound);
    }

    [Fact]
    public void Restart_ShouldResetGame()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();
        game.DrawFromStock();
        game.Restart();
        Assert.Equal(KlondikeGame.GameState.NotStarted, game.State);
        game.StartGame();
        Assert.Equal(KlondikeGame.GameState.Playing, game.State);
    }

    [Fact]
    public void MoveCard_FromTableauToTableau_ShouldWork()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();

        for (int from = 0; from < 7; from++)
        {
            var fromPile = game.Tableaus[from];
            if (fromPile.IsEmpty) continue;

            for (int to = 0; to < 7; to++)
            {
                if (from == to) continue;
                var topCard = fromPile.TopCard!;
                if (game.Tableaus[to].CanPlace(topCard))
                {
                    var result = game.MoveCard(from, to, fromPile.Cards.Count - 1);
                    Assert.True(result);
                    return;
                }
            }
        }
    }

    [Fact]
    public void MoveToFoundation_WithValidAce_ShouldSucceed()
    {
        var game = new KlondikeGame(CreateFullDeck());
        game.StartGame();

        for (int i = 0; i < 7; i++)
        {
            var top = game.Tableaus[i].TopCard;
            if (top != null && top.Rank == "A")
            {
                for (int j = 0; j < 4; j++)
                {
                    if (game.Foundations[j].CanPlace(top))
                    {
                        var result = game.MoveToFoundation(i, j);
                        Assert.True(result);
                        Assert.Equal(1, game.Foundations[j].Cards.Count);
                        return;
                    }
                }
            }
        }
    }
}