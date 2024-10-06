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

    public class ItemSlot : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public Image rarityImage;
        public Image iconImage;

        // The id of this item.
        public Item item;
        public int maxQuantity;
        public bool featured;

        // Whether this item is locked.
        public GameObject lockedPanel;
        public TextMeshProUGUI lockedPanelText;

        // sold out.
        public GameObject soldOutPanel;

        public void Draw(Item item) {
            // Set the ui components.
            nameText.text = item.name;
            rarityText.text = item.rarity.ToString();
            priceText.text = $"${item.price}";
            quantityText.text = item.quantity.ToString();

            SetRarityImage(rarityImage, item.rarity);
            SetIconImage(iconImage, item);
            
            this.item = item;

            if (item.quantity <= 0) {
                SetSoldOut();
            }

            //
            // maxQuantity = itemData.quantity;
            // featured = itemData.featured;
        }
        
        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, Item item) {
            // image.sprite = Resources.Load<Sprite>(item.iconPath);
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

    }

}
