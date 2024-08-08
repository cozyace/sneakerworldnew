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
    public class ItemSlotUI : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public Image rarityPanel;
        public Image mainPanel;

        public void Draw(CrateData crateData) {

            nameText.text = crateData.name;
            rarityText.text = crateData.rarity.ToString();
            priceText.text = $"${crateData.price}";

            RarityUtils.SetRarityPanel(rarityPanel, crateData.rarity);
            RarityUtils.SetRarityCrateIcon(mainPanel, crateData.rarity);

            // mainPanel.sprite = Resources.Load<Sprite>(crateData.imagePath);

        }

        public void Draw(SneakerData sneakerData) {

            nameText.text = sneakerData.name;
            rarityText.text = sneakerData.rarity.ToString();
            priceText.text = $"${sneakerData.price}";
            // rarityPanel.sprite = Resources.Load<Sprite>(crateData.imagePath);
            // Debug.Log(sneakerData.imagePath);
            // mainPanel.sprite = Resources.Load<Sprite>(sneakerData.imagePath);

        }


    }

}
