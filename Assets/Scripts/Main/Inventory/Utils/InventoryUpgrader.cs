// System.
using System;
using System.Linq;
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
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class InventoryUpgrader : MonoBehaviour {

        // The message to throw up if the player does not have enough funds.
        public UnityEvent<string> onUpgradeFailed = new UnityEvent<string>();
        public UnityEvent<string> onUpgradeSuccess = new UnityEvent<string>();

        // Set the username.
        public static async Task TryUpgrade(InventorySystem inventorySystem) {
            Inventory inventory = await inventorySystem.Get();

            try {

                // Check if the inventory is upgrading.
                if (inventory.isUpgrading) {
                    throw new Exception("Already upgrading");
                }

                // Check the Player can afford the crate.
                int price = GetUpgradePrice(inventory.level);
                bool hasFunds = await Player.instance.wallet.Debit(price);
                if (!hasFunds) {
                    throw new Exception("Not enough funds to upgrade!");
                }
                Debug.Log("Managed to process debit.");

                // Start the inventory upgrade.
                inventory.StartUpgrade(DateTime.UtcNow.ToBinary(), 
                    GetUpgradeDuration(inventory.level));
                
                await inventorySystem.Set(inventory);              
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onUpgradeFailed.Invoke(exception.Message);

                await inventorySystem.Set(inventory);              
            }
            
        }

        private static bool checking = false; // since this is no longer per system, needs to be a dictionary
        // tied to the one it was sent with.
        public static async Task CheckFinishedUpgrading(InventorySystem inventorySystem) {
            
            // So that only one check can run at a time.
            if (checking) { return; }
            checking = true;

            // Check the upgrade state.
            Inventory inventory = await inventorySystem.Get();
            bool upgraded = inventory.CheckUpgrade(DateTime.UtcNow);
            if (upgraded) {
                inventorySystem.Set(inventory);
            }

            // Note that this check has finished.
            checking = false;

        }

        public static int GetUpgradePrice(int level) {
            return (int)(200f * Mathf.Pow(2f, (float)level));
        }

        public static float GetUpgradeDuration(int level) {
            return Mathf.Min(60f*60f*24f*7f, 2f * Mathf.Pow(2f, (float)level));
        }

    }


}
