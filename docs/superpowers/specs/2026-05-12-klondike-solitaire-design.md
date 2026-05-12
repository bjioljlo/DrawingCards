# Klondike Solitaire 設計文件

- **日期**: 2026-05-12
- **專案**: DrawingCards / CardGame.Solitaire
- **狀態**: v1 草稿

---

## 1. 目標

在 DrawingCards 卡片遊戲框架中，新增 Klondike Solitaire（經典單人接龍）遊戲。採用獨立專案 `CardGame.Solitaire` 實作，不修改現有 `CardGame` 核心元件。

---

## 2. 專案結構

```
DrawingCards/
├── CardGame.Solitaire/                  # 接龍邏輯專案
│   ├── KlondikeGame.cs                  # 遊戲主控制器
│   ├── TableauPile.cs                   # 7 列牌堆
│   ├── FoundationPile.cs                # 4 色牌堆
│   ├── StockPile.cs                     # 牌庫
│   ├── WastePile.cs                     # 廢牌堆
│   └── CardGame.Solitaire.csproj
├── CardGame.Solitaire.Tests/            # 接龍測試專案
│   ├── TableauPileTests.cs
│   ├── FoundationPileTests.cs
│   ├── StockPileTests.cs
│   ├── WastePileTests.cs
│   └── KlondikeGameTests.cs
└── CardGame.UI.Solitaire/               # 接龍 UI 專案（未來建立）
    └── CardGame.UI.Solitaire.csproj
```

---

## 3. 類別設計與業務規則

### 3.1 `TableauPile` — 單列牌堆

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Index` | `int` | 0..6，第幾列 |
| `Cards` | `IReadOnlyList<Card>` | 所有牌（包含蓋著的） |
| `TopCard` | `Card?` | 最上面那張翻開的牌，空列回傳 null |
| `IsEmpty` | `bool` | 是否為空列 |
| `FaceUpCount` | `int` | 翻開的牌數量 |

| 方法 | 回傳 | 說明 |
|------|------|------|
| `CanPlace(Card)` | `bool` | 檢查單張牌是否能放（紅黑交替、降序、空列需為 K） |
| `CanPlace(IReadOnlyList<Card>)` | `bool` | 檢查一疊牌是否能放 |
| `Place(Card)` | `void` | 放一張牌 |
| `Place(IReadOnlyList<Card>)` | `void` | 放一疊牌 |
| `TakeFrom(int index)` | `List<Card>` | 從指定索引取出該張以後的所有牌 |
| `FlipTop()` | `bool` | 翻開最上面蓋著的牌，回傳是否真的有翻開 |
| `GetMovableCards(int index)` | `IReadOnlyList<Card>?` | 從索引開始檢查是否符合可移動規則，不符回傳 null |

**業務規則：**
1. 疊牌：花色紅黑交替，數字降序（K → Q → J → 10 → ... → 2）
2. 空列只能放 K
3. 移走翻開牌後，下方的蓋牌自動翻開
4. 取牌時只能從某張牌開始連同其上方的牌一起移動

### 3.2 `FoundationPile` — 四色牌堆

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Suit` | `Suit` | 花色（由第一張放入的牌決定） |
| `Cards` | `IReadOnlyList<Card>` | 已放置的牌 |
| `TopCard` | `Card?` | 最上面的牌 |
| `IsComplete` | `bool` | 是否已放到 K |
| `IsEmpty` | `bool` | 是否為空 |

| 方法 | 回傳 | 說明 |
|------|------|------|
| `CanPlace(Card)` | `bool` | 檢查是否能放（同花色、升序） |
| `Place(Card)` | `void` | 放牌 |
| `CheckComplete()` | `bool` | 檢查頂牌是否為 K |

**業務規則：**
1. 必須同花色
2. 數字必須升序（A → 2 → 3 → ... → K）
3. 空堆只能放 A
4. 四疊都到 K 即獲勝

### 3.3 `StockPile` — 牌庫

| 屬性 | 類型 | 說明 |
|------|------|------|
| `RemainingCards` | `int` | 剩餘牌數 |
| `IsEmpty` | `bool` | 是否空了 |

