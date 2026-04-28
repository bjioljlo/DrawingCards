# DrawingCards 卡片遊戲框架

一個可擴充的卡片遊戲開發框架，採用 **SDD (規格驅動開發) + TDD (測試驅動開發)** 開發流程。

---

## 📋 專案簡介

DrawingCards 是一個模組化的卡片遊戲核心架構，提供卡片、牌堆、手牌、桌面、回合管理等基礎元件，
可以快速擴充開發各種類型的紙牌遊戲。

---

## 🛠 技術架構

| 專案 | 說明 | 技術 |
|------|------|------|
| `CardGame` | 核心遊戲邏輯層 | .NET 8 / C# 12 |
| `CardGame.Tests` | 單元測試專案 | xUnit |
| `CardGame.UI` | Windows Forms 使用者介面 | WinForms |

---

## 📁 專案結構

```
DrawingCards/
├── 📄 AGENTS.md               開發規範文件
├── 📄 README.md               本文件
├── 📂 CardGame/              核心邏輯
│   ├── 📄 Card.cs            卡片物件
│   ├── 📄 Deck.cs            牌堆管理
│   ├── 📄 Hand.cs            手牌管理
│   ├── 📄 Player.cs          玩家物件
│   ├── 📄 Table.cs           桌面狀態
│   ├── 📄 Level.cs           遊戲回合與階段管理
│   └── 📄 DiscardPile.cs     棄牌堆
├── 📂 CardGame.Tests/        單元測試
├── 📂 CardGame.UI/           使用者介面
└── 📂 docs/                  SDD 功能規格文件
```

---

## 🚀 快速開始

### 建置專案
```bash
dotnet build
```

### 執行測試
```bash
dotnet test
```

### 執行遊戲
```bash
dotnet run --project CardGame.UI
```

---

## 🎯 開發規範

本專案嚴格遵循 **SDD + TDD** 雙驅動開發流程:

### ✅ 開發流程
1. **SDD 階段**: 先撰寫功能規格文件與驗收標準
2. **TDD 階段**: 先撰寫單元測試，再撰寫實作程式碼
3. **紅 → 綠 → 重構**: 維持測試覆蓋率 90% 以上

詳細開發規範請參閱 [AGENTS.md](AGENTS.md)

---

## 📊 品質指標

| 指標 | 標準 | 目前狀態 |
|------|------|----------|
| 單元測試涵蓋率 | ≥ 90% |  |
| 編譯警告 | 0 個 |  |
| 所有測試 | 100% 通過 |  |
| 循環複雜度 | ≤ 10 |  |

---

## 🤝 開發團隊

採用 AGENTS AI 協同開發流程

---

## 📝 License

MIT License

---

> 🚀 好的規格 + 好的測試 = 好的程式碼
>
> **SDD 讓我們做對的事，TDD 讓我們把事做對**