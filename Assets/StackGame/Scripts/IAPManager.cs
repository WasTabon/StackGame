using UnityEngine;
using System;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

public class IAPManager : MonoBehaviour
#if UNITY_PURCHASING
    , IDetailedStoreListener
#endif
{
    public static IAPManager Instance { get; private set; }

    public const string PRODUCT_BONUS_PACK = "com.stack.bonuspack";

    public event Action PurchaseSucceeded;
    public event Action<string> PurchaseFailed;

#if UNITY_PURCHASING
    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
#endif

    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
#if UNITY_PURCHASING
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(PRODUCT_BONUS_PACK, ProductType.Consumable);
        UnityPurchasing.Initialize(this, builder);
#else
        Debug.Log("[IAP] Unity Purchasing not enabled. Using stub mode.");
        isInitialized = true;
#endif
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public string GetLocalizedPrice()
    {
#if UNITY_PURCHASING
        if (storeController != null)
        {
            Product product = storeController.products.WithID(PRODUCT_BONUS_PACK);
            if (product != null && product.availableToPurchase)
                return product.metadata.localizedPriceString;
        }
#endif
        return "$0.99";
    }

    public void BuyBonusPack()
    {
#if UNITY_PURCHASING
        if (!isInitialized || storeController == null)
        {
            Debug.LogWarning("[IAP] Not initialized");
            PurchaseFailed?.Invoke("Not initialized");
            return;
        }
        storeController.InitiatePurchase(PRODUCT_BONUS_PACK);
#else
        Debug.Log("[IAP] Stub purchase: bonus pack");
        GrantBonuses();
#endif
    }

    private void GrantBonuses()
    {
        PurchaseSucceeded?.Invoke();
    }

#if UNITY_PURCHASING
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        isInitialized = true;
        Debug.Log("[IAP] Initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("[IAP] Init failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("[IAP] Init failed: " + error + " - " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (string.Equals(args.purchasedProduct.definition.id, PRODUCT_BONUS_PACK, StringComparison.Ordinal))
        {
            Debug.Log("[IAP] Bonus pack purchased");
            GrantBonuses();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning("[IAP] Purchase failed: " + product.definition.id + " reason: " + reason);
        PurchaseFailed?.Invoke(reason.ToString());
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription desc)
    {
        Debug.LogWarning("[IAP] Purchase failed: " + product.definition.id + " reason: " + desc.reason + " msg: " + desc.message);
        PurchaseFailed?.Invoke(desc.message);
    }
#endif
}