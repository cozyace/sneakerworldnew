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

    public class SneakerInventoryItem : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public Image rarityImage;
        public Image iconImage;

        // The id of this item.
        public string itemId;

        public void Draw(string sneakerId, int quantity) {
            SneakerData sneakerItem = SneakerData.ParseId(sneakerId);

            // Set the ui components.
            nameText.text = sneakerItem.name;
            rarityText.text = sneakerItem.rarity.ToString();
            priceText.text = $"${sneakerItem.price}";
            quantityText.text = quantity.ToString();
            SetRarityImage(rarityImage, sneakerItem.rarity);
            SetIconImage(iconImage, sneakerItem);
            // Cache the id.
            itemId = sneakerItem.id;
        }

        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

    }

}
