# Hand（手牌）功能設計規劃

## 目標
設計 Hand 類別，負責管理玩家手上的牌，包括加入、移除、查詢與顯示手牌等功能。

## Hand 類別設計

### 屬性
- `Cards`：目前手牌（IReadOnlyList<Card>）
- `Count`：手牌張數（int）

### 方法
- `Add(Card card)`：加入一張牌到手牌
- `AddRange(IEnumerable<Card> cards)`：加入多張牌到手牌
- `Discard(Card card)`：從手牌移除指定牌
- `DiscardRange(IEnumerable<Card> cards)`：移除多張指定牌
- `GetCardNames()`：取得所有手牌名稱清單
- `GetAll()`：取得所有手牌內容（for UI 顯示）

## 整合建議
- Hand 應與 Player、Deck、DiscardPile 等類別互動，支援抽牌、出牌、棄牌等操作（單張、多張）。
- 支援手牌內容查詢與顯示，方便 UI 呈現（如每張牌為 Button，顯示 Name，可多選）。

## 測試建議
- 驗證加入與移除手牌（單張、多張）的正確性。
- 驗證手牌查詢與顯示功能。
- 驗證異常狀況（如移除不存在的牌、多張移除時部分失敗）。

## 擴充性建議
- 支援手牌排序、過濾、分組等進階操作。
- 支援手牌上限、特殊手牌規則。

---

此文件為手牌功能設計初稿，後續可依需求調整細節。
