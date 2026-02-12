using UnityEngine;

public class BlockLayer : MonoBehaviour
{
    public int[] colorIndices = new int[8];

    private int rotationStep = 0;
    private Outline outline;
    private MeshRenderer[] halfRenderers = new MeshRenderer[8];

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = new Color(0.4f, 0.8f, 1f, 1f);
        outline.OutlineWidth = 4f;
        outline.enabled = false;

        CacheRenderers();
    }

    public void Initialize(int[] newColors)
    {
        System.Array.Copy(newColors, colorIndices, 8);
        BuildVisuals();
    }

    private void BuildVisuals()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        float size = 1f;
        float height = 0.4f;
        float thick = 0.08f;
        float gap = 0.02f;
        float hs = size * 0.5f;
        float sideOffset = hs - thick * 0.5f;

        float fbHalfW = (size - gap) * 0.5f;
        float fbShift = (fbHalfW + gap) * 0.5f;

        float lrLen = size - thick * 2f;
        float lrHalfD = (lrLen - gap) * 0.5f;
        float lrShift = (lrHalfD + gap) * 0.5f;

        Vector3[] positions =
        {
            new Vector3(-fbShift, 0f, sideOffset),
            new Vector3(fbShift, 0f, sideOffset),

            new Vector3(sideOffset, 0f, lrShift),
            new Vector3(sideOffset, 0f, -lrShift),

            new Vector3(fbShift, 0f, -sideOffset),
            new Vector3(-fbShift, 0f, -sideOffset),

            new Vector3(-sideOffset, 0f, -lrShift),
            new Vector3(-sideOffset, 0f, lrShift)
        };

        Vector3[] scales =
        {
            new Vector3(fbHalfW, height, thick),
            new Vector3(fbHalfW, height, thick),

            new Vector3(thick, height, lrHalfD),
            new Vector3(thick, height, lrHalfD),

            new Vector3(fbHalfW, height, thick),
            new Vector3(fbHalfW, height, thick),

            new Vector3(thick, height, lrHalfD),
            new Vector3(thick, height, lrHalfD)
        };

        string[] names =
        {
            "Front_L", "Front_R",
            "Right_L", "Right_R",
            "Back_L", "Back_R",
            "Left_L", "Left_R"
        };

        for (int i = 0; i < 8; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = names[i];
            cube.transform.SetParent(transform, false);
            cube.transform.localPosition = positions[i];
            cube.transform.localScale = scales[i];

            MeshRenderer mr = cube.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Standard"));
            mr.material.color = GameColors.FromIndex(colorIndices[i]);
            halfRenderers[i] = mr;
        }

        GameObject fill = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fill.name = "Fill";
        fill.transform.SetParent(transform, false);
        fill.transform.localPosition = Vector3.zero;
        float innerSize = size - thick * 2f - 0.002f;
        fill.transform.localScale = new Vector3(innerSize, height, innerSize);
        MeshRenderer fillMr = fill.GetComponent<MeshRenderer>();
        fillMr.material = new Material(Shader.Find("Standard"));
        fillMr.material.color = GameColors.DarkBase;
    }

    private void CacheRenderers()
    {
        string[] names =
        {
            "Front_L", "Front_R",
            "Right_L", "Right_R",
            "Back_L", "Back_R",
            "Left_L", "Left_R"
        };
        for (int i = 0; i < 8; i++)
        {
            Transform child = transform.Find(names[i]);
            if (child != null)
                halfRenderers[i] = child.GetComponent<MeshRenderer>();
        }
    }

    public void ApplyRotation(int direction)
    {
        if (direction > 0)
            rotationStep = (rotationStep + 1) % 4;
        else
            rotationStep = (rotationStep + 3) % 4;
    }

    public int GetColorIndex(int worldSide, int half)
    {
        float angle = transform.localEulerAngles.y;
        int steps = Mathf.RoundToInt(angle / 90f) % 4;
        if (steps < 0) steps += 4;
        int localSide = (worldSide + 4 - steps) % 4;
        return colorIndices[localSide * 2 + half];
    }

    public void SetHighlight(bool on)
    {
        Debug.Assert(outline != null, "Outline missing on " + gameObject.name);
        outline.enabled = on;
    }

    public void FlashWhite(float duration)
    {
        for (int i = 0; i < 8; i++)
        {
            if (halfRenderers[i] != null)
                halfRenderers[i].material.color = Color.white;
        }
        DG.Tweening.DOVirtual.DelayedCall(duration * 0.5f, () =>
        {
            if (this != null)
                RestoreColors();
        });
    }

    private void RestoreColors()
    {
        for (int i = 0; i < 8; i++)
        {
            if (halfRenderers[i] != null)
                halfRenderers[i].material.color = GameColors.FromIndex(colorIndices[i]);
        }
    }
}
