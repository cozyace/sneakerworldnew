// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Listens to the inventory and updates the UI accordingly. 
    /// </summary>
    public class InventorySizePanel : MonoBehaviour {

        public TextMeshProUGUI sneakerAmount;
        public TextMeshProUGUI sneakersSellingAmount;

        public TextMeshProUGUI crateAmount;

        // Runs once on instantiation.
        void Awake() {
            Player.instance.inventory.onInventoryChanged.AddListener(Draw);
        }
        
        async void Draw(InventoryData data) {
            StoreStateData invState = await Player.instance.inventory.state.GetState();
            sneakerAmount.text = $"{data.sneakers.Count}/{invState.inventorySneakerMax} Sneakers Owned";

            int count = data.sneakers.FindAll(s => s.onSale).Count;
            sneakersSellingAmount.text = $"{count}/{invState.maxSneakersOnSale} On Sale";

            crateAmount.text = $"{data.crates.Count}/{invState.inventoryCratesMax} Crates Owned";
        }
        

    }

}
