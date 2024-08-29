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

    public class SneakerInventorySlot : InventorySlot {

        public override void Draw(string sneakerId, int quantity) {
            SneakerData sneakerItem = SneakerData.ParseId(sneakerId);

            // Set the ui components.
            nameText.text = sneakerItem.name;
            rarityText.text = sneakerItem.rarity.ToString();
            if (priceText != null) { priceText.text = $"${sneakerItem.price}"; }
            quantityText.text = quantity.ToString();
            SetRarityImage(rarityImage, sneakerItem.rarity);
            SetIconImage(iconImage, sneakerItem);
            // Cache the id.
            itemId = sneakerItem.id;
            this.quantity = quantity; 
        }

    }

}
