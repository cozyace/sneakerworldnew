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
   public TMP_Text ItemNameText;
   public TMP_Text ListerNameText;
   public TMP_Text PriceText;
   public TMP_Text QuantityText;
   public TMP_Text RarityText;
   public Button BuyButton;


   private MarketManager _MarketManager;

   public void UpdateUIComponents(MarketManager manager, string itemName, int cash, int gem, int quantity, SneakerRarity rarity, Sprite icon, string sellerName, Sprite rarityPanelSprite, MarketListing listingData)
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
         PriceText.text = "$" + cash;
      else if (gem > 0)
      { //Add signification that this is gems, maybe icon.
         PriceText.text = gem.ToString();
         PriceText.color = Color.magenta;
      }

      BuyButton.onClick.AddListener(() => manager.PurchaseListing(ListingData));
   }
}
