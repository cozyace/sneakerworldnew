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
    public class Stock : InventorySystem {

        // Triggers an event whenever the inventory changes.
        public UnityEvent<Inventory> onStockUpdated = new UnityEvent<Inventory>();
        
        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await state.Init();
            Inventory inventory = await Get();
            onStockUpdated.Invoke(inventory);
        }

        [Button]
        public async void Delete() {
            await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.Inventory, new Inventory());
            Debug.Log("Deleted inventory");
        }

        public override async Task<Inventory> Get() {
            Inventory inventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {inventory!=null}");
            
            if (inventory == null) {
                inventory = new Inventory();
                await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.Inventory, inventory);
            }
            return inventory;
        }

        public override async Task Set(Inventory inventory) {
            await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.Inventory, inventory);
            onStockUpdated.Invoke(inventory);
        }

        public async Task Add(Item item) {
            Inventory inventory = await Get();
            inventory.Add(item);
            await Set(inventory);
        }

        public async Task Remove(Item item) {
            Inventory inventory = await Get();
            inventory.Remove(item);
            await Set(inventory);
        }

        public async Task<Item> Get(Item item) {
            Inventory inventory = await Get();
            Item item = inventory.Find(item);
            return item;
        }

        // public async Task PutItemOnSale(string itemId, bool onSale) {
        //     Inventory currentInventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.Inventory);
        //     StoreStateData stateData = await state.GetState();

        //     InventoryItem item = await GetItem(currentInventory, itemId);

        //     int currOnSale = currentInventory.sneakers.FindAll(s=>s.onSale).Count;
        //     if (currOnSale >= stateData.maxSneakersOnSale && onSale) {
        //         Player.instance.purchaser.onPurchaseFailedEvent.Invoke("Too many sneakers on sale!");
        //         onStockUpdated.Invoke(currentInventory);
        //         return;
        //     }

        //     if (item != null) {
        //         // Deduct the quantity.
        //         item.onSale = onSale;
        //         await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.Inventory, currentInventory);

        //         // Trigger any listeners.
        //         onStockUpdated.Invoke(currentInventory);
        //     }
        // }

        // public async Task AdjustMarkup(string itemId, float amount) {
        //     Inventory currentInventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.Inventory);
        //     InventoryItem item = await GetItem(currentInventory, itemId);
        //     if (item != null) {
        //         // Deduct the quantity.
        //         item.markup += amount;
        //         if (item.markup < -1f) {
        //             item.markup = -1f;
        //         }
        //         await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.Inventory, currentInventory);

        //         // Trigger any listeners.
        //         onStockUpdated.Invoke(currentInventory);
        //     }
        // }

    }

}
