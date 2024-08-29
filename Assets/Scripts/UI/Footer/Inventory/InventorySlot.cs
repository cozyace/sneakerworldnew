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

    public abstract class InventorySlot : MonoBehaviour {

        // public ItemIdParser itemIdParser;

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

        public abstract void Draw(string itemId, int quantity);
        
        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

    }

}
