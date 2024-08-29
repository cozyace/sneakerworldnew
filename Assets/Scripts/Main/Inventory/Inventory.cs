// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// All the functionality for the inventory.
    /// </summary>
    public class Inventory : PlayerSystem {

        // Triggers an event whenever the inventory changes.
        public UnityEvent<InventoryData> onInventoryChanged = new UnityEvent<InventoryData>();


        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            InventoryData stock = await GetInventoryData();
            onInventoryChanged.Invoke(stock);
        }

        [Button]
        public async void DeleteInventory() {
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, new InventoryData());
            Debug.Log("Deleted inventory");
        }

        public async Task<InventoryData> GetInventoryData() {
            return await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
        }

        public async Task AddItemByID(string itemId, int quantity = 1) {
            // How does this handle not having a sneaker already?
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {currentInventory!=null}");

            // Make sure everything exists.
            if (currentInventory == null) {
                currentInventory = new InventoryData();
            }
            if (currentInventory.items == null) {
                currentInventory.items = new List<InventoryItem>();
            }
            
            // Add the item.
            InventoryItem item = currentInventory.items.Find(item => item.itemId == itemId);
            if (item == null) {
                item = new InventoryItem(itemId);
                currentInventory.items.Add(item);
            }
            item.quantity += quantity;

            // Send the data back to the database.
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);
            
            // Trigger any listeners.
            onInventoryChanged.Invoke(currentInventory);

        }

        public async Task RemoveItemByID(string itemId, int quantity = 1) {
            // How does this work if there is not a sneaker already?
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {currentInventory!=null}");

            // Make sure everything exists.
            if (currentInventory == null) {
                currentInventory = new InventoryData();
            }
            if (currentInventory.items == null) {
                currentInventory.items = new List<InventoryItem>();
            }

            // Add the item.
            InventoryItem item = currentInventory.items.Find(item => item.itemId == itemId);
            if (item != null && item.quantity > quantity) {
                
                // Deduct the quantity.
                item.quantity -= quantity;
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);

                // Trigger any listeners.
                onInventoryChanged.Invoke(currentInventory);
            }
            

        }


    }

}
