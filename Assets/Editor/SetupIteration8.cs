using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class SetupIteration8 : Editor
{
    private static readonly Color SwapColor = new Color(0.18f, 0.78f, 0.85f, 1f);
    private static readonly Color DestroyColor = new Color(0.90f, 0.22f, 0.35f, 1f);
    private static readonly Color ShuffleColor = new Color(0.55f, 0.28f, 0.92f, 1f);

    [MenuItem("STACK/Setup Bonuses (Iteration 8)")]
    public static void Setup()
    {
        Tower tower = Object.FindFirstObjectByType<Tower>();
        Debug.Assert(tower != null, "Tower not found!");

        InputController input = Object.FindFirstObjectByType<InputController>();
        Debug.Assert(input != null, "InputController not found!");

        StackChecker checker = Object.FindFirstObjectByType<StackChecker>();
        Debug.Assert(checker != null, "StackChecker not found!");

        CameraController cam = Object.FindFirstObjectByType<CameraController>();
        ParticleSpawner particles = Object.FindFirstObjectByType<ParticleSpawner>();
        Canvas mainCanvas = FindMainCanvas();
        Debug.Assert(mainCanvas != null, "Main Canvas not found!");

        BonusManager bm = SetupBonusManager(tower, input, checker, cam, particles);
        CreateBonusUI(mainCanvas.transform, bm);

        input.bonusManager = bm;
        EditorUtility.SetDirty(input);

        Debug.Log("[Iteration 8] Bonuses setup complete.");
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

    private static BonusManager SetupBonusManager(Tower tower, InputController input, StackChecker checker, CameraController cam, ParticleSpawner particles)
    {
        BonusManager bm = Object.FindFirstObjectByType<BonusManager>();
        if (bm == null)
        {
            GameObject obj = new GameObject("BonusManager");
            bm = obj.AddComponent<BonusManager>();
            Undo.RegisterCreatedObjectUndo(obj, "Create BonusManager");
        }

        bm.tower = tower;
        bm.inputController = input;
        bm.stackChecker = checker;
        bm.cameraController = cam;
        bm.particleSpawner = particles;

        EditorUtility.SetDirty(bm);
        return bm;
    }

    private static void CreateBonusUI(Transform canvasTransform, BonusManager bm)
    {
        Transform existing = canvasTransform.Find("BonusPanel");
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        GameObject panel = new GameObject("BonusPanel");
        panel.transform.SetParent(canvasTransform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 0.5f);
        panelRect.anchorMax = new Vector2(1f, 0.5f);
        panelRect.pivot = new Vector2(1f, 0.5f);
        panelRect.anchoredPosition = new Vector2(-20f, 0f);
        panelRect.sizeDelta = new Vector2(90f, 330f);

        VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 15f;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        var (swapBtn, swapCount) = CreateBonusButton(panel.transform, "SwapBtn", "SWAP", SwapColor, bm, "OnSwapPressed");
        var (destroyBtn, destroyCount) = CreateBonusButton(panel.transform, "DestroyBtn", "DEL", DestroyColor, bm, "OnDestroyPressed");
        var (shuffleBtn, shuffleCount) = CreateBonusButton(panel.transform, "ShuffleBtn", "MIX", ShuffleColor, bm, "OnShufflePressed");

        bm.swapButton = swapBtn.GetComponent<Image>();
        bm.swapCountText = swapCount;
        bm.destroyButton = destroyBtn.GetComponent<Image>();
        bm.destroyCountText = destroyCount;
        bm.shuffleButton = shuffleBtn.GetComponent<Image>();
        bm.shuffleCountText = shuffleCount;

        EditorUtility.SetDirty(bm);
    }

    private static (GameObject btn, Text countText) CreateBonusButton(Transform parent, string name, string label, Color color, BonusManager bm, string methodName)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        LayoutElement le = btnObj.AddComponent<LayoutElement>();
        le.preferredHeight = 90f;

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

        var method = typeof(BonusManager).GetMethod(methodName);
        if (method != null)
        {
            UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), bm, method);
            btn.onClick = new Button.ButtonClickedEvent();
            UnityEventTools.AddPersistentListener(btn.onClick, action);
        }

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(btnObj.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0.4f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.sizeDelta = Vector2.zero;

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = label;
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 18;
        labelText.color = new Color(0.95f, 0.95f, 1f, 1f);
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.fontStyle = FontStyle.Bold;
        labelText.raycastTarget = false;

        GameObject countObj = new GameObject("Count");
        countObj.transform.SetParent(btnObj.transform, false);
        RectTransform countRect = countObj.AddComponent<RectTransform>();
        countRect.anchorMin = new Vector2(0f, 0f);
        countRect.anchorMax = new Vector2(1f, 0.45f);
        countRect.sizeDelta = Vector2.zero;

        Text countText = countObj.AddComponent<Text>();
        countText.text = "2";
        countText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        countText.fontSize = 28;
        countText.color = new Color(0.95f, 0.95f, 1f, 0.9f);
        countText.alignment = TextAnchor.MiddleCenter;
        countText.fontStyle = FontStyle.Bold;
        countText.raycastTarget = false;

        EditorUtility.SetDirty(btn);
        return (btnObj, countText);
    }
}
