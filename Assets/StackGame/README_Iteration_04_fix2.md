# Iteration 4 Fix 2 — 8 кубиков на слой, матчинг по одной стороне

## Что изменилось

### BlockLayer.cs (переписан с нуля)
- 8 кубиков (Unity Cube primitive) на слой: по 2 на каждую сторону (left + right половинки)
- Front/Back — полная ширина, Right/Left — укорочены на толщину стенки чтобы не пересекались в углах
- Маленький gap (0.02) между половинками
- Fill куб в центре (DarkBase), чуть меньше внутреннего пространства (без артефактов)
- Standard shader, цвет через material.color
- rotationStep для логики матчинга

### Tower.cs
- 8 случайных цветов на слой

### StackChecker.cs
- **matchSide** — настройка в инспекторе (0=Front, 1=Right, 2=Back, 3=Left)
- Проверяет только указанную сторону
- Матч = обе половинки (left И right) совпадают между соседними слоями

### SetupIteration1.cs
- Без шейдера/материала

## Как настроить

1. Заменить: BlockLayer.cs, Tower.cs, StackChecker.cs, Editor/SetupIteration1.cs
2. Открыть Gameplay сцену
3. **STACK → Setup Gameplay Scene (Iteration 1)**
4. **STACK → Setup Gameplay UI and Controls (Iteration 2)**
5. **STACK → Setup Stack Mechanic (Iteration 4)**
6. Выбрать StackChecker в Hierarchy → в Inspector задать **Match Side** (перебери 0-3 и найди какая сторона справа от камеры)

## Тестирование

- Каждая сторона слоя = 2 цветных кубика
- Повернуть слой чтобы на matchSide стороне совпали оба цвета с соседом
- Нажать Confirm → вспышка → сжатие → удаление → падение
