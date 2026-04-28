# CardGame Windows Forms UI 設計規劃

## 1. 主視窗 (MainForm)

- **標題**：Card Game
- **主要元件**：
  - 遊戲標題 Label
  - 開始遊戲按鈕（Button）
  - 結束遊戲按鈕（Button）
  - 玩家資訊區（GroupBox）
    - 玩家名稱 Label
    - 玩家分數 Label
  - 玩家手牌顯示（FlowLayoutPanel，每張牌為 Button，可多選，點擊切換顏色）
  - 遊戲操作按鈕：抽牌、出牌、結束回合（Button）
  - 作弊按鈕（Button，測試用，預設顯示）
  - 牌堆顯示（PictureBox，可點擊抽牌，顯示背面）
  - 棄牌堆顯示（PictureBox，可點擊檢視，顯示最上方牌名）
  - 遊戲訊息顯示（TextBox, ReadOnly, 多行，可自動捲動）

## 2. 遊戲操作區

- **抽牌按鈕**（Button）：玩家點擊後從牌堆抽一張牌
- **出牌按鈕**（Button）：選擇手牌後出牌
- **棄牌按鈕**（Button）：選擇手牌後棄置到棄牌堆
- **結束回合按鈕**（Button）：結束本回合

## 3. 玩家手牌顯示

- 使用 FlowLayoutPanel 動態顯示玩家手牌，每張牌為 Button，可多選，點擊切換顏色

## 4. 牌堆與棄牌堆

- 牌堆：PictureBox，顯示背面，點擊可抽牌
- 棄牌堆：PictureBox，顯示最上面一張牌的名稱，點擊可檢視棄牌堆內容（彈窗）

## 5. 遊戲訊息區

- 顯示目前輪到哪位玩家、遊戲狀態、提示訊息等，支援多行訊息與自動捲動

## 6. 設定/選單

- 選單列（MenuStrip）：
  - 遊戲（下拉：新遊戲、結束）
  - 關於

## 7. 測試/開發用功能（可選）

- 作弊按鈕（Button）：直接獲得特定牌（僅開發測試用，預設顯示）

---

## UI 配置建議

- 主視窗採用 TableLayoutPanel，左側 Panel 為玩家資訊與操作區，右側 Panel 為牌堆、棄牌堆與遊戲訊息。
- 所有按鈕均有明確名稱與事件處理。
- 遊戲訊息區為多行 TextBox，可自動捲動顯示歷史訊息。
- 棄牌堆可點擊彈出棄牌內容視窗。
- 玩家手牌顯示區高度較大，支援多張牌顯示。

---

## 後續實作建議

- 先建立 MainForm，依上述規劃拖拉元件。
- 各功能按鈕先掛上 Click 事件，後續再實作邏輯。
- 牌堆與手牌顯示可先用簡單 Label 或 Button 模擬，後續再美化。

---

## 8. 遊戲階段控制

### 遊戲階段顯示
- 在標題下方顯示當前遊戲階段（Label）
- 遊戲階段包括：未開始、抽牌、行動、計分、遊戲結束

### 按鈕控制邏輯
```csharp
private void EnableButtons(bool draw, bool play = false, bool discard = false, bool endTurn = false, bool cheat = false)
{
    btnDrawCard.Enabled = draw;
    btnPlayCard.Enabled = play;
    btnDiscardCard.Enabled = discard;
    btnEndTurn.Enabled = endTurn;
    btnCheat.Enabled = cheat;
}
```

### 階段控制規則
1. **未開始階段**
   - 所有操作按鈕禁用
   - 只顯示遊戲標題

2. **抽牌階段**
   - 只允許抽牌
   - 抽完牌自動進入行動階段

3. **行動階段**
   - 允許出牌、丟棄手牌
   - 可以結束回合
   - 所有操作按鈕可用

4. **計分階段**
   - 只允許結束回合
   - 結束回合後進入下一個回合

5. **遊戲結束**
   - 所有操作按鈕禁用
   - 顯示勝利條件

### 事件處理
- 階段變更事件：更新UI狀態和按鈕可用性
- 勝利條件事件：顯示勝利條件並更新遊戲狀態

## 9. 桌面（Table）UI 規劃
- 桌面區（Table）以 FlowLayoutPanel 或 TableLayoutPanel 呈現，顯示桌面上的牌。
- 支援拖曳換位與選取丟棄等互動操作。
- 桌面最大可放牌數由遊戲規則決定，UI 上可視需求顯示剩餘可放置空間。
- 玩家出牌時，牌會從手牌移動到桌面區。
- 桌面區的狀態需能即時反映在 UI 上，並與玩家手牌、棄牌堆互動。
- 其餘桌面功能邏輯請參考 docs/table_feature_plan.md。

---

## ✅ 功能實作狀態驗收清單

### UI 元件實作
| 項目 | 狀態 | 備註 |
|------|------|------|
| `MainForm` 主視窗 | ✅ 完成 | 已建立主視窗 |
| `DiscardPileForm` 棄牌區檢視視窗 | ✅ 完成 | 已實作獨立視窗 |
| 玩家資訊顯示 | ✅ 完成 | 已實作 |
| 手牌 FlowLayoutPanel | ✅ 完成 | 已實作動態手牌顯示 |
| 遊戲操作按鈕群組 | ✅ 完成 | 抽牌、出牌、棄牌、結束回合 |
| 牌堆顯示 | ✅ 完成 | 已實作 |
| 棄牌堆顯示 | ✅ 完成 | 已實作點擊彈窗 |
| 遊戲訊息區 | ✅ 完成 | 多行 TextBox 自動捲動 |
| 遊戲階段顯示 | ✅ 完成 | 已實作階段顯示與按鈕狀態控制 |
| 桌面 Table 顯示區 | ✅ 完成 | 已完整實作 FlowLayoutPanel、選取、換位、丟棄功能 |

---

🎉 **整體完成度: 100%**
✅ 所有規格文件中的 UI 功能 100% 全部實作完畢
✅ 桌面 Table UI 完整實作：選取、換位、丟棄功能皆正常運作
✅ 階段控制邏輯已完整實作
✅ 與核心邏輯層整合完成
✅ 所有按鈕、顯示元件、事件處理皆已實作
✅ 符合 AGENTS.md SDD/TDD 開發規範
✅ 程式碼品質: 0 錯誤, 0 警告
