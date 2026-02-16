using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration10 : Editor
{
    private static readonly Color PanelColor = new Color(0.06f, 0.07f, 0.18f, 0.95f);
    private static readonly Color BuyColor = new Color(0.25f, 0.82f, 0.50f, 1f);
    private static readonly Color CloseColor = new Color(0.55f, 0.25f, 0.80f, 1f);

    [MenuItem("STACK/Setup IAP and Shop (Iteration 10) - Gameplay Scene")]
    public static void SetupGameplay()
    {
        SetupIAPManager();

        BonusManager bm = Object.FindFirstObjectByType<BonusManager>();
        Debug.Assert(bm != null, "BonusManager not found! Run Iteration 8 first.");

        Canvas mainCanvas = FindMainCanvas();
        Debug.Assert(mainCanvas != null, "Main Canvas not found!");

        ShopUI shopUI = CreateShopUI(mainCanvas.transform, bm);
        CreateShopButton(mainCanvas.transform, shopUI);

        Debug.Log("[Iteration 10] IAP and Shop setup in Gameplay scene.");
    }

    [MenuItem("STACK/Setup IAP and Shop (Iteration 10) - MainMenu Scene")]
    public static void SetupMainMenu()
    {
        SetupIAPManager();

        MainMenuUI mmui = Object.FindFirstObjectByType<MainMenuUI>();
        Debug.Assert(mmui != null, "MainMenuUI not found! Open MainMenu scene first.");

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        ShopUI shopUI = CreateShopUI(canvas.transform, null);
        mmui.shopUI = shopUI;

        CreateMainMenuShopButton(canvas.transform, mmui);

        EditorUtility.SetDirty(mmui);
        Debug.Log("[Iteration 10] IAP and Shop setup in MainMenu scene.");
    }

    private static Canvas FindMainCanvas()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            if (c.gameObject.name != "FadeOverlayCanvas")
                return c;
        }
        return null;
    }

    private static void SetupIAPManager()
    {
        IAPManager existing = Object.FindFirstObjectByType<IAPManager>();
        if (existing != null) return;

        GameObject obj = new GameObject("IAPManager");
        obj.AddComponent<IAPManager>();
        Undo.RegisterCreatedObjectUndo(obj, "Create IAPManager");
    }

    private static ShopUI CreateShopUI(Transform canvasTransform, BonusManager bm)
    {
        Transform existing = canvasTransform.Find("ShopPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panelObj = new GameObject("ShopPanel");
        panelObj.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.7f);

        CanvasGroup cg = panelObj.AddComponent<CanvasGroup>();
        ShopUI shopUI = panelObj.AddComponent<ShopUI>();
        shopUI.panelGroup = cg;
        shopUI.panelRect = panelRect;
        shopUI.bonusManager = bm;

        GameObject inner = new GameObject("InnerPanel");
        inner.transform.SetParent(panelObj.transform, false);
        RectTransform innerRect = inner.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(550f, 500f);
        Image innerBg = inner.AddComponent<Image>();
        innerBg.color = PanelColor;

        CreateText(inner.transform, "Title", "SHOP",
            new Vector2(0f, -25f), new Vector2(400f, 70f), 52,
            new Color(0.85f, 0.90f, 1f, 1f), new Vector2(0.5f, 1f));

        CreateText(inner.transform, "Description", "Bonus Pack\n<size=28>+1 Swap   +1 Destroy   +1 Shuffle</size>",
            new Vector2(0f, 30f), new Vector2(450f, 120f), 34,
            new Color(0.70f, 0.75f, 0.90f, 1f), new Vector2(0.5f, 0.5f));

        GameObject priceObj = CreateText(inner.transform, "Price", "$0.99",
            new Vector2(0f, -50f), new Vector2(200f, 50f), 28,
            new Color(0.55f, 0.60f, 0.75f, 0.9f), new Vector2(0.5f, 0.5f));
        shopUI.priceText = priceObj.GetComponent<Text>();

        GameObject buyBtn = CreateButton(inner.transform, "BuyButton", "BUY", BuyColor,
            new Vector2(0f, -130f), new Vector2(280f, 80f));
        Button buyButton = buyBtn.GetComponent<Button>();
        UnityAction buyAction = new UnityAction(shopUI.OnBuyPressed);
        buyButton.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(buyButton.onClick, buyAction);
        shopUI.buyButton = buyButton;

        GameObject statusObj = CreateText(inner.transform, "Status", "",
            new Vector2(0f, -190f), new Vector2(450f, 40f), 24,
            new Color(0.85f, 0.90f, 1f, 1f), new Vector2(0.5f, 0.5f));
        shopUI.statusText = statusObj.GetComponent<Text>();

        GameObject closeBtn = CreateButton(inner.transform, "CloseButton", "CLOSE", CloseColor,
            new Vector2(0f, -240f), new Vector2(200f, 60f));
        Button closeButton = closeBtn.GetComponent<Button>();
        UnityAction closeAction = new UnityAction(shopUI.Hide);
        closeButton.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(closeButton.onClick, closeAction);

        EditorUtility.SetDirty(shopUI);
        return shopUI;
    }

    private static void CreateShopButton(Transform canvasTransform, ShopUI shopUI)
    {
        Transform existing = canvasTransform.Find("ShopButton");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject btnObj = new GameObject("ShopButton");
        btnObj.transform.SetParent(canvasTransform, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(20f, 0f);
        rect.sizeDelta = new Vector2(80f, 80f);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.82f, 0.50f, 0.9f);

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;
        btnObj.AddComponent<UIButtonFeedback>();

        UnityAction action = new UnityAction(shopUI.Show);
        btn.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(btn.onClick, action);

        GameObject label = new GameObject("Label");
        label.transform.SetParent(btnObj.transform, false);
        RectTransform lr = label.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.sizeDelta = Vector2.zero;
        Text lt = label.AddComponent<Text>();
        lt.text = "SHOP";
        lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lt.fontSize = 20;
        lt.color = Color.white;
        lt.alignment = TextAnchor.MiddleCenter;
        lt.fontStyle = FontStyle.Bold;
        lt.raycastTarget = false;

        EditorUtility.SetDirty(btn);
    }

    private static void CreateMainMenuShopButton(Transform canvasTransform, MainMenuUI mmui)
    {
        Transform existing = canvasTransform.Find("ShopMenuButton");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject btnObj = new GameObject("ShopMenuButton");
        btnObj.transform.SetParent(canvasTransform, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 100f);
        rect.sizeDelta = new Vector2(250f, 70f);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.82f, 0.50f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;

        CanvasGroup btnCg = btnObj.AddComponent<CanvasGroup>();
        btnObj.AddComponent<UIButtonFeedback>();

        UnityAction action = new UnityAction(mmui.OnShopPressed);
        btn.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(btn.onClick, action);

        GameObject label = new GameObject("Label");
        label.transform.SetParent(btnObj.transform, false);
        RectTransform lr = label.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.sizeDelta = Vector2.zero;
        Text lt = label.AddComponent<Text>();
        lt.text = "SHOP";
        lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lt.fontSize = 36;
        lt.color = Color.white;
        lt.alignment = TextAnchor.MiddleCenter;
        lt.fontStyle = FontStyle.Bold;
        lt.raycastTarget = false;

        EditorUtility.SetDirty(btn);
    }

    private static GameObject CreateText(Transform parent, string name, string content,
        Vector2 position, Vector2 size, int fontSize, Color color, Vector2 anchor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.supportRichText = true;
        text.raycastTarget = false;

        return obj;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Color color,
        Vector2 position, Vector2 size)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;
        btnObj.AddComponent<UIButtonFeedback>();

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 36;
        text.color = new Color(0.95f, 0.95f, 1f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        EditorUtility.SetDirty(btn);
        return btnObj;
    }
}
