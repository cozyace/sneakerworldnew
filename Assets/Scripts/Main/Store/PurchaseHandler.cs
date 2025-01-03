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
        public UnityEvent<CrateData> onPurchaseCrateStartEvent = new UnityEvent<CrateData>();
        public UnityEvent<SneakerData> onPurchaseSneakerStartEvent = new UnityEvent<SneakerData>();

        //
        public UnityEvent onProcessingPurchase = new UnityEvent();

        // An event on a successful purchase.
        public UnityEvent<string, string, int> onPurchaseSuccessEvent = new UnityEvent<string, string, int>();

        // An event on a failed purchase.
        public UnityEvent<string> onPurchaseFailedEvent = new UnityEvent<string>();

        // An event
        public UnityEvent<int, int, int> onQuantityChanged = new UnityEvent<int, int, int>();

        // The quantity being attempted to purchase.
        public int quantity = 1;
        public int maxQuanity = 1;

        // The quantity being attempted to purchase.
        public int price = 0;
        public bool featuredItem = false;
        public int debugTotalCost = 0;

        private CrateData crateData = null;
        private SneakerData sneakerData = null;
        string cachedItemId = "";

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

        public void StartPurchase(string itemId, int maxQuanity, bool featuredItem) {
            crateData = null;
            sneakerData = null;

            crateData = CrateData.ParseId(itemId);
            if (crateData != null) {
                // Debug.Log("had crate data");

                onPurchaseCrateStartEvent.Invoke(crateData);
                price = crateData.price;
            }
            sneakerData = SneakerData.ParseId(itemId);
            if (sneakerData != null) {
                // Debug.Log("had sneaker data");

                onPurchaseSneakerStartEvent.Invoke(sneakerData);
                this.price = sneakerData.price;
            }

            this.maxQuanity = maxQuanity;
            this.featuredItem = featuredItem;
            cachedItemId = itemId;

            SetQuantity(1);

        }

        public void ExitPurchase() {
            crateData = null;
            sneakerData = null;
        }

        public void CompletePurchase(Player player) {
            CompletePurchase(player, cachedItemId, quantity);
        }

        // Process the logic for signing the player up.
        public async Task CompletePurchase(Player player, string itemId, int quantity) {

            onProcessingPurchase.Invoke();

            try {

                InventoryData inventory = await player.inventory.GetInventoryData();
                StoreStateData state = await player.inventory.state.GetState();

                // Check if there is space.
                bool hasSpace = false;
                if (itemId.Contains(SneakerData.SNEAKER_ID_PREFIX)) {
                    hasSpace = inventory.sneakers.Find(item => item.itemId == itemId) != null || inventory.sneakers.Count < state.inventorySneakerMax;
                }
                else if (itemId.Contains(CrateData.CRATE_ID_PREFIX)) {
                    hasSpace = inventory.crates.Find(item => item.itemId == itemId) != null || inventory.crates.Count < state.inventoryCratesMax;
                }
                if (!hasSpace) {
                    throw new Exception("Not enough inventory space!");
                }

                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(quantity * price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                await player.inventory.AddItemByID(itemId, quantity);
                if (featuredItem) {
                    await player.store.RemoveFeaturedItemById(itemId, quantity);
                }
                else {
                    await player.store.RemoveRegularItemById(itemId, quantity);
                }
                Debug.Log("Mananged to add item.");

                Debug.Log(SUCCESSFUL_PURCHASE_MESSAGE);
                onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, itemId, quantity);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onPurchaseFailedEvent.Invoke(exception.Message);
            }

        }

    }

}

