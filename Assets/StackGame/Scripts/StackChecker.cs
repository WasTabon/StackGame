using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class StackChecker : MonoBehaviour
{
    public Tower tower;
    public InputController inputController;

    [Tooltip("Which side to check for matches: 0=Front, 1=Right, 2=Back, 3=Left")]
    [Range(0, 3)]
    public int matchSide = 1;

    public float flashDuration = 0.3f;
    public float shrinkDuration = 0.25f;
    public float fallDuration = 0.35f;
    public float cascadeDelay = 0.3f;

    private bool isProcessing = false;

    public bool IsProcessing => isProcessing;

    public event Action<int, int, Vector3> OnLayersRemoved;
    public event Action<BlockLayer> OnLayerRemoving;

    public void CheckAndResolve()
    {
        if (isProcessing) return;
        StartCoroutine(CheckAndResolveRoutine());
    }

    private IEnumerator CheckAndResolveRoutine()
    {
        isProcessing = true;
        inputController.SetInputLocked(true);

        int chainStep = 0;
        bool hadMatch = true;
        while (hadMatch)
        {
            List<int> toRemove = FindMatchingPairs();
            if (toRemove.Count > 0)
            {
                chainStep++;
                hadMatch = true;

                Vector3 avgPos = Vector3.zero;
                foreach (int idx in toRemove)
                    avgPos += tower.layers[idx].transform.position;
                avgPos /= toRemove.Count;

                yield return StartCoroutine(RemoveLayersAnimated(toRemove));
                OnLayersRemoved?.Invoke(toRemove.Count, chainStep, avgPos);
                yield return new WaitForSeconds(cascadeDelay);
            }
            else
            {
                hadMatch = false;
            }
        }

        isProcessing = false;
        inputController.SetInputLocked(false);
        inputController.RefreshSelection();
    }

    private List<int> FindMatchingPairs()
    {
        HashSet<int> matched = new HashSet<int>();
        List<BlockLayer> layers = tower.layers;

        for (int i = 0; i < layers.Count - 1; i++)
        {
            if (DoLayersMatch(layers[i], layers[i + 1]))
            {
                matched.Add(i);
                matched.Add(i + 1);
            }
        }

        List<int> result = new List<int>(matched);
        result.Sort();
        return result;
    }

    private bool DoLayersMatch(BlockLayer a, BlockLayer b)
    {
        int aLeft = a.GetColorIndex(matchSide, 0);
        int aRight = a.GetColorIndex(matchSide, 1);
        int bLeft = b.GetColorIndex(matchSide, 0);
        int bRight = b.GetColorIndex(matchSide, 1);

        return aLeft == bLeft && aRight == bRight;
    }

    private IEnumerator RemoveLayersAnimated(List<int> indices)
    {
        List<BlockLayer> layersToRemove = new List<BlockLayer>();
        foreach (int idx in indices)
        {
            layersToRemove.Add(tower.layers[idx]);
        }

        foreach (var layer in layersToRemove)
        {
            layer.SetHighlight(false);
            OnLayerRemoving?.Invoke(layer);
            layer.FlashWhite(flashDuration);
        }
        yield return new WaitForSeconds(flashDuration);

        Sequence shrinkSeq = DOTween.Sequence();
        foreach (var layer in layersToRemove)
        {
            shrinkSeq.Join(
                layer.transform.DOScale(Vector3.zero, shrinkDuration).SetEase(Ease.InBack, 1.5f)
            );
        }
        yield return shrinkSeq.WaitForCompletion();

        for (int i = indices.Count - 1; i >= 0; i--)
        {
            tower.RemoveLayerAt(indices[i]);
        }

        yield return StartCoroutine(DropLayersAnimated());
    }

    private IEnumerator DropLayersAnimated()
    {
        Sequence dropSeq = DOTween.Sequence();
        for (int i = 0; i < tower.layers.Count; i++)
        {
            float targetY = i * tower.layerSpacing;
            BlockLayer layer = tower.layers[i];
            float currentY = layer.transform.localPosition.y;

            if (Mathf.Abs(currentY - targetY) > 0.01f)
            {
                dropSeq.Join(
                    layer.transform.DOLocalMoveY(targetY, fallDuration).SetEase(Ease.OutBounce)
                );
            }
        }

        if (dropSeq.Duration() > 0)
            yield return dropSeq.WaitForCompletion();
    }
}
