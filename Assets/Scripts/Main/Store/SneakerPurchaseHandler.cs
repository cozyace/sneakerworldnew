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
    /// Handles the functionality of purchasing a crate from the store.
    /// </summary>
    public class SneakerPurchaseHandler : MonoBehaviour {

        // The message to throw up if the player does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to purchase this sneaker!";

        // The message to throw up when the player makes a successful purchase.
        public const string SUCCESSFUL_PURCHASE_MESSAGE = "Successfully purchased this sneaker!";

        // An event on the beginning of the purchase.
        public UnityEvent onPurchaseStartEvent = new UnityEvent();

        // An event on a successful purchase.
        public UnityEvent<string, string> onPurchaseSuccessEvent = new UnityEvent<string, string>();

        // An event on a failed purchase.
        public UnityEvent<string> onPurchaseFailedEvent = new UnityEvent<string>();

        // Use this to debug the sneaker purchase while its happening.
        public SneakerData debugSneakerPurchase = new SneakerData();


        // Process the logic for signing the player up.
        [Button("Attempt Sneaker Purchase")]
        public async Task AttemptPurchase(Player player, string sneakerId, int quantity) {

            // Start the purchase event.
            onPurchaseStartEvent.Invoke();

            try {

                // Get the sneaker data.
                SneakerData sneakerData = await SneakerData.GetSneakerById(sneakerId);
                debugSneakerPurchase = sneakerData;

                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(quantity * sneakerData.price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                await player.inventory.AddSneakerByID(sneakerId, quantity);
                Debug.Log("Mananged to add sneaker.");

                Debug.Log(SUCCESSFUL_PURCHASE_MESSAGE);
                onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, sneakerId);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onPurchaseFailedEvent.Invoke(exception.Message);
            }

        }

    }

}

