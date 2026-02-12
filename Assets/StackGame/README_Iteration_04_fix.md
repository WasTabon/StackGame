# Iteration 4 Fix — Полная переделка системы слоёв

## Что изменилось

Слои полностью переделаны. Вместо процедурного меша с vertex colors — **4 куба (Unity Cube primitive) на слой**, каждый куб = одна сторона = один цвет.

### BlockLayer.cs (переписан с нуля)
- Каждый слой состоит из 4 тонких кубов (Front, Right, Back, Left) + Fill куб в центре
- 4 цвета вместо 8 (1 цвет = 1 сторона)
- Material Standard shader, цвет задаётся через material.color
- rotationStep учитывает вращение для логики матчинга
- FlashWhite меняет material.color на белый

### Tower.cs
- Генерирует 4 случайных цвета вместо 8
- Initialize(int[4]) вместо Initialize(int[8], Material)
- Материал больше не нужен

### StackChecker.cs
- DoLayersMatch проверяет 1 цвет на сторону (не 2 половинки)
- Совпадение = хотя бы одна сторона с одинаковым цветом

### SetupIteration1.cs
- Убрано создание материала и шейдера
- Всё работает на Standard shader

## Как настроить

1. Заменить файлы: BlockLayer.cs, Tower.cs, StackChecker.cs, Editor/SetupIteration1.cs
2. Можно удалить VertexColor.shader если был
3. Открыть Gameplay сцену
4. **STACK → Setup Gameplay Scene (Iteration 1)** — пересоздаст башню
5. **STACK → Setup Stack Mechanic (Iteration 4)** — пересоздаст StackChecker
6. **STACK → Setup Gameplay UI and Controls (Iteration 2)** — пересоздаст UI

## Тестирование

- Каждый слой = квадрат из 4 цветных стенок
- Вращение крутит transform, цвета остаются на своих кубах
- Если у двух соседних слоёв хотя бы 1 сторона совпадает по цвету — Confirm удаляет оба
