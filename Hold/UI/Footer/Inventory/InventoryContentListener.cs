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

        public InventorySystem inventorySystem;
        // public Inventory stock;

        // Runs once on instantiation.
        void Awake() {
            inventorySystem.onInventoryChanged.AddListener(DrawInventory);
        }
        
        // Draw the inventory ui.
        public void DrawInventory(Inventory inventory) {
            // Cache the inventory data.
            // stock = inventory;
            sneakerContent.Draw(inventory.sneakers);
            cratesContent.Draw(inventory.crates);
        }

    }

}
