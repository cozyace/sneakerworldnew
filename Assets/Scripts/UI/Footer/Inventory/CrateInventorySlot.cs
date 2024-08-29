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

    public class CrateInventorySlot : InventorySlot {

        public override void Draw(string crateId, int quantity) {
            CrateData crateItem = CrateData.ParseId(crateId);

            // Set the ui components.
            nameText.text = crateItem.name;
            rarityText.text = crateItem.rarity.ToString();
            if (priceText != null) { priceText.text = $"${crateItem.price}"; }
            if (quantityText != null) { quantityText.text = quantity.ToString(); }
            SetRarityImage(rarityImage, crateItem.rarity);
            SetIconImage(iconImage, crateItem);
            // Cache the id.
            itemId = crateItem.id;
            this.quantity = quantity; 
        }

    }

}
