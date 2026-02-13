using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    public int initialLayerCount = 6;
    public float layerSpacing = 0.5f;
    public LayerSpawner spawner;

    [HideInInspector]
    public List<BlockLayer> layers = new List<BlockLayer>();

    public void SpawnInitialLayers()
    {
        ClearLayers();
        for (int i = 0; i < initialLayerCount; i++)
        {
            SpawnLayerAtIndex(i);
        }
    }

    public BlockLayer SpawnLayerAtIndex(int index)
    {
        GameObject obj = new GameObject("Layer_" + index);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0f, index * layerSpacing, 0f);

        BlockLayer layer = obj.AddComponent<BlockLayer>();
        int[] colors = spawner != null ? spawner.GenerateSmartColors() : GenerateRandomColors();
        layer.Initialize(colors);
        layers.Add(layer);
        return layer;
    }

    public BlockLayer AddLayerOnTop()
    {
        int index = layers.Count;
        return SpawnLayerAtIndex(index);
    }

    public float GetLayerWorldY(int index)
    {
        return transform.position.y + index * layerSpacing;
    }

    public float GetTowerHeight()
    {
        return layers.Count * layerSpacing;
    }

    public void RemoveLayerAt(int index)
    {
        Debug.Assert(index >= 0 && index < layers.Count, "RemoveLayerAt: index out of range: " + index);
        BlockLayer layer = layers[index];
        layers.RemoveAt(index);
        Destroy(layer.gameObject);
    }

    public void ClearLayers()
    {
        foreach (var layer in layers)
        {
            if (layer != null)
                DestroyImmediate(layer.gameObject);
        }
        layers.Clear();
    }

    private int[] GenerateRandomColors()
    {
        int[] c = new int[8];
        for (int i = 0; i < 8; i++)
            c[i] = GameColors.RandomIndex();
        return c;
    }
}
