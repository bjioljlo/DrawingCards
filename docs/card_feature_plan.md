# Card（卡牌）功能設計規劃

## 目標
設計 Card 類別，作為遊戲中所有牌的基本資料結構，支援花色、點數、名稱、描述等屬性，並可擴充特殊效果。

## Card 類別設計

### 屬性
- `Suit`：花色（string）
- `Rank`：點數/牌面（string）
- `Name`：牌名（string，UI 顯示用）
- `ID`：唯一識別碼（int）
- `Description`：描述（string）
- `Effect`：特殊效果描述（string，可選）

### 方法
- `Equals(object obj)`：判斷兩張牌是否相等（根據花色、點數、ID）
- `GetHashCode()`：取得雜湊碼（配合集合操作）
- `ToString()`：回傳牌名或描述，便於 UI 顯示

## 整合建議
- Card 為 Deck、Hand、Player、DiscardPile 等功能的基礎資料結構。
- 可根據遊戲規則擴充屬性（如分數、特殊效果等）。
- 支援序列化與反序列化（如 JSON 載入 Deck 時使用）。
- 可作為 UI 控制元件的 Tag 屬性，支援多張同時操作。

## 測試建議
- 驗證 Card 建構與屬性正確性。
- 驗證 Equals、GetHashCode、ToString 行為。
- 驗證不同卡牌的唯一性。
- 驗證序列化與反序列化正確性。

## 擴充性建議
- 支援特殊牌型（如鬼牌、功能牌）。
- 支援自訂屬性（如分數、顏色、標記等）。
- 支援序列化與反序列化（for 儲存/網路傳輸）。

---

此文件為卡牌功能設計初稿，後續可依需求調整細節。
