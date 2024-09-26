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

    [System.Serializable]
    public class InventoryRoller {

        // Reroll events.
        public UnityEvent<int> onRerollSneakersEvent = new UnityEvent<int>();
        public UnityEvent<int> onRerollCratesEvent = new UnityEvent<int>();

        public UnityEvent<int, int> onRerollInit = new UnityEvent<int, int>();

        // The price for rerolling
        public int pricePerReroll = 100;


        public async Task Reroll(InventorySystem inventorySystem, ItemType itemType) {
            Inventory inventory = await inventorySystem.Get();

            try {
                
                // Cache the validation values.
                int count = inventory.GetMaxCapacity(itemType);
                int currRerolls = inventory.GetRerolls(itemType);
                int cost = GetRerollCost(currRerolls);

                // Check the Player.instance can afford the crate.
                bool hasFunds = await Player.instance.wallet.Debit(cost);
                if (!hasFunds) {
                    throw new Exception("Not enough funds to reroll!");
                }
                Debug.Log("Managed to process debit.");

                // Replace the list.
                List<Item> items = ItemMaker.Random(itemType, count);
                inventory.Clear(itemType);
                inventory.Add(items);

                // Update the rerolls.
                inventory.AddReroll(itemType);
                int nextRollPrice = GetRollPrice(currRerolls+1);

                await inventorySystem.Set(inventory);

                Debug.Log("Successfully rerolled!");
                onRerollEvent.Invoke(inventory);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
            }
            
        }

        public int GetRerollCost(int currRolls) {
            return pricePerReroll * (currRolls + 1);
        }

    }

}
