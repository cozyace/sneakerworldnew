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

    public class StoreItem : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public Image rarityImage;
        public Image iconImage;

        // Cache the item id for what this UI component corresponds to.
        public string itemId;
        public int maxQuantity;
        public bool featured;

        // Whether this item is locked.
        public GameObject lockedPanel;
        public TextMeshProUGUI lockedPanelText;

        // sold out.
        public GameObject soldOutPanel;

        public void Draw<TItemData>(TItemData itemData, float dur)
            where TItemData : ItemData {

            // Set the ui components.
            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();
            priceText.text = $"${itemData.price}";
            if (quantityText != null) {
                quantityText.text = $"{itemData.quantity} Left";
            }
            if (itemData.quantity <= 0) {
                SetSoldOut();
            }
            SetRarityImage(rarityImage, itemData.rarity);
            SetIconImage(iconImage, itemData);

            // Cache the item id.
            itemId = itemData.id;
            maxQuantity = itemData.quantity;
            featured = itemData.featured;
            
        }

       
        public void SetSoldOut() {
            soldOutPanel.SetActive(true);
        }

        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

    }

}
