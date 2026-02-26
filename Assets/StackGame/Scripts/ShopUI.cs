using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopUI : MonoBehaviour
{
    public CanvasGroup panelGroup;
    public RectTransform panelRect;
    public Text statusText;
    public BonusManager bonusManager;

    private void Awake()
    {
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.PurchaseSucceeded += OnPurchaseSuccess;
            IAPManager.Instance.PurchaseFailed += OnPurchaseFailed;
        }
    }

    private void OnDisable()
    {
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.PurchaseSucceeded -= OnPurchaseSuccess;
            IAPManager.Instance.PurchaseFailed -= OnPurchaseFailed;
        }
    }

    public void Show()
    {
        if (statusText != null)
            statusText.text = "";

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
        panelRect.localScale = Vector3.one * 0.8f;
        panelGroup.DOFade(1f, 0.25f);
        panelRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        panelGroup.DOFade(0f, 0.2f).OnComplete(() =>
        {
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        });
    }

    private void OnPurchaseSuccess()
    {
        if (statusText != null)
        {
            statusText.color = new Color(0.25f, 0.82f, 0.50f, 1f);
            statusText.text = "+1 Swap  +1 Destroy  +1 Shuffle";
        }

        if (bonusManager != null)
        {
            bonusManager.AddBonuses(1, 1, 1);
        }
        else
        {
            int pending = PlayerPrefs.GetInt("PendingBonuses", 0);
            PlayerPrefs.SetInt("PendingBonuses", pending + 1);
            PlayerPrefs.Save();
        }

        if (SFXManager.Instance != null)
            SFXManager.Instance.PlayLevelComplete();

        if (panelRect != null)
            panelRect.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5);
    }

    private void OnPurchaseFailed(string reason)
    {
        if (statusText != null)
        {
            statusText.color = new Color(0.90f, 0.22f, 0.35f, 1f);
            statusText.text = "Purchase failed";
        }
    }
}