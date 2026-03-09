using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;

public class IAPManager : MonoBehaviour
{
    public string productId = "com.swap.inapp";

    [Header("Shop Panel")]
    public CanvasGroup panelGroup;
    public RectTransform panelRect;

    [Header("UI")]
    public Text priceText;
    public Text statusText;

    private void Awake()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
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

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Purchase complete");

            int pending = PlayerPrefs.GetInt("PendingBonuses", 0);
            PlayerPrefs.SetInt("PendingBonuses", pending + 1);
            PlayerPrefs.Save();

            if (statusText != null)
            {
                statusText.color = new Color(0.25f, 0.82f, 0.50f, 1f);
                statusText.text = "+1 Swap  +1 Destroy  +1 Shuffle";
            }

            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayLevelComplete();

            if (panelRect != null)
                panelRect.DOPunchScale(Vector3.one * 0.1f, 0.3f, 5);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Failed: " + description.message);

            if (statusText != null)
            {
                statusText.color = new Color(0.90f, 0.22f, 0.35f, 1f);
                statusText.text = "Purchase failed";
            }
        }
    }

    public void OnProductFetched(Product product)
    {
        Debug.Log("[IAP] Fetched: " + product.metadata.localizedPriceString);
        if (priceText != null)
            priceText.text = product.metadata.localizedPriceString;
    }
}