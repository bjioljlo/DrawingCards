namespace CardGame
{
    public class Level
    {
        // 遊戲階段枚舉
        public enum GamePhase
        {
            NotStarted,      // 遊戲未開始
            Drawing,         // 抽牌階段
            Action,          // 行動階段
            Scoring,         // 計分階段
            GameOver         // 遊戲結束
        }

        // 勝利條件枚舉
        public enum VictoryCondition
        {
            None,           // 無條件（遊戲進行中）
            TargetScore,    // 達到目標分數
            NoCardsLeft,    // 無牌可抽
            SpecificCard,   // 抽到特定牌
            Custom          // 自訂條件
        }

        // 事件委托
        public delegate void PhaseChangedEventHandler(GamePhase newPhase);
        public delegate void VictoryConditionMetEventHandler(VictoryCondition condition);
        public event PhaseChangedEventHandler? PhaseChanged;
        public event VictoryConditionMetEventHandler? VictoryConditionMet;

        // 屬性
        public GamePhase CurrentPhase { get; private set; } = GamePhase.NotStarted;
        public int TurnCount { get; private set; } = 0;
        public DateTime PhaseStartedAt { get; private set; }
        public bool IsGameStarted => CurrentPhase != GamePhase.NotStarted;
        public bool IsGameOver => CurrentPhase == GamePhase.GameOver;
        public VictoryCondition CurrentVictoryCondition { get; private set; } = VictoryCondition.None;

        // 勝利條件相關屬性
        private readonly List<Player> _players;
        private readonly Deck _deck;
        private readonly Card? _targetCard;
        private readonly int _targetScore;

        // 建構子
        public Level(List<Player> players, Deck deck, Card? targetCard = null, int targetScore = 0)
        {
            _players = players;
            _deck = deck;
            _targetCard = targetCard;
            _targetScore = targetScore;
            Reset();
        }

        // 方法
        public void StartGame()
        {
            if (IsGameStarted)
                throw new InvalidOperationException("遊戲已經開始");

            CurrentPhase = GamePhase.Drawing;
            TurnCount = 1;
            PhaseStartedAt = DateTime.Now;
            PhaseChanged?.Invoke(CurrentPhase);
        }

        public void NextPhase()
        {
            if (!CanProceedToNextPhase())
                throw new InvalidOperationException("無法進入下一個階段");

            // 檢查勝利條件
            if (CheckVictoryConditions())
            {
                CurrentPhase = GamePhase.GameOver;
                PhaseChanged?.Invoke(CurrentPhase);
                return;
            }

            switch (CurrentPhase)
            {
                case GamePhase.Drawing:
                    CurrentPhase = GamePhase.Action;
                    break;
                case GamePhase.Action:
                    CurrentPhase = GamePhase.Scoring;
                    break;
                case GamePhase.Scoring:
                    CurrentPhase = GamePhase.Drawing;
                    TurnCount++;
                    break;
                default:
                    throw new InvalidOperationException($"無法從 {CurrentPhase} 階段進入下一個階段");
            }

            PhaseStartedAt = DateTime.Now;
            PhaseChanged?.Invoke(CurrentPhase);
        }

        public void SetVictoryCondition(VictoryCondition condition)
        {
            CurrentVictoryCondition = condition;
        }

        private bool CheckVictoryConditions()
        {
            switch (CurrentVictoryCondition)
            {
                case VictoryCondition.TargetScore:
                    return CheckTargetScoreCondition();
                case VictoryCondition.NoCardsLeft:
                    return CheckNoCardsLeftCondition();
                case VictoryCondition.SpecificCard:
                    return CheckSpecificCardCondition();
                case VictoryCondition.Custom:
                    return CheckCustomCondition();
                default:
                    return false;
            }
        }

        private bool CheckTargetScoreCondition()
        {
            foreach (var player in _players)
            {
                if (player.Score >= _targetScore)
                {
                    VictoryConditionMet?.Invoke(VictoryCondition.TargetScore);
                    return true;
                }
            }
            return false;
        }

        private bool CheckNoCardsLeftCondition()
        {
            if (_deck.Count == 0)
            {
                VictoryConditionMet?.Invoke(VictoryCondition.NoCardsLeft);
                return true;
            }
            return false;
        }

        private bool CheckSpecificCardCondition()
        {
            if (_targetCard == null)
                return false;

            foreach (var player in _players)
            {
                if (player.Hand.Cards.Contains(_targetCard) || player.DiscardPile.Contains(_targetCard))
                {
                    VictoryConditionMet?.Invoke(VictoryCondition.SpecificCard);
                    return true;
                }
            }
            return false;
        }

        private bool CheckCustomCondition()
        {
            // TODO: 實作自訂條件邏輯
            return false;
        }

        public void EndGame()
        {
            if (IsGameOver)
                return;

            CurrentPhase = GamePhase.GameOver;
            PhaseChanged?.Invoke(CurrentPhase);
        }

        public GamePhase GetCurrentPhase()
        {
            return CurrentPhase;
        }

        public TimeSpan GetPhaseDuration()
        {
            return DateTime.Now - PhaseStartedAt;
        }

        public void Reset()
        {
            CurrentPhase = GamePhase.NotStarted;
            TurnCount = 0;
            PhaseStartedAt = DateTime.MinValue;
            CurrentVictoryCondition = VictoryCondition.None;
            PhaseChanged?.Invoke(CurrentPhase);
        }

        private bool CanProceedToNextPhase()
        {
            return CurrentPhase switch
            {
                GamePhase.NotStarted => false,
                GamePhase.GameOver => false,
                _ => true,
            };
        }
    }
}
