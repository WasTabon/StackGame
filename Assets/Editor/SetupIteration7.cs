using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration7 : Editor
{
    private static readonly Color PanelColor = new Color(0.06f, 0.07f, 0.18f, 0.95f);
    private static readonly Color BtnNextColor = new Color(0.25f, 0.82f, 0.50f, 1f);
    private static readonly Color BtnRetryColor = new Color(0.15f, 0.55f, 0.75f, 1f);
    private static readonly Color BtnMenuColor = new Color(0.55f, 0.25f, 0.80f, 1f);
    private static readonly Color BtnLevelColor = new Color(0.15f, 0.55f, 0.75f, 1f);
    private static readonly Color LockedColor = new Color(0.15f, 0.16f, 0.25f, 1f);

    [MenuItem("STACK/Setup Game Modes (Iteration 7) - Gameplay Scene")]
    public static void SetupGameplay()
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        Debug.Assert(gm != null, "GameManager not found! Run Iteration 5 first.");

        Tower tower = Object.FindFirstObjectByType<Tower>();
        Canvas mainCanvas = FindMainCanvas();

        LevelData[] levels = CreateDefaultLevels();
        LevelManager lm = SetupLevelManager(gm, tower, mainCanvas, levels);
        EndlessManager em = SetupEndlessManager(gm);
        LevelCompleteUI lcUI = CreateLevelCompleteUI(mainCanvas.transform, gm);

        gm.levelManager = lm;
        gm.endlessManager = em;
        gm.levelCompleteUI = lcUI;
        EditorUtility.SetDirty(gm);

        CreateGoalUI(mainCanvas.transform, lm);

        Debug.Log("[Iteration 7] Game modes setup in Gameplay scene. " + levels.Length + " levels created.");
    }

    [MenuItem("STACK/Setup Level Select (Iteration 7) - MainMenu Scene")]
    public static void SetupMainMenu()
    {
        MainMenuUI mmui = Object.FindFirstObjectByType<MainMenuUI>();
        Debug.Assert(mmui != null, "MainMenuUI not found! Open MainMenu scene first.");

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        LevelSelectUI lsUI = CreateLevelSelectUI(canvas.transform);
        mmui.levelSelectUI = lsUI;

        CreateHighScoreText(canvas.transform, mmui);

        EditorUtility.SetDirty(mmui);
        Debug.Log("[Iteration 7] Level select setup in MainMenu scene.");
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

    private static LevelData[] CreateDefaultLevels()
    {
        string folder = "Assets/STACK/Data";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            if (!AssetDatabase.IsValidFolder("Assets/STACK"))
                AssetDatabase.CreateFolder("Assets", "STACK");
            AssetDatabase.CreateFolder("Assets/STACK", "Data");
        }

        var configs = new (LevelData.GoalType type, int value, float spawn, int max, int start, int colors)[]
        {
            (LevelData.GoalType.RemoveLayers, 4, 12f, 10, 6, 3),
            (LevelData.GoalType.RemoveLayers, 6, 10f, 10, 6, 4),
            (LevelData.GoalType.ReachScore, 1000, 10f, 10, 6, 4),
            (LevelData.GoalType.ChainReaction, 2, 10f, 10, 6, 4),
            (LevelData.GoalType.RemoveLayers, 10, 8f, 10, 6, 5),
            (LevelData.GoalType.SurviveTime, 60, 7f, 8, 6, 5),
            (LevelData.GoalType.ReachScore, 3000, 7f, 9, 6, 5),
            (LevelData.GoalType.ChainReaction, 3, 8f, 10, 7, 5),
            (LevelData.GoalType.RemoveLayers, 15, 6f, 8, 7, 5),
            (LevelData.GoalType.ReachScore, 5000, 5f, 8, 7, 5),
        };

        LevelData[] levels = new LevelData[configs.Length];
        for (int i = 0; i < configs.Length; i++)
        {
            string path = folder + "/Level_" + (i + 1).ToString("D2") + ".asset";
            LevelData existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (existing != null)
            {
                levels[i] = existing;
                continue;
            }

            LevelData ld = ScriptableObject.CreateInstance<LevelData>();
            ld.levelNumber = i + 1;
            ld.goalType = configs[i].type;
            ld.goalValue = configs[i].value;
            ld.spawnInterval = configs[i].spawn;
            ld.maxLayers = configs[i].max;
            ld.startingLayers = configs[i].start;
            ld.colorCount = configs[i].colors;

            AssetDatabase.CreateAsset(ld, path);
            levels[i] = ld;
        }

        AssetDatabase.SaveAssets();
        return levels;
    }

    private static LevelManager SetupLevelManager(GameManager gm, Tower tower, Canvas canvas, LevelData[] levels)
    {
        LevelManager lm = Object.FindFirstObjectByType<LevelManager>();
        if (lm == null)
        {
            GameObject obj = new GameObject("LevelManager");
            lm = obj.AddComponent<LevelManager>();
            Undo.RegisterCreatedObjectUndo(obj, "Create LevelManager");
        }

        lm.gameManager = gm;
        lm.tower = tower;
        lm.levels = levels;
        EditorUtility.SetDirty(lm);
        return lm;
    }

    private static EndlessManager SetupEndlessManager(GameManager gm)
    {
        EndlessManager em = Object.FindFirstObjectByType<EndlessManager>();
        if (em == null)
        {
            GameObject obj = new GameObject("EndlessManager");
            em = obj.AddComponent<EndlessManager>();
            Undo.RegisterCreatedObjectUndo(obj, "Create EndlessManager");
        }

        em.gameManager = gm;
        EditorUtility.SetDirty(em);
        return em;
    }

    private static void CreateGoalUI(Transform canvasTransform, LevelManager lm)
    {
        Transform existing = canvasTransform.Find("GoalPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panelObj = new GameObject("GoalPanel");
        panelObj.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(500f, 200f);

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = PanelColor;
        panelObj.SetActive(false);

        GameObject goalTextObj = new GameObject("GoalText");
        goalTextObj.transform.SetParent(panelObj.transform, false);
        RectTransform gtRect = goalTextObj.AddComponent<RectTransform>();
        gtRect.anchorMin = Vector2.zero;
        gtRect.anchorMax = Vector2.one;
        gtRect.sizeDelta = Vector2.zero;

        Text goalText = goalTextObj.AddComponent<Text>();
        goalText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        goalText.fontSize = 36;
        goalText.color = new Color(0.85f, 0.90f, 1f, 1f);
        goalText.alignment = TextAnchor.MiddleCenter;
        goalText.fontStyle = FontStyle.Bold;
        goalText.raycastTarget = false;

        lm.goalPanel = panelRect;
        lm.goalText = goalText;

        Transform existingProgress = canvasTransform.Find("ProgressText");
        if (existingProgress != null)
            Object.DestroyImmediate(existingProgress.gameObject);

        GameObject progressObj = new GameObject("ProgressText");
        progressObj.transform.SetParent(canvasTransform, false);
        RectTransform progRect = progressObj.AddComponent<RectTransform>();
        progRect.anchorMin = new Vector2(0.5f, 1f);
        progRect.anchorMax = new Vector2(0.5f, 1f);
        progRect.pivot = new Vector2(0.5f, 1f);
        progRect.anchoredPosition = new Vector2(0f, -120f);
        progRect.sizeDelta = new Vector2(400f, 50f);

        Text progressText = progressObj.AddComponent<Text>();
        progressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        progressText.fontSize = 28;
        progressText.color = new Color(0.65f, 0.70f, 0.85f, 0.9f);
        progressText.alignment = TextAnchor.MiddleCenter;
        progressText.raycastTarget = false;

        lm.progressText = progressText;
        EditorUtility.SetDirty(lm);
    }

    private static LevelCompleteUI CreateLevelCompleteUI(Transform canvasTransform, GameManager gm)
    {
        Transform existing = canvasTransform.Find("LevelCompletePanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panelObj = new GameObject("LevelCompletePanel");
        panelObj.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.6f);

        CanvasGroup cg = panelObj.AddComponent<CanvasGroup>();
        LevelCompleteUI lcUI = panelObj.AddComponent<LevelCompleteUI>();
        lcUI.panelGroup = cg;
        lcUI.panelRect = panelRect;

        GameObject inner = new GameObject("InnerPanel");
        inner.transform.SetParent(panelObj.transform, false);
        RectTransform innerRect = inner.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(600f, 500f);
        Image innerBg = inner.AddComponent<Image>();
        innerBg.color = PanelColor;

        GameObject titleObj = CreateUIText(inner.transform, "Title", "LEVEL COMPLETE!",
            new Vector2(0f, -30f), new Vector2(500f, 80f), 56,
            new Color(0.25f, 0.82f, 0.50f, 1f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1f));
        lcUI.titleText = titleObj.GetComponent<Text>();

        GameObject scoreObj = CreateUIText(inner.transform, "Score", "SCORE\n0",
            new Vector2(0f, 30f), new Vector2(400f, 120f), 48,
            new Color(0.85f, 0.90f, 1f, 1f), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f));
        lcUI.scoreText = scoreObj.GetComponent<Text>();

        GameObject nextBtn = CreateButton(inner.transform, "NextButton", "NEXT", BtnNextColor,
            new Vector2(0f, -120f), new Vector2(220f, 80f), gm, "NextLevel");
        lcUI.nextButton = nextBtn;

        CreateButton(inner.transform, "RetryButton", "RETRY", BtnRetryColor,
            new Vector2(-130f, -210f), new Vector2(220f, 70f), gm, "Retry");
        CreateButton(inner.transform, "MenuButton", "MENU", BtnMenuColor,
            new Vector2(130f, -210f), new Vector2(220f, 70f), gm, "GoToMenu");

        EditorUtility.SetDirty(lcUI);
        return lcUI;
    }

    private static LevelSelectUI CreateLevelSelectUI(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("LevelSelectPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panelObj = new GameObject("LevelSelectPanel");
        panelObj.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelBg = panelObj.AddComponent<Image>();
        panelBg.color = new Color(0f, 0f, 0f, 0.7f);

        CanvasGroup cg = panelObj.AddComponent<CanvasGroup>();
        LevelSelectUI lsUI = panelObj.AddComponent<LevelSelectUI>();
        lsUI.panelGroup = cg;
        lsUI.panelRect = panelRect;

        GameObject inner = new GameObject("InnerPanel");
        inner.transform.SetParent(panelObj.transform, false);
        RectTransform innerRect = inner.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(650f, 700f);
        Image innerBg = inner.AddComponent<Image>();
        innerBg.color = PanelColor;

        CreateUIText(inner.transform, "Title", "SELECT LEVEL",
            new Vector2(0f, -30f), new Vector2(500f, 70f), 48,
            new Color(0.85f, 0.90f, 1f, 1f), TextAnchor.MiddleCenter, new Vector2(0.5f, 1f));

        GameObject grid = new GameObject("ButtonGrid");
        grid.transform.SetParent(inner.transform, false);
        RectTransform gridRect = grid.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.sizeDelta = new Vector2(500f, 400f);
        gridRect.anchoredPosition = new Vector2(0f, -20f);

        GridLayoutGroup glg = grid.AddComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(100f, 100f);
        glg.spacing = new Vector2(15f, 15f);
        glg.childAlignment = TextAnchor.MiddleCenter;
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 4;

        lsUI.buttonContainer = grid.transform;

        for (int i = 0; i < 10; i++)
        {
            GameObject btnObj = new GameObject("Level_" + (i + 1));
            btnObj.transform.SetParent(grid.transform, false);
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = i == 0 ? BtnLevelColor : LockedColor;

            Button btn = btnObj.AddComponent<Button>();
            btn.interactable = i == 0;
            btnObj.AddComponent<UIButtonFeedback>();

            GameObject txtObj = new GameObject("Label");
            txtObj.transform.SetParent(btnObj.transform, false);
            RectTransform txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.sizeDelta = Vector2.zero;

            Text txt = txtObj.AddComponent<Text>();
            txt.text = (i + 1).ToString();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 40;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontStyle = FontStyle.Bold;
            txt.raycastTarget = false;
        }

        GameObject backBtn = new GameObject("BackButton");
        backBtn.transform.SetParent(inner.transform, false);
        RectTransform backRect = backBtn.AddComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0f);
        backRect.anchorMax = new Vector2(0.5f, 0f);
        backRect.pivot = new Vector2(0.5f, 0f);
        backRect.anchoredPosition = new Vector2(0f, 20f);
        backRect.sizeDelta = new Vector2(200f, 60f);

        Image backImg = backBtn.AddComponent<Image>();
        backImg.color = BtnMenuColor;

        Button backButton = backBtn.AddComponent<Button>();
        backBtn.AddComponent<UIButtonFeedback>();
        UnityAction backAction = new UnityAction(lsUI.OnBackPressed);
        backButton.onClick = new Button.ButtonClickedEvent();
        UnityEventTools.AddPersistentListener(backButton.onClick, backAction);

        lsUI.backButton = backBtn;

        GameObject backTxt = new GameObject("Label");
        backTxt.transform.SetParent(backBtn.transform, false);
        RectTransform btRect = backTxt.AddComponent<RectTransform>();
        btRect.anchorMin = Vector2.zero;
        btRect.anchorMax = Vector2.one;
        btRect.sizeDelta = Vector2.zero;
        Text bt = backTxt.AddComponent<Text>();
        bt.text = "BACK";
        bt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        bt.fontSize = 30;
        bt.color = Color.white;
        bt.alignment = TextAnchor.MiddleCenter;
        bt.fontStyle = FontStyle.Bold;
        bt.raycastTarget = false;

        EditorUtility.SetDirty(lsUI);
        return lsUI;
    }

    private static void CreateHighScoreText(Transform canvasTransform, MainMenuUI mmui)
    {
        Transform existing = canvasTransform.Find("HighScoreText");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject obj = new GameObject("HighScoreText");
        obj.transform.SetParent(canvasTransform, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 40f);
        rect.sizeDelta = new Vector2(400f, 50f);

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.color = new Color(0.65f, 0.70f, 0.85f, 0.8f);
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        mmui.highScoreText = text;
        EditorUtility.SetDirty(mmui);
    }

    private static GameObject CreateUIText(Transform parent, string name, string content,
        Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor anchor, Vector2 pivotAnchor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = pivotAnchor;
        rect.anchorMax = pivotAnchor;
        rect.pivot = pivotAnchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = obj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = anchor;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        return obj;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Color color,
        Vector2 position, Vector2 size, GameManager gm, string methodName)
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
        text.fontSize = 32;
        text.color = new Color(0.95f, 0.95f, 1f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;

        if (gm != null)
        {
            var method = typeof(GameManager).GetMethod(methodName);
            if (method != null)
            {
                UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), gm, method);
                btn.onClick = new Button.ButtonClickedEvent();
                UnityEventTools.AddPersistentListener(btn.onClick, action);
            }
        }

        EditorUtility.SetDirty(btn);
        return btnObj;
    }
}
