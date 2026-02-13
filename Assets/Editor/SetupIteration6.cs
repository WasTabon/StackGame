using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupIteration6 : Editor
{
    [MenuItem("STACK/Setup Visual Effects (Iteration 6)")]
    public static void Setup()
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        Debug.Assert(gm != null, "GameManager not found! Run Iteration 5 first.");

        Canvas mainCanvas = FindMainCanvas();
        Debug.Assert(mainCanvas != null, "Main Canvas not found!");

        Camera cam = Camera.main;
        Debug.Assert(cam != null, "Main Camera not found!");

        ScorePopup popup = SetupScorePopup(mainCanvas, cam);
        ParticleSpawner particles = SetupParticleSpawner();
        SpawnTimerUI timerUI = SetupSpawnTimerUI(mainCanvas);

        gm.scorePopup = popup;
        gm.particleSpawner = particles;
        gm.spawnTimerUI = timerUI;
        EditorUtility.SetDirty(gm);

        Debug.Log("[Iteration 6] Visual effects setup complete.");
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

    private static ScorePopup SetupScorePopup(Canvas canvas, Camera cam)
    {
        ScorePopup existing = Object.FindFirstObjectByType<ScorePopup>();
        if (existing != null)
        {
            existing.canvas = canvas;
            existing.gameCamera = cam;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        GameObject obj = new GameObject("ScorePopup");
        ScorePopup popup = obj.AddComponent<ScorePopup>();
        popup.canvas = canvas;
        popup.gameCamera = cam;
        Undo.RegisterCreatedObjectUndo(obj, "Create ScorePopup");
        EditorUtility.SetDirty(popup);
        return popup;
    }

    private static ParticleSpawner SetupParticleSpawner()
    {
        ParticleSpawner existing = Object.FindFirstObjectByType<ParticleSpawner>();
        if (existing != null)
            return existing;

        GameObject obj = new GameObject("ParticleSpawner");
        ParticleSpawner spawner = obj.AddComponent<ParticleSpawner>();
        Undo.RegisterCreatedObjectUndo(obj, "Create ParticleSpawner");
        EditorUtility.SetDirty(spawner);
        return spawner;
    }

    private static SpawnTimerUI SetupSpawnTimerUI(Canvas canvas)
    {
        Transform existing = canvas.transform.Find("SpawnTimerBar");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject barRoot = new GameObject("SpawnTimerBar");
        barRoot.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = barRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(1f, 1f);
        rootRect.pivot = new Vector2(0.5f, 1f);
        rootRect.anchoredPosition = new Vector2(0f, 0f);
        rootRect.sizeDelta = new Vector2(0f, 12f);

        Image bgImage = barRoot.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f, 0.6f);
        bgImage.raycastTarget = false;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(barRoot.transform, false);
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.18f, 0.78f, 0.85f, 0.8f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        fillImage.fillAmount = 0f;
        fillImage.raycastTarget = false;

        SpawnTimerUI timerUI = barRoot.AddComponent<SpawnTimerUI>();
        timerUI.fillBar = fillImage;
        timerUI.bgBar = bgImage;

        EditorUtility.SetDirty(timerUI);
        return timerUI;
    }
}
