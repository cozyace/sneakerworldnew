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
    /// 
    /// </summary>
    public class Wallet : MonoBehaviour {

        // The cash in this player's wallet. NOTE: this is just for tracking, in reality, the firebase value is the one used.
        [SerializeField, ReadOnly]
        private int debugCash;

        // An event to trigger when a transaction has occured.
        public UnityEvent<int, int> onTransactionEvent = new UnityEvent<int, int>();

        // Cache a reference to the player.
        public Player player;


        // Initializes the user.
        public async Task<bool> Initialize(Player player) {
            this.player = player;
            try {
                debugCash = await GetBalance();
                StartCoroutine(IEQueryBalance());
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        private IEnumerator IEQueryBalance() {
            while (true) {
                UpdateBalance();
                yield return new WaitForSeconds(1f);
            }
        }

        public async Task UpdateBalance() {
            debugCash = await GetBalance();
        }

        public async Task<int> GetBalance() {
            return await FirebaseManager.GetDatabaseValue<int>(FirebasePath.Cash);
        }

        [Button("Attempt Debit")]
        public async Task<bool> Debit(int value) {
            int currentCash = await GetBalance();
            Debug.Log($"Attempting Debit of {value}. Current cash is {currentCash}");

            if (currentCash < value) {
                return false;
            }
            await FirebaseManager.SetDatabaseValue<int>(FirebasePath.Cash, currentCash - value);

            // Notify any listeners that a transaction event occurred.
            onTransactionEvent.Invoke(currentCash, -value);
            return true;
        }

        [Button("Attempt Credit")]
        public async Task Credit(int value) {
            int currentCash = await GetBalance();
            await FirebaseManager.SetDatabaseValue<int>(FirebasePath.Cash, currentCash + value);

            // Notify any listeners that a transaction event occurred.
            onTransactionEvent.Invoke(currentCash, value);
        }

    }

}
