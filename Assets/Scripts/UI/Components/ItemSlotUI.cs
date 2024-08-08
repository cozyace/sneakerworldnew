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

    using ItemData = SneakerWorld.Main.ItemData;
    using Rarity = SneakerWorld.Main.Rarity;
    // using RarityUtils = SneakerWorld.Main.RarityUtils;

    public class ItemSlotUI : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public Image rarityPanel;
        public Image mainPanel;

        //
        public string itemId;

        public void Draw<TItemData>(TItemData itemData)
            where TItemData : ItemData {

            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();
            priceText.text = $"${itemData.price}";

            SetRarityPanel(rarityPanel, itemData.rarity);
            SetIcon(mainPanel, itemData);

            itemId = itemData.id;

            // mainPanel.sprite = Resources.Load<Sprite>(crateData.imagePath);

        }

        public static void SetRarityPanel(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIcon(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }


    }

}
