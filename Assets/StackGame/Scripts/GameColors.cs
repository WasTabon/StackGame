using UnityEngine;

public static class GameColors
{
    public static readonly Color[] Palette = new Color[]
    {
        new Color(0.90f, 0.22f, 0.35f, 1f),
        new Color(0.18f, 0.78f, 0.85f, 1f),
        new Color(0.55f, 0.28f, 0.92f, 1f),
        new Color(0.95f, 0.55f, 0.20f, 1f),
        new Color(0.25f, 0.82f, 0.50f, 1f),
    };

    public static readonly Color DarkBase = new Color(0.05f, 0.06f, 0.15f, 1f);
    public static readonly Color DarkStrip = new Color(0.03f, 0.04f, 0.10f, 1f);
    public static readonly Color Background = new Color(0.04f, 0.05f, 0.12f, 1f);

    public static int RandomIndex()
    {
        return Random.Range(0, Palette.Length);
    }

    public static Color FromIndex(int index)
    {
        return Palette[index % Palette.Length];
    }
}
