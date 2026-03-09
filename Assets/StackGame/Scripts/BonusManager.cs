using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BonusManager : MonoBehaviour
{
    public Tower tower;
    public InputController inputController;
    public StackChecker stackChecker;
    public CameraController cameraController;
    public ParticleSpawner particleSpawner;

    [Header("Bonus Counts")]
    public int swapCount = 2;
    public int destroyCount = 2;
    public int shuffleCount = 1;

    [Header("UI")]
    public Text swapCountText;
    public Text destroyCountText;
    public Text shuffleCountText;
    public Image swapButton;
    public Image destroyButton;
    public Image shuffleButton;

    [Header("Chain Rewards")]
    public int chainForSwap = 3;
    public int chainForDestroy = 2;
    public int chainForShuffle = 4;

    private BonusMode currentMode = BonusMode.None;
    private int swapFirstIndex = -1;

    private static readonly Color ActiveColor = new Color(0.95f, 0.55f, 0.20f, 1f);
    private static readonly Color InactiveColor = new Color(0.20f, 0.22f, 0.35f, 1f);

    public enum BonusMode
    {
        None,
        Swap,
        Destroy,
        Shuffle
    }

    public BonusMode CurrentMode => currentMode;

    private void Start()
    {
        stackChecker.OnLayersRemoved += OnLayersRemoved;
        UpdateAllUI();

        int pending = PlayerPrefs.GetInt("PendingBonuses", 0);
        if (pending > 0)
        {
            AddBonuses(pending, pending, pending);
            PlayerPrefs.SetInt("PendingBonuses", 0);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        if (stackChecker != null)
            stackChecker.OnLayersRemoved -= OnLayersRemoved;
    }

    private void OnLayersRemoved(int count, int chainStep, Vector3 pos)
    {
        if (chainStep >= chainForShuffle)
        {
            shuffleCount++;
            ShowBonusReward(shuffleButton);
        }
        if (chainStep >= chainForSwap)
        {
            swapCount++;
            ShowBonusReward(swapButton);
        }
        if (chainStep >= chainForDestroy)
        {
            destroyCount++;
            ShowBonusReward(destroyButton);
        }
        UpdateAllUI();
    }

    public void AddBonuses(int swap, int destroy, int shuffle)
    {
        swapCount += swap;
        destroyCount += destroy;
        shuffleCount += shuffle;

        if (swap > 0) ShowBonusReward(swapButton);
        if (destroy > 0) ShowBonusReward(destroyButton);
        if (shuffle > 0) ShowBonusReward(shuffleButton);

        UpdateAllUI();
    }

    public void OnSwapPressed()
    {
        if (inputController.IsLocked()) return;
        if (stackChecker.IsProcessing) return;

        if (currentMode == BonusMode.Swap)
        {
            CancelMode();
            return;
        }

        if (swapCount <= 0) return;

        currentMode = BonusMode.Swap;
        swapFirstIndex = -1;
        UpdateModeHighlight();
    }

    public void OnDestroyPressed()
    {
        if (inputController.IsLocked()) return;
        if (stackChecker.IsProcessing) return;

        if (currentMode == BonusMode.Destroy)
        {
            CancelMode();
            return;
        }

        if (destroyCount <= 0) return;

        currentMode = BonusMode.Destroy;
        UpdateModeHighlight();
    }

    public void OnShufflePressed()
    {
        if (inputController.IsLocked()) return;
        if (stackChecker.IsProcessing) return;

        if (shuffleCount <= 0) return;

        shuffleCount--;
        UpdateAllUI();
        ExecuteShuffle();
    }

    public bool HandleConfirm(int selectedIndex)
    {
        if (currentMode == BonusMode.None) return false;

        if (currentMode == BonusMode.Destroy)
        {
            ExecuteDestroy(selectedIndex);
            return true;
        }

        if (currentMode == BonusMode.Swap)
        {
            if (swapFirstIndex < 0)
            {
                swapFirstIndex = selectedIndex;
                return true;
            }
            else
            {
                if (swapFirstIndex != selectedIndex)
                    ExecuteSwap(swapFirstIndex, selectedIndex);
                else
                    CancelMode();
                return true;
            }
        }

        return false;
    }

    private void ExecuteDestroy(int index)
    {
        if (index < 0 || index >= tower.layers.Count) return;

        destroyCount--;
        BlockLayer layer = tower.layers[index];

        if (particleSpawner != null)
            particleSpawner.SpawnForLayer(layer);

        layer.FlashWhite(0.2f);

        DOVirtual.DelayedCall(0.2f, () =>
        {
            layer.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            {
                tower.RemoveLayerAt(index);
                DropAllLayers(() =>
                {
                    CancelMode();
                    inputController.RefreshSelection();
                    stackChecker.CheckAndResolve();
                });
            });
        });

        UpdateAllUI();
    }

    private void ExecuteSwap(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= tower.layers.Count) return;
        if (indexB < 0 || indexB >= tower.layers.Count) return;

        swapCount--;

        BlockLayer layerA = tower.layers[indexA];
        BlockLayer layerB = tower.layers[indexB];

        float yA = layerA.transform.localPosition.y;
        float yB = layerB.transform.localPosition.y;

        tower.layers[indexA] = layerB;
        tower.layers[indexB] = layerA;

        Sequence seq = DOTween.Sequence();
        seq.Join(layerA.transform.DOLocalMoveY(yB, 0.35f).SetEase(Ease.InOutCubic));
        seq.Join(layerB.transform.DOLocalMoveY(yA, 0.35f).SetEase(Ease.InOutCubic));
        seq.OnComplete(() =>
        {
            CancelMode();
            inputController.RefreshSelection();
            stackChecker.CheckAndResolve();
        });

        UpdateAllUI();
    }

    private void ExecuteShuffle()
    {
        inputController.SetInputLocked(true);

        int count = tower.layers.Count;
        for (int i = count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            if (i != j)
            {
                BlockLayer temp = tower.layers[i];
                tower.layers[i] = tower.layers[j];
                tower.layers[j] = temp;
            }
        }

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < tower.layers.Count; i++)
        {
            float targetY = i * tower.layerSpacing;
            BlockLayer layer = tower.layers[i];
            seq.Join(layer.transform.DOLocalMoveY(targetY, 0.45f).SetEase(Ease.InOutBack));
        }

        seq.OnComplete(() =>
        {
            inputController.SetInputLocked(false);
            inputController.RefreshSelection();
            stackChecker.CheckAndResolve();
        });
    }

    private void DropAllLayers(Action onComplete)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < tower.layers.Count; i++)
        {
            float targetY = i * tower.layerSpacing;
            BlockLayer layer = tower.layers[i];
            float currentY = layer.transform.localPosition.y;
            if (Mathf.Abs(currentY - targetY) > 0.01f)
            {
                seq.Join(layer.transform.DOLocalMoveY(targetY, 0.3f).SetEase(Ease.OutBounce));
            }
        }
        if (seq.Duration() > 0)
            seq.OnComplete(() => onComplete?.Invoke());
        else
            onComplete?.Invoke();
    }

    public void CancelMode()
    {
        currentMode = BonusMode.None;
        swapFirstIndex = -1;
        UpdateModeHighlight();
    }

    private void UpdateModeHighlight()
    {
        if (swapButton != null)
        {
            ColorBlock cb = swapButton.GetComponent<Button>().colors;
            cb.normalColor = currentMode == BonusMode.Swap ? ActiveColor : Color.white;
            swapButton.GetComponent<Button>().colors = cb;
        }
        if (destroyButton != null)
        {
            ColorBlock cb = destroyButton.GetComponent<Button>().colors;
            cb.normalColor = currentMode == BonusMode.Destroy ? ActiveColor : Color.white;
            destroyButton.GetComponent<Button>().colors = cb;
        }
    }

    private void UpdateAllUI()
    {
        if (swapCountText != null)
            swapCountText.text = swapCount.ToString();
        if (destroyCountText != null)
            destroyCountText.text = destroyCount.ToString();
        if (shuffleCountText != null)
            shuffleCountText.text = shuffleCount.ToString();

        UpdateButtonState(swapButton, swapCount);
        UpdateButtonState(destroyButton, destroyCount);
        UpdateButtonState(shuffleButton, shuffleCount);
    }

    private void UpdateButtonState(Image btnImg, int count)
    {
        if (btnImg == null) return;
        Button btn = btnImg.GetComponent<Button>();
        if (btn != null)
            btnImg.color = count > 0 ? btn.colors.normalColor != ActiveColor ? new Color(0.15f, 0.55f, 0.75f, 1f) : ActiveColor : InactiveColor;
    }

    private void ShowBonusReward(Image btnImg)
    {
        if (btnImg == null) return;
        RectTransform rect = btnImg.GetComponent<RectTransform>();
        rect.DOPunchScale(Vector3.one * 0.3f, 0.3f, 6);
    }
}
