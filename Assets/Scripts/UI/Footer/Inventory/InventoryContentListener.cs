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
    public class InventoryContentListener : MonoBehaviour {

        // Sneakers.
        public InventoryContentDrawer sneakerContent;
        public InventoryContentDrawer cratesContent;

        public InventoryData stock;

        // Runs once on instantiation.
        void Awake() {
            Player.instance.inventory.onInventoryChanged.AddListener(DrawInventory);
        }
        
        // Draw the inventory ui.
        public void DrawInventory(InventoryData inventoryData) {
            // Cache the inventory data.
            stock = inventoryData;
            sneakerContent.Draw(inventoryData.sneakers);
            cratesContent.Draw(inventoryData.crates);
        }

    }

}
