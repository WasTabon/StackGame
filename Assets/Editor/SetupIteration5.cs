using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration5 : Editor
{
    private static readonly Color PanelColor = new Color(0.06f, 0.07f, 0.18f, 0.95f);
    private static readonly Color BtnRetryColor = new Color(0.15f, 0.55f, 0.75f, 1f);
    private static readonly Color BtnMenuColor = new Color(0.55f, 0.25f, 0.80f, 1f);

    [MenuItem("STACK/Setup GameManager and GameOver (Iteration 5)")]
    public static void Setup()
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();
        Debug.Assert(tower != null, "Tower not found! Run Iteration 1 first.");

        StackChecker checker = Object.FindFirstObjectByType<StackChecker>();
        Debug.Assert(checker != null, "StackChecker not found! Run Iteration 4 first.");

        InputController input = Object.FindFirstObjectByType<InputController>();
        Debug.Assert(input != null, "InputController not found! Run Iteration 2 first.");

        Canvas mainCanvas = FindMainCanvas();
        Debug.Assert(mainCanvas != null, "Main Canvas not found! Run Iteration 2 first.");

        LayerSpawner spawner = SetupLayerSpawner(tower, checker);
        Text scoreText = CreateScoreUI(mainCanvas.transform);
        GameOverUI goUI = CreateGameOverUI(mainCanvas.transform);
        GameManager gm = SetupGameManager(tower, checker, input, goUI, scoreText);

        Debug.Log("[Iteration 5] GameManager, LayerSpawner, and GameOver UI setup complete.");
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

    private static LayerSpawner SetupLayerSpawner(Tower tower, StackChecker checker)
    {
        LayerSpawner spawner = Object.FindFirstObjectByType<LayerSpawner>();
        if (spawner == null)
        {
            GameObject obj = new GameObject("LayerSpawner");
            spawner = obj.AddComponent<LayerSpawner>();
            Undo.RegisterCreatedObjectUndo(obj, "Create LayerSpawner");
        }

        spawner.tower = tower;
        spawner.matchSide = checker.matchSide;
        tower.spawner = spawner;

        EditorUtility.SetDirty(spawner);
        EditorUtility.SetDirty(tower);
        return spawner;
    }

    private static GameManager SetupGameManager(Tower tower, StackChecker checker, InputController input, GameOverUI goUI, Text scoreText)
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm == null)
        {
            GameObject obj = new GameObject("GameManager");
            gm = obj.AddComponent<GameManager>();
            Undo.RegisterCreatedObjectUndo(obj, "Create GameManager");
        }

        gm.tower = tower;
        gm.stackChecker = checker;
        gm.inputController = input;
        gm.gameOverUI = goUI;
        gm.scoreText = scoreText;

        EditorUtility.SetDirty(gm);
        return gm;
    }

    private static Text CreateScoreUI(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("ScoreText");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject obj = new GameObject("ScoreText");
        obj.transform.SetParent(canvasTransform, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -40f);
        rect.sizeDelta = new Vector2(400f, 80f);

        Text text = obj.AddComponent<Text>();
        text.text = "0";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 56;
        text.color = new Color(0.85f, 0.90f, 1f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        EditorUtility.SetDirty(text);
        return text;
    }

    private static GameOverUI CreateGameOverUI(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("GameOverPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.6f);

        CanvasGroup cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        GameOverUI goUI = panelObj.AddComponent<GameOverUI>();
        goUI.panelGroup = cg;
        goUI.panelRect = panelRect;

        GameObject innerPanel = new GameObject("InnerPanel");
        innerPanel.transform.SetParent(panelObj.transform, false);
        RectTransform innerRect = innerPanel.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.pivot = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(600f, 500f);

        Image innerBg = innerPanel.AddComponent<Image>();
        innerBg.color = PanelColor;

        GameObject titleObj = new GameObject("GameOverTitle");
        titleObj.transform.SetParent(innerPanel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -30f);
        titleRect.sizeDelta = new Vector2(500f, 80f);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "GAME OVER";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 64;
        titleText.color = new Color(0.90f, 0.22f, 0.35f, 1f);
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        titleText.raycastTarget = false;

        GameObject scoreObj = new GameObject("FinalScore");
        scoreObj.transform.SetParent(innerPanel.transform, false);
        RectTransform scoreRect = scoreObj.AddComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.pivot = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = new Vector2(0f, 30f);
        scoreRect.sizeDelta = new Vector2(400f, 120f);

        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = "SCORE\n0";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 48;
        scoreText.color = new Color(0.85f, 0.90f, 1f, 1f);
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.raycastTarget = false;

        goUI.finalScoreText = scoreText;

        GameManager gm = Object.FindFirstObjectByType<GameManager>();

        CreateGameOverButton(innerPanel.transform, "RetryButton", "RETRY", BtnRetryColor,
            new Vector2(-130f, -170f), gm, "Retry");
        CreateGameOverButton(innerPanel.transform, "MenuButton", "MENU", BtnMenuColor,
            new Vector2(130f, -170f), gm, "GoToMenu");

        EditorUtility.SetDirty(goUI);
        return goUI;
    }

    private static void CreateGameOverButton(Transform parent, string name, string label, Color color,
        Vector2 position, GameManager gm, string methodName)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 80f);
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

        if (gm != null)
        {
            var method = typeof(GameManager).GetMethod(methodName);
            Debug.Assert(method != null, "Method " + methodName + " not found on GameManager!");
            UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), gm, method);
            btn.onClick = new Button.ButtonClickedEvent();
            UnityEventTools.AddPersistentListener(btn.onClick, action);
        }

        EditorUtility.SetDirty(btn);
    }
}
