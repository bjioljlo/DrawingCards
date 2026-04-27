# AGENTS 開發規範 - SDD + TDD 開發流程

## 專案目標
本文件定義 DrawingCards 卡片遊戲的開發規範，採用 **SDD (Specification Driven Development) + TDD (Test Driven Development)** 雙驅動開發模式。

---

## 📋 開發流程 (SDD → TDD → 實作)

### 1️⃣ 第一階段: SDD 規格驅動開發
每個功能開發前必須先完成以下文件:

| 步驟 | 產出文件 | 內容說明 |
|------|----------|----------|
| 1 | `docs/[功能]_feature_plan.md` | ✅ 功能需求說明<br/>✅ 使用者場景<br/>✅ 業務規則<br/>✅ 邊界條件 |
| 2 | 介面合約 | ✅ 公開方法簽章<br/>✅ 輸入輸出定義<br/>✅ 例外狀況 |
| 3 | 驗收標準 | ✅ 通過條件<br/>✅ 測試案例清單 |

> 💡 SDD 原則: **先寫規格，再寫程式**。沒有文件的功能不應該進入開發。

---

### 2️⃣ 第二階段: TDD 測試驅動開發
在撰寫實際程式碼之前，必須先撰寫單元測試:

#### TDD 循環 (紅 → 綠 → 重構)
1. **紅**: 寫一個失敗的測試
2. **綠**: 寫剛好足夠的程式碼讓測試通過
3. **重構**: 重構程式碼，保持測試通過

#### 測試規範
```csharp
// 測試命名規範: [受測方法]_[情境]_[預期結果]
public void EndTurn_WhenInActionPhase_ShouldEnableButton()
public void EndTurn_WhenInDrawingPhase_ShouldDisableButton()
public void EndTurn_WhenInScoringPhase_ShouldThrowException()
```

✅ 每個公開方法至少 3 個測試案例:
  - 正常路徑
  - 邊界條件
  - 錯誤處理

---

## 🎯 功能開發範本

### ✅ 範例: 結束回合按鈕功能

#### 🔹 SDD 文件 (`docs/endturn_feature_plan.md`)
```
## 功能需求
結束回合按鈕只在行動階段亮起可供點擊

## 業務規則
1. 抽牌階段: 按鈕停用 (灰色)
2. 行動階段: 按鈕啟用 (正常)
3. 計分階段: 按鈕停用 (灰色)
4. 非行動階段點擊按鈕應顯示錯誤訊息
5. 出牌後應保留在行動階段，按鈕維持啟用

## 驗收標準
- [ ] 抽牌階段按鈕無法點擊
- [ ] 行動階段按鈕可以點擊
- [ ] 計分階段按鈕無法點擊
- [ ] 出牌後按鈕仍然保持亮起
- [ ] 非行動階段點擊顯示正確錯誤訊息
```

#### 🔹 TDD 測試 (`CardGame.Tests/EndTurnTests.cs`)
```csharp
[Fact]
public void EndTurnButton_ActionPhase_Enabled()
{
    // Arrange
    var level = new Level();
    level.StartGame();
    level.NextPhase(); // 進入行動階段
    
    // Act
    var buttonState = level.CanEndTurn();
    
    // Assert
    Assert.True(buttonState);
}

[Fact]
public void EndTurnButton_DrawingPhase_Disabled()
{
    // Arrange
    var level = new Level();
    level.StartGame();
    
    // Act
    var buttonState = level.CanEndTurn();
    
    // Assert
    Assert.False(buttonState);
}
```

#### 🔹 實作程式碼
```csharp
public bool CanEndTurn() 
{
    return CurrentPhase == GamePhase.Action;
}
```

---

## 📁 專案結構規範

```
DrawingCards/
├── 📄 AGENTS.md               (本規範文件)
├── 📂 CardGame/              (核心邏輯)
│   ├── 📄 Card.cs
│   ├── 📄 Deck.cs
│   ├── 📄 Player.cs
│   ├── 📄 Table.cs
│   └── 📄 Level.cs
├── 📂 CardGame.Tests/        (單元測試)
│   ├── 📄 CardTests.cs
│   ├── 📄 DeckTests.cs
│   ├── 📄 PlayerTests.cs
│   ├── 📄 TableTests.cs
│   └── 📄 LevelTests.cs
├── 📂 CardGame.UI/           (UI 層)
│   └── 📄 MainForm.cs
└── 📂 docs/                  (SDD 文件)
    ├── 📄 card_feature_plan.md
    ├── 📄 deck_feature_plan.md
    ├── 📄 hand_feature_plan.md
    ├── 📄 table_feature_plan.md
    └── 📄 level_feature_plan.md
```

---

## ✅ 程式碼品質標準

### 設計原則
1. **單一職責**: 每個類別只有一個改變的理由
2. **開放封閉**: 對擴充開放，對修改封閉
3. **依賴反轉**: 高層模組不依賴低層模組
4. **介面隔離**: 不要強迫客戶端依賴他們不需要的方法

### 程式碼規範
- 方法長度 ≤ 30 行
- 類別長度 ≤ 300 行
- 巢狀層級 ≤ 3 層
- 參數數量 ≤ 5 個
- 明確的命名，不要縮寫
- 每個方法只有一個進入點與一個離開點

---

## 🔄 開發生命週期

### 每個功能的開發步驟:
1. 📝 建立 SDD 功能計畫文件
2. ✅ 提交文件進行審核
3. 🧪 撰寫單元測試
4. 🔴 執行測試 (預期失敗)
5. 💻 撰寫實作程式碼
6. 🟢 執行測試 (全部通過)
7. ♻️ 重構程式碼
8. ✔️ 手動驗收測試
9. 🚀 提交合併

---

## 🎯 品質指標

| 指標 | 標準 |
|------|------|
| 單元測試涵蓋率 | ≥ 90% |
| 編譯警告 | 0 個 |
| 程式碼分析警告 | 0 個 |
| 所有測試 | 100% 通過 |
| 循環複雜度 | ≤ 10 |

---

## 📌 承諾
身為 AGENTS 開發團隊成員，我承諾:
1. 沒有文件的功能不開發
2. 沒有測試的程式碼不提交
3. 不通過測試的版本不整合
4. 永遠先寫測試，再寫實作
5. 保持程式碼乾淨、可讀、可維護

---

> 🚀 好的規格 + 好的測試 = 好的程式碼
> 
> **SDD 讓我們做對的事，TDD 讓我們把事做對**