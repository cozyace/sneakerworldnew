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
        public Image rarityImage;
        public Image iconImage;

        // The id of this item.
        public string itemId;
        public int quantity;

        // sold out.
        public GameObject soldOutPanel;

        public void Draw(string itemId, int quantity) {
            if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                CrateData crateItem = CrateData.ParseId(itemId);
                DrawWithItemData(crateItem, quantity);
            }
            else if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                SneakerData sneakerItem = SneakerData.ParseId(itemId);
                DrawWithItemData(sneakerItem, quantity);
            }
            if (soldOutPanel != null && quantity <= 0) {
                SetSoldOut();
            }

            // Cache the id.
            this.itemId = itemId;
            this.quantity = quantity;
            gameObject.SetActive(true); 
        }
        
        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

        public void SetSoldOut() {
            soldOutPanel.SetActive(true);
        }

        public void DrawWithItemData(ItemData itemData, int quantity) {
            // Set the ui components.
            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();
            if (priceText != null) { priceText.text = $"{itemData.price}"; }
            if (quantityText != null) { quantityText.text = quantity.ToString(); }
            SetRarityImage(rarityImage, itemData.rarity);
            SetIconImage(iconImage, itemData);
        }

    }

}
