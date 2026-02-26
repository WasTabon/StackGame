using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }

    public GameObject panel;

    public string productId = "com.swap.inapp";

    public event Action PurchaseSucceeded;
    public event Action<string> PurchaseFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Purchase complete");
            panel.SetActive(true);
            PurchaseSucceeded?.Invoke();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == productId)
        {
            Debug.Log("[IAP] Purchase failed: " + description.message);
            PurchaseFailed?.Invoke(description.message);
        }
    }

    public void OnProductFetched(Product product)
    {
        Debug.Log("[IAP] Product fetched: " + product.metadata.localizedPriceString);
    }
}