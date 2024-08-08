// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    using Inventory = Main.Inventory;
    using InventoryData = Main.Inventory.InventoryData;
    using InventoryItem = Main.Inventory.InventoryItem;

    /// <summary>
    /// Listens to the inventory and updates the UI accordingly. 
    /// </summary>
    public class InventoryUI : MonoBehaviour {

        // A reference to the inventory.
        [SerializeField]
        private Inventory inventory;

        // Runs once before the first frame.
        void Start() {
            inventory.onInventoryChanged.AddListener(DrawInventory);
        }
        
        // Draw the inventory ui.
        public static void DrawInventory(InventoryData inventoryData) {
            for (int i = 0; i < inventoryData.sneakers.Count; i++) {
                // DrawSneakerItem(inventoryData.sneakers[i]);
            }

        }

        public static void DrawSneakerItem(InventoryItem item) {
            // Instantiate(sneakerItemPrefab);

            // SneakerData sneakerData = Main.SneakerData.GetSneakerById(item.itemId);
            // sneakerItemPrefab.name.text = sneakerData.name;
            // sneakerItemPrefab.version.text = version.ToString();
            // sneakerItemPrefab.SetSigned(sneaker)

        }

    }

}
