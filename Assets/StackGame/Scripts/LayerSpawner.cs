using UnityEngine;
using System.Collections.Generic;

public class LayerSpawner : MonoBehaviour
{
    public Tower tower;
    public int matchSide = 1;

    [Range(0f, 1f)]
    public float chainChance = 0.35f;

    [Range(0f, 1f)]
    public float nearMatchChance = 0.25f;

    public int[] GenerateSmartColors()
    {
        int[] colors = GenerateRandomColors();

        if (tower.layers.Count == 0)
            return colors;

        BlockLayer topLayer = tower.layers[tower.layers.Count - 1];
        PlaceMatchOnRandomSide(colors, topLayer);

        if (tower.layers.Count >= 3 && Random.value < chainChance)
            TrySetupChain(colors);

        if (Random.value < nearMatchChance && tower.layers.Count >= 2)
            TryAddNearMatch(colors);

        if (!HasAnySolvablePair(colors))
            ForceGuaranteedMatch(colors);

        return colors;
    }

    private void PlaceMatchOnRandomSide(int[] newColors, BlockLayer neighbor)
    {
        int placeSide = Random.Range(0, 4);
        int neighborLeft = GetLayerRawSideColor(neighbor, matchSide, 0);
        int neighborRight = GetLayerRawSideColor(neighbor, matchSide, 1);

        newColors[placeSide * 2 + 0] = neighborLeft;
        newColors[placeSide * 2 + 1] = neighborRight;
    }

    private void TrySetupChain(int[] newColors)
    {
        int count = tower.layers.Count;
        int gapIndex = Random.Range(0, count - 1);

        BlockLayer above = tower.layers[gapIndex + 1];
        BlockLayer below = tower.layers[gapIndex];

        for (int rotA = 0; rotA < 4; rotA++)
        {
            int aLeft = below.colorIndices[rotA * 2 + 0];
            int aRight = below.colorIndices[rotA * 2 + 1];

            for (int rotB = 0; rotB < 4; rotB++)
            {
                int bLeft = above.colorIndices[rotB * 2 + 0];
                int bRight = above.colorIndices[rotB * 2 + 1];

                if (aLeft == bLeft && aRight == bRight)
                    return;
            }
        }

        int shareSide = Random.Range(0, 4);
        int pickLayer = Random.value < 0.5f ? gapIndex : gapIndex + 1;
        BlockLayer source = tower.layers[pickLayer];
        int srcLeft = GetLayerRawSideColor(source, matchSide, 0);
        int srcRight = GetLayerRawSideColor(source, matchSide, 1);

        int otherLayer = pickLayer == gapIndex ? gapIndex + 1 : gapIndex;
        BlockLayer target = tower.layers[otherLayer];
        int otherSide = Random.Range(0, 4);
        target.colorIndices[otherSide * 2 + 0] = srcLeft;
        target.colorIndices[otherSide * 2 + 1] = srcRight;
    }

    private void TryAddNearMatch(int[] newColors)
    {
        int randomLayerIdx = Random.Range(0, tower.layers.Count);
        BlockLayer randomLayer = tower.layers[randomLayerIdx];

        int srcSide = Random.Range(0, 4);
        int dstSide = Random.Range(0, 4);

        newColors[dstSide * 2 + 0] = randomLayer.colorIndices[srcSide * 2 + 0];
        newColors[dstSide * 2 + 1] = GameColors.RandomIndex();
    }

    private bool HasAnySolvablePair(int[] newLayerColors)
    {
        List<BlockLayer> layers = tower.layers;

        if (layers.Count == 0) return true;

        for (int i = 0; i < layers.Count - 1; i++)
        {
            if (CanMatchWithRotation(layers[i], layers[i + 1]))
                return true;
        }

        BlockLayer top = layers[layers.Count - 1];
        if (CanMatchNewWithExisting(newLayerColors, top))
            return true;

        return false;
    }

    private bool CanMatchWithRotation(BlockLayer a, BlockLayer b)
    {
        for (int rotA = 0; rotA < 4; rotA++)
        {
            int aLeft = a.colorIndices[((matchSide + 4 - rotA) % 4) * 2 + 0];
            int aRight = a.colorIndices[((matchSide + 4 - rotA) % 4) * 2 + 1];

            for (int rotB = 0; rotB < 4; rotB++)
            {
                int bLeft = b.colorIndices[((matchSide + 4 - rotB) % 4) * 2 + 0];
                int bRight = b.colorIndices[((matchSide + 4 - rotB) % 4) * 2 + 1];

                if (aLeft == bLeft && aRight == bRight)
                    return true;
            }
        }
        return false;
    }

    private bool CanMatchNewWithExisting(int[] newColors, BlockLayer existing)
    {
        for (int rotNew = 0; rotNew < 4; rotNew++)
        {
            int nLeft = newColors[((matchSide + 4 - rotNew) % 4) * 2 + 0];
            int nRight = newColors[((matchSide + 4 - rotNew) % 4) * 2 + 1];

            for (int rotEx = 0; rotEx < 4; rotEx++)
            {
                int eLeft = existing.colorIndices[((matchSide + 4 - rotEx) % 4) * 2 + 0];
                int eRight = existing.colorIndices[((matchSide + 4 - rotEx) % 4) * 2 + 1];

                if (nLeft == eLeft && nRight == eRight)
                    return true;
            }
        }
        return false;
    }

    private void ForceGuaranteedMatch(int[] newColors)
    {
        if (tower.layers.Count == 0) return;

        BlockLayer top = tower.layers[tower.layers.Count - 1];
        int srcSide = Random.Range(0, 4);
        int dstSide = Random.Range(0, 4);

        newColors[dstSide * 2 + 0] = top.colorIndices[srcSide * 2 + 0];
        newColors[dstSide * 2 + 1] = top.colorIndices[srcSide * 2 + 1];
    }

    private int GetLayerRawSideColor(BlockLayer layer, int side, int half)
    {
        float angle = layer.transform.localEulerAngles.y;
        int steps = Mathf.RoundToInt(angle / 90f) % 4;
        if (steps < 0) steps += 4;
        int localSide = (side + 4 - steps) % 4;
        return layer.colorIndices[localSide * 2 + half];
    }

    private int[] GenerateRandomColors()
    {
        int[] c = new int[8];
        for (int i = 0; i < 8; i++)
            c[i] = GameColors.RandomIndex();
        return c;
    }
}
