# Player 功能設計規劃

## 目標
設計一個 Player（玩家）類別，讓遊戲能支援多名玩家，並能管理每位玩家的手牌、分數與基本操作。

## Player 類別設計

### 屬性
- `Id`：玩家唯一識別碼（int 或 Guid）
- `Name`：玩家名稱（string）
- `Hand`：玩家手牌（Hand 類別）
- `Score`：玩家分數（int）

### 遊戲區域
- `Deck`：牌組（Deck 類別，所有未被抽出的牌）
- `DiscardPile`：棄牌區（可設計為 DiscardPile 類別或 List<Card>，存放被棄掉的牌）

### 方法
- `DrawCard(Deck deck)`：從牌組抽一張牌加入手牌
- `DrawCards(Deck deck, int count)`：從牌組抽多張牌
- `PlayCard(Card card, Table table)`：從手牌打出一張牌並放到桌面（Table）
- `PlayCards(IEnumerable<Card> cards, Table table)`：從手牌打出多張牌並放到桌面（Table）（可選擇性擴充）
- `ShowHand()`：顯示手牌內容（for UI 顯示）
- `CalculateScore()`：計算玩家分數
- `DiscardCard(Card card, DiscardPile discardPile)`：將手牌中的牌棄至棄牌區
- `DiscardCards(IEnumerable<Card> cards, DiscardPile discardPile)`：將多張手牌棄至棄牌區

## 整合建議
- 在 CardGame 專案新增 Player.cs，實作 Player 類別。
- 新增 DiscardPile 類別（或以 List<Card> 實作），管理棄牌區。
- 修改遊戲主流程（如 Program.cs 或 CardGame.cs），支援多名玩家的初始化與回合操作。
- 玩家與 Hand、Deck、DiscardPile、Card 類別互動，實現抽牌、出牌、棄牌等功能。

## 測試建議
- 在 CardGame.Tests 專案新增 PlayerTests.cs，撰寫 Player 相關單元測試。
- 測試內容包含：
  - 玩家初始化
  - 抽牌、出牌、棄牌（單張、多張）
  - 分數計算

## UI 整合（如有需要）
- 在 CardGame.UI 的 MainForm.cs 增加顯示玩家資訊（名稱、分數、手牌）的功能。
- 支援多名玩家的切換與操作。
- 支援手牌多選、出多張牌、棄多張牌的 UI 操作。

## 擴充性建議
- 可擴充 AIPlayer 類別，繼承自 Player，實現電腦玩家邏輯。
- 支援玩家屬性自訂（如頭像、顏色等）。

---

此文件為玩家功能設計初稿，後續可依需求調整細節。