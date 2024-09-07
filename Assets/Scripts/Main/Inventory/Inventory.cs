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

        public StoreUpgrader state;

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await state.Init();
            InventoryData inventory = await GetInventoryData();
            onInventoryChanged.Invoke(inventory);
        }

        [Button]
        public async void DeleteInventory() {
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, new InventoryData());
            Debug.Log("Deleted inventory");
        }

        public async Task<InventoryData> GetInventoryData() {
            InventoryData inventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            if (inventory == null) {
                inventory = new InventoryData();
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, inventory);
            }
            return inventory;
        }

        public async Task AddItemByID(string itemId, int quantity = 1) {
            // How does this handle not having a sneaker already?
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {currentInventory!=null}");

            // Make sure everything exists.
            if (currentInventory == null) {
                currentInventory = new InventoryData();
            }
            
            
            // Find the item.
            List<InventoryItem> searchList = new List<InventoryItem>();
            if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                searchList = currentInventory.sneakers;
            }
            else if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                searchList = currentInventory.crates;
            }

            if (searchList == null) {
                searchList = new List<InventoryItem>();
            }

            InventoryItem item = searchList.Find(item => item.itemId == itemId);
            if (item == null) {
                item = new InventoryItem(itemId);
                searchList.Add(item);
            }
            item.quantity += quantity;

            // Send the data back to the database.
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);
            
            // Trigger any listeners.
            onInventoryChanged.Invoke(currentInventory);

        }

        public async Task<InventoryItem> GetItem(InventoryData currentInventory, string itemId) {
            // How does this work if there is not a sneaker already?
            Debug.Log($"Managed to find inventory: {currentInventory!=null}");

            // Make sure everything exists.
            if (currentInventory == null) {
                currentInventory = new InventoryData();
            }

            // Add the item.
            List<InventoryItem> searchList = new List<InventoryItem>();
            if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                searchList = currentInventory.sneakers;
            }
            else if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                searchList = currentInventory.crates;
            }

            if (searchList == null) {
                searchList = new List<InventoryItem>();
            }

            InventoryItem item = searchList.Find(item => item.itemId == itemId);
            return item;
        }

        public async Task PutItemOnSale(string itemId, bool onSale) {
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            StoreStateData stateData = await state.GetState();

            InventoryItem item = await GetItem(currentInventory, itemId);

            int currOnSale = currentInventory.sneakers.FindAll(s=>s.onSale).Count;
            if (currOnSale >= stateData.maxSneakersOnSale && onSale) {
                Player.instance.purchaser.onPurchaseFailedEvent.Invoke("Too many sneakers on sale!");
                onInventoryChanged.Invoke(currentInventory);
                return;
            }

            if (item != null) {
                // Deduct the quantity.
                item.onSale = onSale;
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);

                // Trigger any listeners.
                onInventoryChanged.Invoke(currentInventory);
            }
        }

        public async Task AdjustMarkup(string itemId, float amount) {
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            InventoryItem item = await GetItem(currentInventory, itemId);
            if (item != null) {
                // Deduct the quantity.
                item.markup += amount;
                if (item.markup < -1f) {
                    item.markup = -1f;
                }
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);

                // Trigger any listeners.
                onInventoryChanged.Invoke(currentInventory);
            }
        }


        public async Task RemoveItemByID(string itemId, int quantity = 1) {
            // How does this work if there is not a sneaker already?
            InventoryData currentInventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {currentInventory!=null}");

            // Make sure everything exists.
            if (currentInventory == null) {
                currentInventory = new InventoryData();
            }

            // Add the item.
            List<InventoryItem> searchList = new List<InventoryItem>();
            if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                searchList = currentInventory.sneakers;
            }
            else if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                searchList = currentInventory.crates;
            }

            if (searchList == null) {
                searchList = new List<InventoryItem>();
            }

            InventoryItem item = searchList.Find(item => item.itemId == itemId);
            if (item != null) {
                
                // Deduct the quantity.
                item.quantity -= quantity;
                if (item.quantity <= 0) {
                    if (searchList.Contains(item)) { searchList.Remove(item); }

                    // if (currentInventory.sneakers.Contains(item)) { currentInventory.sneakers.Remove(item); }
                    // if (currentInventory.crates.Contains(item)) { currentInventory.crates.Remove(item); }
                }
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, currentInventory);

                // Trigger any listeners.
                onInventoryChanged.Invoke(currentInventory);
            }
            

        }


    }

}
