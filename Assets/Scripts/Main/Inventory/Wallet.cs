// System.
using System;
using System.Collections;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// Stores all the cash and gems that the player has. 
    /// </summary>
    public class Wallet : PlayerSystem {

        // An event to trigger when a transaction has occured.
        public UnityEvent<int, int> onCashTransactionEvent = new UnityEvent<int, int>();
        public UnityEvent<int, int> onGemsTransactionEvent = new UnityEvent<int, int>();

        // An event to trigger when this system has been initialized.
        public UnityEvent<int> onCashInitEvent = new UnityEvent<int>();
        public UnityEvent<int> onGemsInitEvent = new UnityEvent<int>();

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            int cash = await GetCash();
            int gems = await GetGems();
            onCashInitEvent.Invoke(cash);
            onGemsInitEvent.Invoke(gems);
        }

        // Get the player's cash balance. 
        public async Task<int> GetCash() {
            return await FirebaseManager.GetDatabaseValue<int>(FirebasePath.Cash);
        }

        // Get the player's gem balance. 
        public async Task<int> GetGems() {
            return await FirebaseManager.GetDatabaseValue<int>(FirebasePath.Gems);
        }

        // Debit cash from the players wallet.
        [Button]
        public async Task<bool> Debit(int value) {
            int currentCash = await GetCash();
            Debug.Log($"Attempting debit of {value} from {player.id}. Current cash is {currentCash}");

            if (currentCash < value) {
                Debug.Log($"Could not debit {value} from {player.id}. Did not have enough cash.");
                return false;
            }

            await FirebaseManager.SetDatabaseValue<int>(FirebasePath.Cash, currentCash - value);
            Debug.Log($"Debited {player.id} for {value}. Cash is now {currentCash - value}");

            // Notify any listeners that a transaction event occurred.
            onCashTransactionEvent.Invoke(currentCash, -value);
            return true;
        }

        // Credit cash to the players wallet.
        [Button]
        public async Task Credit(int value) {
            int currentCash = await GetCash();
            await FirebaseManager.SetDatabaseValue<int>(FirebasePath.Cash, currentCash + value);
            Debug.Log($"Credited {player.id} for {value}. Cash is now {currentCash + value}");

            // Notify any listeners that a transaction event occurred.
            onCashTransactionEvent.Invoke(currentCash, value);
        }

        // Credit gems to the players wallet.
        [Button]
        public async Task CreditGems(int value) {
            int currentGems = await GetGems();
            await FirebaseManager.SetDatabaseValue<int>(FirebasePath.Gems, currentGems + value);
            Debug.Log($"Credited {player.id} for {value} gems. Gems is now {currentGems + value}");

            // Notify any listeners that a transaction event occurred.
            onGemsTransactionEvent.Invoke(currentGems, value);
        }

    }

}
