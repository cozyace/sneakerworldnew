using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    IStoreController m_StoreController; 
    public string removeAdsProductId = "com.cozyace.sneakerworld.removeads";
    private bool isAdsRemoved;
    public GameObject removeAdsUI, purchaseSuccessfulText, purchaseFailedText;

    void Start()
    {
        if (PlayerPrefs.GetInt("ADS_REMOVED") == 1)
            removeAdsUI.SetActive(false);

        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(removeAdsProductId, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void RemoveAdsButton()
    {
        m_StoreController.InitiatePurchase(removeAdsProductId);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;

        if (product.definition.id == removeAdsProductId)
        {
            RemoveAds();
        }

        purchaseSuccessfulText.SetActive(true);
        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        purchaseFailedText.SetActive(true);
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }

    void RemoveAds()
    {
        isAdsRemoved = true;
        PlayerPrefs.SetInt("ADS_REMOVED", 1);
    }
}