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

    public class CrateInventoryItem : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public Image rarityImage;
        public Image iconImage;

        // The id of this item.
        public string itemId;

        public void Draw(string crateId, int quantity) {
            CrateData crateItem = CrateData.ParseId(crateId);

            // Set the ui components.
            nameText.text = crateItem.name;
            rarityText.text = crateItem.rarity.ToString();
            priceText.text = $"${crateItem.price}";
            quantityText.text = quantity.ToString();
            SetRarityImage(rarityImage, crateItem.rarity);
            SetIconImage(iconImage, crateItem);
            // Cache the id.
            itemId = crateItem.id;
        }

        // public ays OpenCrate() {
        //     Player.instance.
        // }

        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }

    }

}
