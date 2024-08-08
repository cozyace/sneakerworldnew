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
        public UnityEvent<int, int> onQuantityChanged = new UnityEvent<int, int>();

        // The quantity being attempted to purchase.
        public int quantity = 1;

        // The quantity being attempted to purchase.
        public int price = 0;

        private CrateData crateData = null;
        private SneakerData sneakerData = null;
        string cachedItemId = "";

        public void SetQuantity(int value = 1) {
            quantity = value;
            onQuantityChanged.Invoke(quantity, price);
        }

        // The quantity being attempted to purchase.
        public void AddQuantity(int value = 1) {
            quantity += value;
            if (quantity <= 0) {
                quantity = 1;
            }
            onQuantityChanged.Invoke(quantity, price);
        }

        public void AttemptPurchase(UI.ItemSlotUI itemUi) {
            StartPurchase(itemUi.itemId);
            Debug.Log($"Attempting purchase of : {itemUi.itemId}");
        }


        // Process the logic for signing the player up.
        public void StartPurchase(string itemId) {
            crateData = null;
            sneakerData = null;

            crateData = CrateData.ParseId(itemId);
            if (crateData != null) {
                Debug.Log("had crate data");

                onPurchaseCrateStartEvent.Invoke(crateData);
                price = crateData.price;
            }
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

                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(quantity * price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                await player.inventory.AddItemByID(itemId, quantity);
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

