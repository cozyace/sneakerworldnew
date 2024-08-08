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

    using CrateData = SneakerWorld.Main.CrateData;
    using SneakerData = SneakerWorld.Main.SneakerData;
    using RarityUtils = SneakerWorld.Main.RarityUtils;

    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class FeaturedItemSlotUI : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public Image rarityPanel;
        public Image mainPanel;

        public void Draw(CrateData crateData) {

            nameText.text = crateData.name.Replace("_", " ");
            rarityText.text = crateData.rarity.ToString();
            priceText.text = $"${crateData.price}";

            RarityUtils.SetRarityPanel(rarityPanel, crateData.rarity);
            RarityUtils.SetRarityCrateIcon(mainPanel, crateData.rarity);

            // mainPanel.sprite = Resources.Load<Sprite>(crateData.imagePath);

        }


    }

}
