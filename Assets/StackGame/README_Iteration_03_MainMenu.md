# README — Iteration 3: MainMenu и переходы между сценами

## Что изменилось

### Новые скрипты:
- **SceneLoader.cs** — синглтон с DontDestroyOnLoad. Хранит выбранный режим (Levels/Endless). Загружает сцены с fade-переходом. Переподписывается на sceneLoaded при смене сцен. После загрузки сцены автоматически делает fade from black.
- **FadeOverlay.cs** — чёрный Image на отдельном Canvas (sortingOrder=100). FadeToBlack/FadeFromBlack через DOTween. Блокирует raycast во время fade.
- **MainMenuUI.cs** — анимация появления меню: заголовок scale bounce (OutBack), кнопки fade+slide снизу с задержками. Кнопки вызывают SceneLoader.
- **SetupIteration3.cs** — Editor скрипт с двумя кнопками.

## Как настроить

### 1. Создать сцену MainMenu:
1. File → New Scene → Save как `Assets/STACK/Scenes/MainMenu.unity`
2. Меню: **STACK → Setup MainMenu Scene (Iteration 3)**
3. Создаст: камеру с тёмно-синим фоном, MainMenuCanvas (заголовок STACK, кнопки LEVELS и ENDLESS), FadeOverlayCanvas, SceneLoader, EventSystem

### 2. Добавить FadeOverlay на Gameplay сцену:
1. Открыть Gameplay сцену
2. Меню: **STACK → Setup Gameplay Fade (Iteration 3)**
3. Добавит FadeOverlayCanvas + SceneLoader (если нет)

### 3. Build Settings:
1. File → Build Settings
2. Добавить обе сцены: `MainMenu` (index 0), `Gameplay` (index 1)

## Как тестировать

- Запустить MainMenu сцену
- Заголовок "STACK" появляется с bounce анимацией
- Кнопки LEVELS и ENDLESS плавно появляются снизу с fade
- При нажатии любой кнопки — экран плавно уходит в чёрный → загружается Gameplay → плавный fade из чёрного
- Кнопки имеют тактильный feedback (scale+color при нажатии)

## Ожидаемый результат

Полноценное главное меню с анимированным появлением и плавными переходами между сценами. Тёмно-синяя тема. SceneLoader сохраняет выбранный режим для использования в Gameplay.
