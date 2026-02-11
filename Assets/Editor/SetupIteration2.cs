using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration2 : Editor
{
    private static readonly Color PanelBg = new Color(0.04f, 0.05f, 0.12f, 0.92f);
    private static readonly Color BtnColor = new Color(0.10f, 0.13f, 0.28f, 1f);
    private static readonly Color BtnConfirmColor = new Color(0.15f, 0.65f, 0.55f, 1f);
    private static readonly Color TextColor = new Color(0.82f, 0.85f, 0.95f, 1f);
    private static readonly Color IndicatorBg = new Color(0.06f, 0.08f, 0.18f, 0.7f);
    private static readonly Color DimTextColor = new Color(0.5f, 0.55f, 0.7f, 1f);

    [MenuItem("STACK/Setup Gameplay UI and Controls (Iteration 2)")]
    public static void Setup()
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();
        Debug.Assert(tower != null, "Tower not found! Run Iteration 1 setup first.");

        CameraController cam = Object.FindFirstObjectByType<CameraController>();
        Debug.Assert(cam != null, "CameraController not found! Run Iteration 1 setup first.");

        InputController input = SetupInputController(tower, cam);
        SetupCanvas(input);

        Debug.Log("[Iteration 2] Gameplay UI and Controls setup complete.");
    }

    private static InputController SetupInputController(Tower tower, CameraController cam)
    {
        InputController input = Object.FindFirstObjectByType<InputController>();
        if (input == null)
        {
            GameObject obj = new GameObject("InputController");
            input = obj.AddComponent<InputController>();
            Undo.RegisterCreatedObjectUndo(obj, "Create InputController");
        }
        input.tower = tower;
        input.cameraController = cam;
        EditorUtility.SetDirty(input);
        return input;
    }

    private static void SetupCanvas(InputController input)
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        GameObject canvasObj;

        if (canvas == null)
        {
            canvasObj = new GameObject("GameplayCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
        }
        else
        {
            canvasObj = canvas.gameObject;
        }

        EnsureEventSystem();
        SetupControlPanel(canvasObj.transform, input);
        SetupLayerIndicator(canvasObj.transform);

        Transform indicatorText = canvasObj.transform.Find("LayerIndicator/Text");
        if (indicatorText != null)
        {
            input.layerIndicatorText = indicatorText.GetComponent<Text>();
            EditorUtility.SetDirty(input);
        }
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
        }
    }

    private static void SetupControlPanel(Transform canvasT, InputController input)
    {
        Transform existing = canvasT.Find("ControlPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        float panelHeight = 360f;
        float bottomPadding = 40f;
        float sidePadding = 60f;
        float btnSize = 130f;
        float btnGap = 20f;
        float confirmSize = 160f;

        GameObject panel = CreateUIObject("ControlPanel", canvasT);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.sizeDelta = new Vector2(0f, panelHeight + bottomPadding);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = PanelBg;

        float centerY = bottomPadding + (panelHeight * 0.5f);

        GameObject upBtn = CreateArrowButton("UpButton", panel.transform, "▲", btnSize,
            new Vector2(sidePadding + btnSize * 0.5f, centerY + btnSize * 0.5f + btnGap * 0.5f),
            false);
        GameObject downBtn = CreateArrowButton("DownButton", panel.transform, "▼", btnSize,
            new Vector2(sidePadding + btnSize * 0.5f, centerY - btnSize * 0.5f - btnGap * 0.5f),
            false);

        GameObject leftBtn = CreateArrowButton("LeftRotateButton", panel.transform, "◄", btnSize,
            new Vector2(-(sidePadding + btnSize + btnGap * 0.5f), centerY),
            true);
        GameObject rightBtn = CreateArrowButton("RightRotateButton", panel.transform, "►", btnSize,
            new Vector2(-(sidePadding + btnGap * 0.5f), centerY),
            true);

        GameObject confirmBtn = CreateConfirmButton("ConfirmButton", panel.transform, confirmSize,
            new Vector2(0f, centerY));

        GameObject navLabel = CreateLabel("NavLabel", panel.transform, "SELECT",
            new Vector2(sidePadding + btnSize * 0.5f, centerY - btnSize - btnGap * 0.5f - 20f), false);
        GameObject rotLabel = CreateLabel("RotLabel", panel.transform, "ROTATE",
            new Vector2(-(sidePadding + btnSize * 0.5f + btnGap * 0.25f), centerY - btnSize * 0.5f - btnGap - 20f), true);

        WireButton(upBtn, input, "OnMoveUp");
        WireButton(downBtn, input, "OnMoveDown");
        WireButton(leftBtn, input, "OnRotateLeft");
        WireButton(rightBtn, input, "OnRotateRight");
        WireButton(confirmBtn, input, "OnConfirmPressed");
    }

    private static GameObject CreateArrowButton(string name, Transform parent, string arrow, float size, Vector2 pos, bool anchorRight)
    {
        GameObject obj = CreateUIObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();

        if (anchorRight)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
        }
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = pos;

        Image img = obj.AddComponent<Image>();
        img.color = BtnColor;

        Button btn = obj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        cb.pressedColor = new Color(0.65f, 0.65f, 0.65f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;

        obj.AddComponent<UIButtonFeedback>();

        GameObject textObj = CreateUIObject("Label", obj.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = arrow;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 52;
        text.color = TextColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        return obj;
    }

    private static GameObject CreateConfirmButton(string name, Transform parent, float size, Vector2 pos)
    {
        GameObject obj = CreateUIObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = pos;

        Image img = obj.AddComponent<Image>();
        img.color = BtnConfirmColor;

        Button btn = obj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;

        obj.AddComponent<UIButtonFeedback>();

        GameObject textObj = CreateUIObject("Label", obj.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "✓";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 80;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        return obj;
    }

    private static GameObject CreateLabel(string name, Transform parent, string labelText, Vector2 pos, bool anchorRight)
    {
        GameObject obj = CreateUIObject(name, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();

        if (anchorRight)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
        }
        else
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
        }
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(200f, 30f);
        rect.anchoredPosition = pos;

        Text text = obj.AddComponent<Text>();
        text.text = labelText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 22;
        text.color = DimTextColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        return obj;
    }

    private static void SetupLayerIndicator(Transform canvasT)
    {
        Transform existing = canvasT.Find("LayerIndicator");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject indicator = CreateUIObject("LayerIndicator", canvasT);
        RectTransform rect = indicator.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(260f, 70f);
        rect.anchoredPosition = new Vector2(0f, -80f);

        Image bg = indicator.AddComponent<Image>();
        bg.color = IndicatorBg;

        GameObject textObj = CreateUIObject("Text", indicator.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = "Layer 1 / 6";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.color = TextColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
    }

    private static void WireButton(GameObject btnObj, InputController input, string methodName)
    {
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick = new Button.ButtonClickedEvent();

        var method = typeof(InputController).GetMethod(methodName);
        Debug.Assert(method != null, "Method " + methodName + " not found on InputController!");

        UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), input, method);
        UnityEventTools.AddPersistentListener(btn.onClick, action);
        EditorUtility.SetDirty(btn);
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }
}
