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

    using CrateItem = CrateData.CrateItem;

    /// <summary>
    /// Handles the functionality of purchasing a crate from the store.
    /// </summary>
    public class CratePurchaseHandler : MonoBehaviour {

        // The message to throw up if the player does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to purchase this crate!";

        // The message to throw up when the player makes a successful purchase.
        public const string SUCCESSFUL_PURCHASE_MESSAGE = "Successfully purchased this crate!";

        // An event on the beginning of the purchase.
        public UnityEvent onPurchaseStartEvent = new UnityEvent();

        // An event on a successful purchase.
        public UnityEvent<string, string> onPurchaseSuccessEvent = new UnityEvent<string, string>();

        // An event on a failed purchase.
        public UnityEvent<string> onPurchaseFailedEvent = new UnityEvent<string>();

        // Use this to debug the crate purchase while its happening.
        public CrateItem debugCratePurchase = new CrateItem();


        // Process the logic for signing the player up.
        [Button("Attempt Crate Purchase")]
        public async Task AttemptPurchase(Player player, string crateId) {

            // Start the purchase event.
            onPurchaseStartEvent.Invoke();

            try {

                // Pre-emptively select the sneaker from the crate.
                CrateData crateData = await CrateData.GetCrateById(crateId);
                CrateItem crateItem = crateData.GetRandomItemFromCrate();

                debugCratePurchase = crateItem;

                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(crateData.price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                await player.inventory.AddSneakerByID(crateItem.sneakerId, crateItem.quantity);
                Debug.Log("Mananged to add sneaker.");

                Debug.Log(SUCCESSFUL_PURCHASE_MESSAGE);
                onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, crateItem.sneakerId);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onPurchaseFailedEvent.Invoke(exception.Message);
            }

        }

    }

}

