// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class Inventory : MonoBehaviour {

        [System.Serializable]
        public class InventoryItem {
            public string itemId;
            public int quantity;
            public InventoryItem(string itemId) {
                this.itemId = itemId;
                this.quantity = 0;
            }
        }

        // Stores the inventory data retrieved from the database.
        [System.Serializable]
        public class InventoryData {
            public List<InventoryItem> items = new List<InventoryItem>(); 
        }

        // Triggers an event whenever the inventory changes.
        public UnityEvent<InventoryData> onInventoryChanged = new UnityEvent<InventoryData>();

        // Regularly updating stock within the inventory.
        public InventoryData stock;

        // Cache a reference to the player.
        public Player player;


        // Initializes the inventory.
        public async Task<bool> Initialize(Player player) {
            // Cache a reference to the player.
            this.player = player;
            try {
                StartCoroutine(IEQueryInventory());
                await default(Task);
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        private IEnumerator IEQueryInventory() {
            while (true) {
                UpdateInventory();
                yield return new WaitForSeconds(1f);
            }
        }

        public async Task UpdateInventory() {
            stock = await GetInventoryData();
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
