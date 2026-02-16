# README — Iteration 10: IAP (Unity Purchasing)

## Новые скрипты

### IAPManager.cs (синглтон, DontDestroyOnLoad)
- Один consumable продукт: `com.stack.bonuspack` — даёт +1 Swap, +1 Destroy, +1 Shuffle
- Работает через Unity IAP для iOS (Apple App Store)
- Если Unity Purchasing не подключён — симулирует покупку (для тестирования в Editor)
- Используёт `#if UNITY_PURCHASING` — компилируется без ошибок с и без плагина
- Получает локализованную цену из Store

### ShopUI.cs
- Панель магазина: название, описание, цена, кнопка BUY, статус, кнопка CLOSE
- При покупке из Gameplay — сразу добавляет бонусы через BonusManager.AddBonuses
- При покупке из MainMenu — сохраняет в PlayerPrefs ("PendingBonuses"), BonusManager забирает при старте игры
- Анимированный feedback: панель пульсирует при успехе, текст меняет цвет

### Изменённые скрипты
- **BonusManager.cs** — ClaimPendingBonuses() в Start читает PlayerPrefs и добавляет купленные бонусы
- **MainMenuUI.cs** — добавлен OnShopPressed() и ссылка на ShopUI

## Как подключить Unity IAP

1. Window → Package Manager → Unity IAP → Install
2. Services → In-App Purchasing → Enable
3. Настроить продукт `com.stack.bonuspack` (Consumable) в App Store Connect
4. IAPManager автоматически подхватит через `#if UNITY_PURCHASING`

**Без Unity IAP**: всё работает в Editor — покупка симулируется, бонусы добавляются.

## Как настроить

### Gameplay сцена:
1. Заменить: BonusManager.cs
2. Добавить: IAPManager.cs, ShopUI.cs, Editor/SetupIteration10.cs
3. **STACK → Setup IAP and Shop (Iteration 10) - Gameplay Scene**
4. Кнопка SHOP появится слева по центру экрана

### MainMenu сцена:
1. Обновить: MainMenuUI.cs
2. Переключиться на MainMenu
3. **STACK → Setup IAP and Shop (Iteration 10) - MainMenu Scene**
4. Кнопка SHOP появится внизу экрана
