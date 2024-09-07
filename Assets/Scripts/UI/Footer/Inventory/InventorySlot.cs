// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    public class InventorySlot : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public TextMeshProUGUI markupValText;
        public TextMeshProUGUI markupPercText;
        public Toggle onSaleToggle;

        public Image rarityImage;
        public Image iconImage;

        // The id of this item.
        public string itemId;
        public int quantity;

        // sold out.
        public GameObject soldOutPanel;

        public void Draw(string itemId, int quantity, bool onSale = false, float markup = 1f) {
            if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                CrateData crateItem = CrateData.ParseId(itemId);
                DrawWithItemData(crateItem, quantity, onSale, markup);
            }
            else if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                SneakerData sneakerItem = SneakerData.ParseId(itemId);
                DrawWithItemData(sneakerItem, quantity, onSale, markup);
            }
            if (soldOutPanel != null) {
                SetSoldOut(quantity);
            }

            // Cache the id.
            this.itemId = itemId;
            this.quantity = quantity;
            gameObject.SetActive(true); 
        }

        public void Clear() {
            nameText.text = "";
            rarityText.text = "";
            if (priceText != null) { priceText.text = ""; }
            if (quantityText != null) { quantityText.text = ""; }
            rarityImage.sprite = null;
            iconImage.sprite = null;
            itemId = "";
            quantity = 0;
        }
        
        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

        public void SetSoldOut(int quantity) {
            if (quantity <= 0) {
                soldOutPanel.SetActive(true);
            }
            else {
                soldOutPanel.SetActive(false);
            }
        }

        public void DrawWithItemData(ItemData itemData, int quantity, bool onSale, float markup) {
            markup = Player.instance.shop.markup;

            // Set the ui components.
            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();
            if (priceText != null) { priceText.text = $"{itemData.price}"; }
            if (quantityText != null) { quantityText.text = quantity.ToString(); }
            if (onSaleToggle != null) { onSaleToggle.SetIsOnWithoutNotify(onSale); }
            if (markupPercText != null) {  
                int markupPerc = (int)Mathf.Round(markup*100f);
                markupPercText.text = $"{markupPerc.ToString()}%";   
            }
            if (markupValText != null) {  
                int markupVal = (int)Mathf.Round((1+markup)*itemData.price);
                markupValText.text = $"{markupVal.ToString()}";   
            }

            SetRarityImage(rarityImage, itemData.rarity);
            SetIconImage(iconImage, itemData);
        }

    }

}
