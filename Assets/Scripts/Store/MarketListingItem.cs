using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketListingItem : MonoBehaviour
{
   [Header("Data")]
   public string Name;
   public int CashPrice;
   public int GemPrice;
   public int Quantity;
   public SneakerRarity Rarity;
   public Sprite Icon;
   public string SellerName;
   public DateTime ExpiryDate;
   public MarketListing ListingData;
   
   [Header("UI Components")]
   public Image IconImage;
   public Image RarityPanel;
   public Image CurrencyTypeIcon;
   public TMP_Text ItemNameText;
   public TMP_Text ListerNameText;
   public TMP_Text PriceText;
   public TMP_Text QuantityText;
   public TMP_Text RarityText;
   public TMP_Text ListingDurationText; //How much time is remaining on the listing.
   public Button BuyButton;
   public Button RemoveButton;

   private MarketManager _MarketManager;


   private void Start()
   {
      Invoke(nameof(DisableIfEmpty), 0.15f);
      
   }

   //Turns this instance off, if the data is missing.
   private void DisableIfEmpty()
   {
      if (Name != "" || Icon != null)
         return;
      
      gameObject.SetActive(false);
      Debug.LogWarning("Disabled Market Listing (Was missing data!)");
   }

   private async void UpdateExpiryTime()
   {
      if (ExpiryDate == default)
         return;
      
      TimeSpan remainingTime = ExpiryDate - DateTime.Now; //Might need to check server time instead of client datetime, to avoid device time manipulation.

      if (remainingTime <= TimeSpan.Zero)
      {
         //Check if the listing still exists.
         bool doesMarketListingExist = await _MarketManager.GameManager.firebase.DoesMarketListingExist(ListingData.key);

         if (doesMarketListingExist)
            await _MarketManager.GameManager.firebase.ExpireMarketListing(ListingData.key);
        
         Destroy(gameObject);
      }
      else
         ListingDurationText.text = $"{(int)remainingTime.TotalHours:00}:{remainingTime.Minutes:00}:{remainingTime.Seconds:00}";
   }
   
   public void UpdateUIComponents(MarketManager manager, bool isPersonalListing, string itemName, int cash, int gem, int quantity, DateTime expiryDate, SneakerRarity rarity, Sprite icon, string sellerName, Sprite rarityPanelSprite, MarketListing listingData)
   {
      _MarketManager = manager;
      Name = itemName;
      CashPrice = cash;
      GemPrice = gem;
      Quantity = quantity;
      Rarity = rarity;
      Icon = icon;
      SellerName = sellerName;
      ExpiryDate = expiryDate;

      ListingData = listingData;

      IconImage.sprite = icon;
      ListerNameText.text = sellerName;
      ItemNameText.text = itemName;
      RarityText.text = rarity.ToString();
      QuantityText.text = quantity.ToString();
      RarityPanel.sprite = rarityPanelSprite;
      
      InvokeRepeating(nameof(UpdateExpiryTime), 0f, 1f);
      
      if (cash > 0)
      {
         PriceText.text = AbbreviateNumber(cash);
         PriceText.color = Color.green;
         CurrencyTypeIcon.sprite = Resources.Load<Sprite>("Cash");
      }
      else if (gem > 0)
      { //Add signification that this is gems, maybe icon.
         PriceText.text = AbbreviateNumber(gem);
         PriceText.color = Color.cyan;
         CurrencyTypeIcon.sprite = Resources.Load<Sprite>("Gem");
      }
      
      BuyButton.gameObject.SetActive(!isPersonalListing);
      RemoveButton.gameObject.SetActive(isPersonalListing);

      BuyButton.onClick.AddListener(() => manager.PurchaseListing(ListingData));
      RemoveButton.onClick.AddListener(() => manager.EnableDeleteConfirmationMenu(ListingData));
   }
   
   
   
   
   private static string AbbreviateNumber(int num)
   {
      if (num >= 1000000000) // Billions
      {
         return (num / 1000000000.0).ToString("0.#") + "b";
      }
      else if (num >= 1000000) // Millions
      {
         return (num / 1000000.0).ToString("0.#") + "m";
      }
      else if (num >= 1000) // Thousands
      {
         return (num / 1000.0).ToString("0.#") + "k";
      }
      else
      {
         return num.ToString();
      }
   }
}
