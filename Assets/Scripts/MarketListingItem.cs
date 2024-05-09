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
   public Button BuyButton;
   public Button RemoveButton;

   private MarketManager _MarketManager;


   private void Start()
   {
      Invoke(nameof(DisableIfEmpty), 1.25f);
   }

   //Turns this instance off, if the data is missing.
   private void DisableIfEmpty()
   {
      if (Name != "" || Icon != null)
         return;
      
      gameObject.SetActive(false);
      Debug.LogWarning("Disabled Market Listing (Was missing data!)");
   }
   
   public void UpdateUIComponents(MarketManager manager, bool isPersonalListing, string itemName, int cash, int gem, int quantity, SneakerRarity rarity, Sprite icon, string sellerName, Sprite rarityPanelSprite, MarketListing listingData)
   {
      Name = itemName;
      CashPrice = cash;
      GemPrice = gem;
      Quantity = quantity;
      Rarity = rarity;
      Icon = icon;
      SellerName = sellerName;

      ListingData = listingData;

      IconImage.sprite = icon;
      ListerNameText.text = sellerName;
      ItemNameText.text = itemName;
      RarityText.text = rarity.ToString();
      QuantityText.text = quantity.ToString();
      RarityPanel.sprite = rarityPanelSprite;

      if (cash > 0)
      {
         PriceText.text = "$" + cash;
         PriceText.color = Color.green;
         CurrencyTypeIcon.sprite = Resources.Load<Sprite>("Cash");
      }
      else if (gem > 0)
      { //Add signification that this is gems, maybe icon.
         PriceText.text = gem.ToString();
         PriceText.color = Color.cyan;
         CurrencyTypeIcon.sprite = Resources.Load<Sprite>("Gem");
      }
      
      BuyButton.gameObject.SetActive(!isPersonalListing);
      RemoveButton.gameObject.SetActive(isPersonalListing);

      BuyButton.onClick.AddListener(() => manager.PurchaseListing(ListingData));
      RemoveButton.onClick.AddListener(() => manager.EnableDeleteConfirmationMenu(ListingData));
   }
}
