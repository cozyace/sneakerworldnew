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

        // The message to throw up if the player does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to purchase this crate!";

        // The message to throw up when the player makes a successful purchase.
        public const string SUCCESSFUL_PURCHASE_MESSAGE = "Successfully purchased this crate!";

        // An event on the beginning of the purchase.
        public UnityEvent<Item> onPurchaseStartEvent = new UnityEvent<Item>();
        public UnityEvent onProcessingPurchase = new UnityEvent();
        public UnityEvent<int, int, int> onQuantityChanged = new UnityEvent<int, int, int>();
        public UnityEvent<string> onPurchaseFailedEvent = new UnityEvent<string>();
        public UnityEvent<string, string, int> onPurchaseSuccessEvent = new UnityEvent<string, string, int>();

        // The quantity being attempted to purchase.
        public int quantity = 1;
        public int maxQuanity = 1;

        // The quantity being attempted to purchase.
        public int price = 0;
        public bool featuredItem = false;
        public int debugTotalCost = 0;

        Item cachedItem;

        public void SetQuantity(int value = 1) {
            quantity = value;
            onQuantityChanged.Invoke(quantity, maxQuanity, price);
        }

        // The quantity being attempted to purchase.
        public void AddQuantity(int value = 1) {
            quantity += value;
            if (quantity <= 0) {
                quantity = 1;
            }
            else if (quantity >= maxQuanity) {
                quantity = maxQuanity;
            }
            onQuantityChanged.Invoke(quantity, maxQuanity, price);
        }

        public void StartPurchase(Item item, int maxQuanity, bool featuredItem) {

            onPurchaseStartEvent.Invoke(item);
            this.price = item.price;
            this.maxQuanity = maxQuanity;
            // this.featuredItem = featuredItem;
            cachedItem = item;

            SetQuantity(1);

        }

        public void ExitPurchase() {
            cachedItem = null;
        }

        public void CompletePurchase() {
            CompletePurchase(cachedItem, Player.instance.inventory);
        }

        // Process the logic for signing the player up.
        public async Task CompletePurchase(Item item, int quantity, InventorySystem to) {

            onProcessingPurchase.Invoke();
            item = item.Duplicate(quantity);

            try {

                InventoryData inventory = await to.Inventory();

                // Check if there is capacity.
                bool hasCapacity = inventory.HasCapacity(item);
                if (!hasCapacity) {
                    throw new Exception("Not enough inventory space!");
                }

                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(quantity * price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the item to the inventory.
                inventory.Add(item);
                to.Set(inventory);
                Debug.Log("Mananged to add item.");

                Debug.Log(SUCCESSFUL_PURCHASE_MESSAGE);
                onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, item);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onPurchaseFailedEvent.Invoke(exception.Message);
            }

        }

    }

}

