using System;
using System.Linq;
using Xunit;

namespace CardGame.Tests
{
    public class LevelTests
    {
        private readonly Level _level;
        private readonly Player _player;
        private readonly Deck _deck;
        private readonly Card _targetCard;

        public LevelTests()
        {
            _player = new Player(1, "Test Player");
            var testCards = new List<Card>
            {
                new Card("Test Suit", "Test Rank", "Test Card 1"),
                new Card("Test Suit", "Test Rank", "Test Card 2"),
                new Card("Test Suit", "Test Rank", "Test Card 3")
            };
            _deck = new Deck(testCards);
            _targetCard = new Card("Test Suit", "Test Rank", "Test Target Card");
            _level = new Level(new List<Player> { _player }, _deck, _targetCard, 100);
        }

        [Fact]
        public void StartGame_ShouldSetCorrectInitialPhase()
        {
            // Act
            _level.StartGame();

            // Assert
            Assert.Equal(Level.GamePhase.Drawing, _level.CurrentPhase);
            Assert.Equal(1, _level.TurnCount);
            Assert.True(_level.IsGameStarted);
        }

        [Fact]
        public void NextPhase_ShouldProgressThroughPhasesCorrectly()
        {
            // Arrange
            _level.StartGame();

            // Act & Assert
            _level.NextPhase();
            Assert.Equal(Level.GamePhase.Action, _level.CurrentPhase);

            _level.NextPhase();
            Assert.Equal(Level.GamePhase.Scoring, _level.CurrentPhase);

            _level.NextPhase();
            Assert.Equal(Level.GamePhase.Drawing, _level.CurrentPhase);
            Assert.Equal(2, _level.TurnCount);
        }

        [Fact]
        public void EndGame_ShouldSetGameOverPhase()
        {
            // Arrange
            _level.StartGame();

            // Act
            _level.EndGame();

            // Assert
            Assert.Equal(Level.GamePhase.GameOver, _level.CurrentPhase);
            Assert.True(_level.IsGameOver);
        }

        [Fact]
        public void Reset_ShouldReturnToInitialState()
        {
            // Arrange
            _level.StartGame();
            _level.NextPhase();

            // Act
            _level.Reset();

            // Assert
            Assert.Equal(Level.GamePhase.NotStarted, _level.CurrentPhase);
            Assert.Equal(0, _level.TurnCount);
            Assert.False(_level.IsGameStarted);
        }

        [Fact]
        public void PhaseChanged_ShouldRaiseEvent()
        {
            // Arrange
            var eventRaised = false;
            Level.GamePhase? newPhase = null;
            _level.PhaseChanged += (phase) => { eventRaised = true; newPhase = phase; };

            // Act
            _level.StartGame();

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(Level.GamePhase.Drawing, newPhase);
        }

        [Fact]
        public void VictoryConditionMet_ShouldRaiseEvent()
        {
            // Arrange
            var eventRaised = false;
            Level.VictoryCondition? metCondition = null;
            _level.VictoryConditionMet += (condition) => { eventRaised = true; metCondition = condition; };
            _level.StartGame();

            // Act
            _level.SetVictoryCondition(Level.VictoryCondition.TargetScore);
            _player.Score = 100; // 設定玩家分數達到目標
            _level.NextPhase(); // 觸發勝利條件檢查

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(Level.VictoryCondition.TargetScore, metCondition);
        }

        [Fact]
        public void GetPhaseDuration_ShouldReturnReasonableTime()
        {
            // Arrange
            _level.StartGame();

            // Act
            var duration = _level.GetPhaseDuration();

            // Assert
            Assert.True(duration.TotalMilliseconds >= 0);
            Assert.True(duration.TotalSeconds <= 1); // 合理的上限
        }

        [Fact]
        public void NextPhase_ShouldThrowWhenNotStarted()
        {
            // Assert
            Assert.Throws<InvalidOperationException>(() => _level.NextPhase());
        }

        [Fact]
        public void NextPhase_ShouldThrowWhenGameOver()
        {
            // Arrange
            _level.StartGame();
            _level.EndGame();

            // Assert
            Assert.Throws<InvalidOperationException>(() => _level.NextPhase());
        }

        [Fact]
        public void StartGame_ShouldThrowWhenAlreadyStarted()
        {
            // Arrange
            _level.StartGame();

            // Assert
            Assert.Throws<InvalidOperationException>(() => _level.StartGame());
        }

        [Fact]
        public void PhaseChangedEvent_ShouldBeCalled()
        {
            // Arrange
            var phaseChangedCalled = false;
            _level.PhaseChanged += (phase) => phaseChangedCalled = true;

            // Act
            _level.StartGame();

            // Assert
            Assert.True(phaseChangedCalled);
        }

        [Fact]
        public void CheckTargetScoreCondition_ShouldReturnTrueWhenTargetScoreReached()
        {
            // Arrange
            _level.StartGame();
            _level.SetVictoryCondition(Level.VictoryCondition.TargetScore);
            _player.Score = 100;

            // Act
            _level.NextPhase();

            // Assert
            Assert.Equal(Level.GamePhase.GameOver, _level.CurrentPhase);
        }

        [Fact]
        public void CheckNoCardsLeftCondition_ShouldReturnTrueWhenNoCardsLeft()
        {
            // Arrange
            _level.StartGame();
            _level.SetVictoryCondition(Level.VictoryCondition.NoCardsLeft);
            _deck.ClearDeck(); // 測試用：清空牌組

            // Act
            _level.NextPhase();

            // Assert
            Assert.Equal(Level.GamePhase.GameOver, _level.CurrentPhase);
        }

        [Fact]
        public void CheckSpecificCardCondition_ShouldReturnTrueWhenTargetCardFound()
        {
            // Arrange
            _level.StartGame();
            _level.SetVictoryCondition(Level.VictoryCondition.SpecificCard);
            _player.Hand.Add(_targetCard);

            // Act
            _level.NextPhase();

            // Assert
            Assert.Equal(Level.GamePhase.GameOver, _level.CurrentPhase);
        }
    }
}
