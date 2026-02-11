using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration3 : Editor
{
    private static readonly Color BgColor = new Color(0.04f, 0.05f, 0.12f, 1f);
    private static readonly Color BtnLevelsColor = new Color(0.15f, 0.55f, 0.75f, 1f);
    private static readonly Color BtnEndlessColor = new Color(0.55f, 0.25f, 0.80f, 1f);
    private static readonly Color TitleColor = new Color(0.85f, 0.90f, 1f, 1f);
    private static readonly Color BtnTextColor = new Color(0.95f, 0.95f, 1f, 1f);

    [MenuItem("STACK/Setup MainMenu Scene (Iteration 3)")]
    public static void SetupMainMenu()
    {
        SetupMainMenuCamera();
        SceneLoader loader = SetupSceneLoader();
        MainMenuUI menuUI = SetupMainMenuCanvas();

        Debug.Log("[Iteration 3] MainMenu scene setup complete. Add 'MainMenu' and 'Gameplay' scenes to Build Settings!");
    }

    [MenuItem("STACK/Setup Gameplay Fade (Iteration 3)")]
    public static void SetupGameplayFade()
    {
        SetupFadeCanvas();
        SetupSceneLoader();
        Debug.Log("[Iteration 3] Gameplay fade overlay added.");
    }

    private static void SetupMainMenuCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
            Undo.RegisterCreatedObjectUndo(camObj, "Create MainMenu Camera");
        }
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = BgColor;
        mainCam.orthographic = true;
        EditorUtility.SetDirty(mainCam);
    }

    private static SceneLoader SetupSceneLoader()
    {
        SceneLoader loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader == null)
        {
            GameObject obj = new GameObject("SceneLoader");
            loader = obj.AddComponent<SceneLoader>();
            Undo.RegisterCreatedObjectUndo(obj, "Create SceneLoader");
        }
        EditorUtility.SetDirty(loader);
        return loader;
    }

    private static MainMenuUI SetupMainMenuCanvas()
    {
        Transform existingCanvas = null;
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            if (c.gameObject.name == "MainMenuCanvas")
            {
                existingCanvas = c.transform;
                break;
            }
        }

        GameObject canvasObj;
        if (existingCanvas != null)
        {
            canvasObj = existingCanvas.gameObject;
            ClearChildren(canvasObj.transform, "FadeOverlayCanvas");
        }
        else
        {
            canvasObj = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 5;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create MainMenuCanvas");
        }

        EnsureEventSystem();

        MainMenuUI menuUI = canvasObj.GetComponent<MainMenuUI>();
        if (menuUI == null)
            menuUI = canvasObj.AddComponent<MainMenuUI>();

        CreateTitle(canvasObj.transform, menuUI);
        CreateMenuButton(canvasObj.transform, menuUI, "LevelsButton", "LEVELS", BtnLevelsColor,
            new Vector2(0f, -100f), "OnLevelsPressed", true);
        CreateMenuButton(canvasObj.transform, menuUI, "EndlessButton", "ENDLESS", BtnEndlessColor,
            new Vector2(0f, -260f), "OnEndlessPressed", false);

        SetupFadeCanvas();

        EditorUtility.SetDirty(menuUI);
        return menuUI;
    }

    private static void CreateTitle(Transform parent, MainMenuUI menuUI)
    {
        Transform existing = parent.Find("Title");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject titleObj = CreateUIObject("Title", parent);
        RectTransform rect = titleObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600f, 180f);
        rect.anchoredPosition = new Vector2(0f, 250f);

        Text text = titleObj.AddComponent<Text>();
        text.text = "STACK";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 120;
        text.color = TitleColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        menuUI.titleRect = rect;
    }

    private static void CreateMenuButton(Transform parent, MainMenuUI menuUI, string name,
        string label, Color btnColor, Vector2 offset, string methodName, bool isLevels)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject btnObj = CreateUIObject(name, parent);
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(520f, 120f);
        rect.anchoredPosition = offset;

        Image img = btnObj.AddComponent<Image>();
        img.color = btnColor;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        cb.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        cb.fadeDuration = 0.06f;
        btn.colors = cb;

        btnObj.AddComponent<UIButtonFeedback>();

        CanvasGroup cg = btnObj.AddComponent<CanvasGroup>();

        GameObject textObj = CreateUIObject("Label", btnObj.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 48;
        text.color = BtnTextColor;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        var method = typeof(MainMenuUI).GetMethod(methodName);
        Debug.Assert(method != null, "Method " + methodName + " not found on MainMenuUI!");
        UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), menuUI, method);
        btn.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(btn.onClick, action);

        if (isLevels)
        {
            menuUI.levelsButtonRect = rect;
            menuUI.levelsCanvasGroup = cg;
        }
        else
        {
            menuUI.endlessButtonRect = rect;
            menuUI.endlessCanvasGroup = cg;
        }

        EditorUtility.SetDirty(btn);
    }

    private static void SetupFadeCanvas()
    {
        FadeOverlay existingFade = Object.FindFirstObjectByType<FadeOverlay>();
        if (existingFade != null)
            return;

        Transform existingFadeCanvas = null;
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in canvases)
        {
            if (c.gameObject.name == "FadeOverlayCanvas")
            {
                existingFadeCanvas = c.transform;
                break;
            }
        }

        GameObject fadeCanvasObj;
        if (existingFadeCanvas != null)
        {
            fadeCanvasObj = existingFadeCanvas.gameObject;
        }
        else
        {
            fadeCanvasObj = new GameObject("FadeOverlayCanvas");
            Canvas fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 100;

            CanvasScaler scaler = fadeCanvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            Undo.RegisterCreatedObjectUndo(fadeCanvasObj, "Create FadeOverlayCanvas");
        }

        Transform existingOverlay = fadeCanvasObj.transform.Find("FadeImage");
        if (existingOverlay == null)
        {
            GameObject fadeImgObj = CreateUIObject("FadeImage", fadeCanvasObj.transform);
            RectTransform rect = fadeImgObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image img = fadeImgObj.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0f);
            img.raycastTarget = false;

            fadeImgObj.AddComponent<FadeOverlay>();
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

    private static void ClearChildren(Transform parent, string excludeName)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.gameObject.name != excludeName)
                Object.DestroyImmediate(child.gameObject);
        }
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }
}
