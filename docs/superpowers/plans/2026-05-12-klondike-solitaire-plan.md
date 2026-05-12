# Klondike Solitaire Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create a complete Klondike Solitaire game engine (`CardGame.Solitaire`) with full test coverage and a WinForms UI (`CardGame.UI.Solitaire`).

**Architecture:**
- New standalone class library project `CardGame.Solitaire/` referencing existing `CardGame` for the `Card` type. Five classes: `TableauPile`, `FoundationPile`, `StockPile`, `WastePile`, `KlondikeGame`. Each follows TDD (red → green → commit).
- New WinForms application project `CardGame.UI.Solitaire/` with a single form `SolitaireForm` for interactive gameplay.

**Tech Stack:** .NET 9 / C# 12, xUnit, WinForms

---

## File Structure (Actual State)

| Project | Files | Status |
|---------|-------|--------|
| `CardGame.Solitaire/` | `SuitColor.cs`, `TableauPile.cs`, `FoundationPile.cs`, `StockPile.cs`, `WastePile.cs`, `KlondikeGame.cs` | ✅ All implemented |
| `CardGame.Solitaire.Tests/` | `SuitColorTests.cs`(18), `TableauPileTests.cs`(12), `FoundationPileTests.cs`(7), `StockPileTests.cs`(4), `WastePileTests.cs`(5), `KlondikeGameTests.cs`(16) | ✅ 62 tests total |
| `CardGame.UI.Solitaire/` | `SolitaireForm.cs`, `Program.cs` | ✅ WinForms app |

---

## ✅ Completed Tasks

### Task 1: Create project scaffolding and helper utility ✅

- [x] **Step 1: Create Solitaire class library project**
- [x] **Step 2: Create Solitaire test project**
- [x] **Step 3: Add both projects to solution**
- [x] **Step 4: Write the failing SuitColor tests**
- [x] **Step 5: Run tests to verify they fail**
- [x] **Step 6: Write minimal SuitColor implementation**
- [x] **Step 7: Run tests to verify they pass**
- [x] **Step 8: Commit**

Commit: `feat: scaffold CardGame.Solitaire projects and SuitColor helper`

### Task 2: TableauPile ✅

- [x] **Step 1: Write failing tests for TableauPile**
- [x] **Step 2: Run tests to verify they fail**
- [x] **Step 3: Write minimal TableauPile implementation**
- [x] **Step 4: Run tests to verify they pass**
- [x] **Step 5: Commit**

Commit: `feat: add TableauPile with placement rules and card movement`

### Task 3: FoundationPile ✅

- [x] **Step 1: Write failing tests for FoundationPile**
- [x] **Step 2: Run tests to verify they fail**
- [x] **Step 3: Write minimal FoundationPile implementation**
- [x] **Step 4: Run tests to verify they pass**
- [x] **Step 5: Commit**

Commit: `feat: add FoundationPile with ascending same-suit rules`

### Task 4: StockPile and WastePile ✅

- [x] **Step 1: Write StockPile failing tests**
- [x] **Step 2: Write WastePile failing tests**
- [x] **Step 3: Run tests to verify they fail**
- [x] **Step 4: Write StockPile implementation**
- [x] **Step 5: Write WastePile implementation**
- [x] **Step 6: Run tests to verify they pass**
- [x] **Step 7: Commit**

Commit: `feat: add StockPile and WastePile with draw/recycle mechanics`

### Task 5: KlondikeGame ✅

- [x] **Step 1: Write failing tests for KlondikeGame**
- [x] **Step 2: Run tests to verify they fail**
- [x] **Step 3: Write KlondikeGame implementation**
- [x] **Step 4: Run tests to verify they pass**
- [x] **Step 5: Run full solution tests**
- [x] **Step 6: Commit**

Commit: `feat: implement Klondike Solitaire game engine`

➡ Combined commits from Tasks 1-5 were squashed into one commit: `feat: implement Klondike Solitaire game engine`

### Task 6: WinForms UI (Post-Plan Addition) ✅

- [x] **Step 1: Create CardGame.UI.Solitaire WinForms project**
- [x] **Step 2: Implement SolitaireForm with stock/waste/foundation/tableau areas**
- [x] **Step 3: Implement card rendering with suit symbols (♠♥♦♣) and rank display**
- [x] **Step 4: Implement click-to-select and click-to-move interaction**
- [x] **Step 5: Support all move types (stock draw, waste recycle, tableau↔tableau, tableau→foundation, waste→tableau, waste→foundation)**
- [x] **Step 6: Implement win detection and visual feedback**
- [x] **Step 7: Build and verify application runs**
- [x] **Step 8: Fix OnResize NPE during form initialization**
- [x] **Step 9: Commit**

Commit: `feat: add Solitaire WinForms UI project`

### Task 7: Plan Alignment ✅

- [x] **Step 1: Mark all Task 1-5 checkboxes as completed**
- [x] **Step 2: Add Task 6 (UI) and Task 7 (plan alignment) to document**
- [x] **Step 3: Update architecture description to include UI project**
- [x] **Step 4: Update file structure table with actual state**

---

## Self-Review

**Spec coverage check:**
- ✅ TableauPile: placement rules (red-black, descending, K on empty), FlipTop, TakeFrom, GetMovableCards
- ✅ FoundationPile: A start, same-suit ascending, K completion
- ✅ StockPile/WastePile: draw 1, recycle waste
- ✅ KlondikeGame: deal 7 columns, draw from stock, waste→tableau, waste→foundation, tableau→foundation, tableau→tableau, recycle, check win, restart
- ✅ WinForms UI: interactive gameplay with all move types, card rendering, win detection

**Placeholder scan:** All steps completed. No TBD, TODO, or incomplete sections.

**Type consistency:** All method signatures across tasks match. `SuitColor.RankToNumber`, `CanPlace`, `TakeFrom`, `TakeTop`, `TakeAll` all consistent.

**Test Results:** 113 tests total — 51 original + 62 new. All passing, 0 failures.