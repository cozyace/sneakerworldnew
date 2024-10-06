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
    /// Handles the functionality of purchasing something from the store.
    /// </summary>
    public class PurchaseHandler : MonoBehaviour {

        // An event on the beginning of the purchase.
        public UnityEvent<Item> onPurchaseStartEvent = new UnityEvent<Item>();
        public UnityEvent<Item, int> onProcessingPurchase = new UnityEvent<Item, int>();
        public UnityEvent<string> onPurchaseFailedEvent = new UnityEvent<string>();
        public UnityEvent<Item, string> onPurchaseSuccessEvent = new UnityEvent<Item, string>();

        // The quantity being attempted to purchase.
        public int quantity = 1;
        public int maxQuanity = 1;

        // The quantity being attempted to purchase.
        public int price = 0;
        public bool featuredItem = false;
        public int debugTotalCost = 0;

        Item cachedItem;

        public void SetQuantity(int value = 1) {
            if (cachedItem == null) { return; }
            
            cachedItem.quantity = value;
            onProcessingPurchase.Invoke(cachedItem, maxQuanity);
        }

        // The quantity being attempted to purchase.
        public void AddQuantity(int value = 1) {
            if (cachedItem == null) { return; }
            int quantity = cachedItem.quantity;

            quantity += value;
            if (quantity <= 0) {
                quantity = 1;
            }
            else if (quantity >= maxQuanity) {
                quantity = maxQuanity;
            }

            cachedItem.quantity = quantity;
            onProcessingPurchase.Invoke(cachedItem, maxQuanity);
        }

        public void StartPurchase(Item item, int maxQuanity, bool featuredItem) {
            this.maxQuanity = maxQuanity;
            cachedItem = item.Duplicate(1);
            SetQuantity(1);

            onPurchaseStartEvent.Invoke(item);
        }

        public void ExitPurchase() {
            cachedItem = null;
        }

        public void CompletePurchase() {
            CompletePurchase(cachedItem, Player.instance.stock);
        }

        // Process the logic for signing the player up.
        public async Task CompletePurchase(Item item, InventorySystem targetInventorySystem) {

            onProcessingPurchase.Invoke(item, maxQuanity);

            try {

                Inventory cachedInventory = await targetInventorySystem.Get();

                // Check if there is capacity.
                bool hasCapacity = cachedInventory.HasCapacity(cachedItem.itemType);
                if (!hasCapacity) {
                    throw new Exception("Not enough inventory space!");
                }

                // Check the player can afford the crate.
                bool hasFunds = await Player.instance.wallet.Debit(cachedItem.totalPrice);
                if (!hasFunds) {
                    throw new Exception("Not enough funds to purchase this crate!");
                }
                Debug.Log("Managed to process debit.");

                // Add the item to the inventory.
                cachedInventory.Add(cachedItem);
                targetInventorySystem.Set(cachedInventory);

                // Invoke the event.
                onPurchaseSuccessEvent.Invoke(cachedItem, "Successfully purchased this crate!");
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onPurchaseFailedEvent.Invoke(exception.Message);
            }

        }

    }

}

