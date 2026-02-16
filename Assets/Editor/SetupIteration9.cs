using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupIteration9 : Editor
{
    private static readonly Color PanelColor = new Color(0.06f, 0.07f, 0.18f, 0.92f);

    [MenuItem("STACK/Setup Polish and SFX (Iteration 9) - Gameplay Scene")]
    public static void SetupGameplay()
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        Debug.Assert(gm != null, "GameManager not found!");

        InputController input = Object.FindFirstObjectByType<InputController>();
        Canvas mainCanvas = FindMainCanvas();
        Camera cam = Camera.main;

        SetupSFXManager();
        ScreenShake shake = SetupScreenShake(cam);
        ScoreAnimator sa = SetupScoreAnimator(gm);
        TutorialManager tutorial = SetupTutorial(mainCanvas.transform, input, gm);

        gm.screenShake = shake;
        gm.scoreAnimator = sa;
        EditorUtility.SetDirty(gm);

        Debug.Log("[Iteration 9] Polish setup complete.");
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

    private static void SetupSFXManager()
    {
        SFXManager existing = Object.FindFirstObjectByType<SFXManager>();
        if (existing != null) return;

        GameObject obj = new GameObject("SFXManager");
        obj.AddComponent<SFXManager>();
        Undo.RegisterCreatedObjectUndo(obj, "Create SFXManager");
    }

    private static ScreenShake SetupScreenShake(Camera cam)
    {
        ScreenShake existing = Object.FindFirstObjectByType<ScreenShake>();
        if (existing != null)
        {
            existing.cam = cam;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        GameObject obj = new GameObject("ScreenShake");
        ScreenShake shake = obj.AddComponent<ScreenShake>();
        shake.cam = cam;
        Undo.RegisterCreatedObjectUndo(obj, "Create ScreenShake");
        EditorUtility.SetDirty(shake);
        return shake;
    }

    private static ScoreAnimator SetupScoreAnimator(GameManager gm)
    {
        ScoreAnimator existing = Object.FindFirstObjectByType<ScoreAnimator>();
        if (existing != null)
        {
            existing.scoreText = gm.scoreText;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        GameObject obj = new GameObject("ScoreAnimator");
        ScoreAnimator sa = obj.AddComponent<ScoreAnimator>();
        sa.scoreText = gm.scoreText;
        Undo.RegisterCreatedObjectUndo(obj, "Create ScoreAnimator");
        EditorUtility.SetDirty(sa);
        return sa;
    }

    private static TutorialManager SetupTutorial(Transform canvasTransform, InputController input, GameManager gm)
    {
        TutorialManager existing = Object.FindFirstObjectByType<TutorialManager>();
        if (existing != null) return existing;

        Transform existingPanel = canvasTransform.Find("TutorialOverlay");
        if (existingPanel != null)
            Object.DestroyImmediate(existingPanel.gameObject);

        GameObject overlayObj = new GameObject("TutorialOverlay");
        overlayObj.transform.SetParent(canvasTransform, false);
        overlayObj.transform.SetAsLastSibling();
        RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;

        Image overlayBg = overlayObj.AddComponent<Image>();
        overlayBg.color = new Color(0f, 0f, 0f, 0.7f);

        CanvasGroup cg = overlayObj.AddComponent<CanvasGroup>();

        GameObject msgPanel = new GameObject("MessagePanel");
        msgPanel.transform.SetParent(overlayObj.transform, false);
        RectTransform msgRect = msgPanel.AddComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0.5f, 0.5f);
        msgRect.anchorMax = new Vector2(0.5f, 0.5f);
        msgRect.sizeDelta = new Vector2(600f, 300f);

        Image msgBg = msgPanel.AddComponent<Image>();
        msgBg.color = PanelColor;

        GameObject textObj = new GameObject("MessageText");
        textObj.transform.SetParent(msgPanel.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 0.15f);
        textRect.anchorMax = new Vector2(0.95f, 0.95f);
        textRect.sizeDelta = Vector2.zero;

        Text msgText = textObj.AddComponent<Text>();
        msgText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        msgText.fontSize = 28;
        msgText.color = new Color(0.85f, 0.90f, 1f, 1f);
        msgText.alignment = TextAnchor.MiddleCenter;
        msgText.supportRichText = true;
        msgText.raycastTarget = false;

        GameObject tapObj = new GameObject("TapToContinue");
        tapObj.transform.SetParent(msgPanel.transform, false);
        RectTransform tapRect = tapObj.AddComponent<RectTransform>();
        tapRect.anchorMin = new Vector2(0.5f, 0f);
        tapRect.anchorMax = new Vector2(0.5f, 0f);
        tapRect.pivot = new Vector2(0.5f, 0f);
        tapRect.anchoredPosition = new Vector2(0f, 10f);
        tapRect.sizeDelta = new Vector2(300f, 30f);

        Text tapText = tapObj.AddComponent<Text>();
        tapText.text = "Tap to continue";
        tapText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tapText.fontSize = 20;
        tapText.color = new Color(0.5f, 0.55f, 0.7f, 0.8f);
        tapText.alignment = TextAnchor.MiddleCenter;
        tapText.raycastTarget = false;

        CanvasGroup tapCg = tapObj.AddComponent<CanvasGroup>();

        TutorialManager tm = overlayObj.AddComponent<TutorialManager>();
        tm.inputController = input;
        tm.gameManager = gm;
        tm.overlayGroup = cg;
        tm.messagePanel = msgRect;
        tm.messageText = msgText;
        tm.tapToContinue = tapObj;

        Undo.RegisterCreatedObjectUndo(overlayObj, "Create Tutorial");
        EditorUtility.SetDirty(tm);
        return tm;
    }
}
