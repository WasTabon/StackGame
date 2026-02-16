# README — Iteration 9: Polish (SFX, Screen Shake, Score Animation, Tutorial)

## Новые скрипты

### SFXManager.cs (синглтон, DontDestroyOnLoad)
Процедурный звук — не нужны внешние аудио файлы. Генерирует звуки через AudioClip.Create:
- **PlayRotate** — короткий тон при вращении
- **PlaySelect** — тик при смене слоя
- **PlayConfirm** — высокий тон при подтверждении
- **PlayMatch** — аккорд при совпадении
- **PlayChain(step)** — нарастающий аккорд для цепных реакций
- **PlaySpawn** — восходящий свип при спавне нового слоя
- **PlayDrop** — нисходящий свип при падении
- **PlayGameOver** — низкий нисходящий аккорд
- **PlayLevelComplete** — арпеджио вверх
- **PlayBonus** — аккорд при использовании бонуса
- **PlayCancel** — нисходящий свип при отмене

### ScreenShake.cs
Тряска камеры при матчах. Интенсивность масштабируется по chainStep:
- chain x1 — лёгкая тряска
- chain x2 — средняя
- chain x3+ — сильная

### ScoreAnimator.cs
Анимированный счётчик очков: число плавно "накручивается" до нового значения + punch scale при обновлении.

### TutorialManager.cs
Пошаговый туториал при первом запуске (6 шагов):
1. Приветствие
2. Вращение (LEFT/RIGHT)
3. Выбор слоя (UP/DOWN)
4. Матчинг (CONFIRM)
5. Цепные реакции и бонусы
6. Спавн и game over

Показывается один раз, сохраняет флаг через PlayerPrefs. Overlay с затемнением + панель с текстом + "Tap to continue".

### Изменённые скрипты
- **GameManager.cs** — интеграция ScreenShake, ScoreAnimator, SFX вызовы
- **InputController.cs** — SFX вызовы при вращении, выборе, подтверждении, бонусах, отмене

## Как настроить

1. Заменить: GameManager.cs, InputController.cs
2. Добавить: SFXManager.cs, ScreenShake.cs, ScoreAnimator.cs, TutorialManager.cs, Editor/SetupIteration9.cs
3. Открыть Gameplay сцену
4. **STACK → Setup Polish and SFX (Iteration 9) - Gameplay Scene**

## Заметки
- SFXManager — синглтон с DontDestroyOnLoad, работает между сценами
- Для сброса туториала: вызвать TutorialManager.ResetTutorial() или удалить PlayerPrefs ключ "TutorialSeen"
- ScoreAnimator подхватывает scoreText из GameManager, заменяет прямое обновление