| 方法 | 回傳 | 說明 |
|------|------|------|
| `Draw()` | `Card?` | 翻一張牌到 Waste，沒牌回傳 null |
| `ResetFromWaste(WastePile)` | `void` | 從 Waste 回收所有牌 |

**業務規則：**
1. 初始 24 張
2. 每次翻 1 張到 Waste
3. 空了 + Waste 有牌時可以回收
4. 回收不洗牌，保持原始順序

### 3.4 `WastePile` — 廢牌堆

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Cards` | `IReadOnlyList<Card>` | 所有牌 |
| `TopCard` | `Card?` | 最上面的牌 |
| `IsEmpty` | `bool` | 是否為空 |
| `CanReturnToStock` | `bool` | Stock 空且 Waste 有牌時為 true |

| 方法 | 回傳 | 說明 |
|------|------|------|
| `Add(Card)` | `void` | 從 Stock 加入一張牌 |
| `TakeTop()` | `Card` | 取出最上面那張 |
| `TakeAll()` | `List<Card>` | 取出所有牌（回收用） |

### 3.5 `KlondikeGame` — 遊戲主控制器

| 屬性 | 類型 | 說明 |
|------|------|------|
| `State` | `GameState` | NotStarted / Playing / Won |
| `Stock` | `StockPile` | 牌庫 |
| `Waste` | `WastePile` | 廢牌堆 |
| `Tableaus` | `IReadOnlyList<TableauPile>` | 7 列 |
| `Foundations` | `IReadOnlyList<FoundationPile>` | 4 色堆 |
| `MovesCount` | `int` | 移動次數 |

| 方法 | 回傳 | 說明 |
|------|------|------|
| `StartGame()` | `void` | 初始化遊戲（發牌到 7 列） |
| `DrawFromStock()` | `void` | 從 Stock 翻牌到 Waste |
| `MoveCard(int fromT, int toT, int idx)` | `bool` | Tableau → Tableau |
| `MoveFromWasteToTableau(int toT)` | `bool` | Waste → Tableau |
| `MoveFromWasteToFoundation(int toF)` | `bool` | Waste → Foundation |
| `MoveToFoundation(int fromT, int toF)` | `bool` | Tableau → Foundation |
| `RecycleWaste()` | `void` | 回收 Waste 回 Stock |
| `CheckWin()` | `bool` | 檢查是否獲勝 |
| `Restart()` | `void` | 重新開始 |

**發牌邏輯：**
1. 第 0 列：1 張（翻開）
2. 第 1 列：2 張（第 2 張翻開）
3. 第 2 列：3 張（第 3 張翻開）
4. ...依此類推到第 6 列：7 張（第 7 張翻開）
5. 共發 28 張，剩 24 張進 Stock

---

## 4. 遊戲流程

```
StartGame()
     │
     ▼
[Playing] ──點擊 Stock──→ DrawFromStock() ──→ Waste 增加
     │                                              │
     │         ┌─── 點擊 Waste 頂牌 ───┐             │
     │         ▼                       ▼             │
     │  MoveFromWasteToTableau()  MoveFromWasteToFoundation()
     │         │                       │
     │         ▼                       ▼
     │    Tableau 更新           Foundation 更新
     │         │                       │
     │         └──── CheckWin() ──────┘
     │                  │
     │              已全到 K？
     │               ├─ yes → [Won]
     │               └─ no  → 繼續
     │
     └── 回收 ──→ RecycleWaste() (當 Stock 空時)
```

---

## 5. 未完成項目（未來階段）

- **翻牌模式**：支援翻 3 張模式
- **計時與計分**
- **拖曳操作**（目前暫以點擊選取 + 點擊放置替代）
- **提示功能**
- **取消/重做**
- **動畫效果**
- **存檔/載入**

---

## 6. 測試策略

每個類別遵循 TDD，測試覆蓋率目標 ≥ 90%：

- **`TableauPileTests`**: 放置規則測試 x6，空列規則 x2，翻牌 x2，取牌 x3
- **`FoundationPileTests`**: 放置規則測試 x4，完成判斷 x2
- **`StockPileTests`**: 抽牌 x2，空牌堆 x2，回收 x2
- **`WastePileTests`**: 加入/取出 x3，回收 x2
- **`KlondikeGameTests`**: 初始狀態 x3，完整遊戲流程 x2，移動操作 x4，勝利判斷 x2