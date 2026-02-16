# README — Iteration 7: Режимы игры (Levels + Endless)

## Новые скрипты

### LevelData.cs (ScriptableObject)
- Настраиваемые уровни: тип цели, значение, скорость спавна, макс. слоёв, стартовые слои, количество цветов
- 4 типа целей: RemoveLayers, ReachScore, SurviveTime, ChainReaction

### LevelManager.cs
- Загружает LevelData, настраивает GameManager/Tower
- Отслеживает прогресс к цели (удалённые слои, счёт, время, цепи)
- GoalPanel — появляется при старте уровня с описанием цели (анимация scale)
- ProgressText — обновляется в реальном времени ("4/10", "x2/x3", "30s")
- Разблокировка: сохраняет прогресс через PlayerPrefs

### EndlessManager.cs
- Ускорение спавна со временем: начинает с 10с, каждую минуту -1с, минимум 3с
- High score через PlayerPrefs

### LevelCompleteUI.cs
- Панель "LEVEL COMPLETE!" с кнопками NEXT / RETRY / MENU
- NEXT скрывается если нет следующего уровня

### LevelSelectUI.cs
- Сетка 4×3 с кнопками уровней
- Зелёные = пройдены, синие = доступны, тёмные = заблокированы
- Кнопка BACK

### Изменённые скрипты
- **GameManager.cs** — интеграция LevelManager/EndlessManager, SpawnInitialLayers в Start, NextLevel метод
- **MainMenuUI.cs** — Levels открывает LevelSelectUI вместо прямого перехода, показывает high score
- **GameOverUI.cs** — показывает best score в endless
- **GameColors.cs** — SetActiveColorCount для контроля сложности (меньше цветов = проще)

## 10 уровней по умолчанию

| # | Цель | Значение | Спавн | Цвета |
|---|------|----------|-------|-------|
| 1 | Remove Layers | 4 | 12с | 3 |
| 2 | Remove Layers | 6 | 10с | 4 |
| 3 | Reach Score | 1000 | 10с | 4 |
| 4 | Chain Reaction | x2 | 10с | 4 |
| 5 | Remove Layers | 10 | 8с | 5 |
| 6 | Survive Time | 60с | 7с | 5 |
| 7 | Reach Score | 3000 | 7с | 5 |
| 8 | Chain Reaction | x3 | 8с | 5 |
| 9 | Remove Layers | 15 | 6с | 5 |
| 10 | Reach Score | 5000 | 5с | 5 |

## Как настроить

### Gameplay сцена:
1. Заменить: GameManager.cs, GameOverUI.cs, GameColors.cs
2. Добавить: LevelData.cs, LevelManager.cs, EndlessManager.cs, LevelCompleteUI.cs, Editor/SetupIteration7.cs
3. Меню: **STACK → Setup Game Modes (Iteration 7) - Gameplay Scene**
4. Создаст LevelManager, EndlessManager, LevelCompleteUI, GoalPanel, ProgressText, 10 LevelData ассетов в Assets/STACK/Data/

### MainMenu сцена:
1. Добавить: LevelSelectUI.cs, обновить MainMenuUI.cs
2. Переключиться на MainMenu сцену
3. Меню: **STACK → Setup Level Select (Iteration 7) - MainMenu Scene**
4. Создаст LevelSelectUI с сеткой кнопок, привяжет к MainMenuUI
