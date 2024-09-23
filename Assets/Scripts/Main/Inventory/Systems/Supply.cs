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
    public class Supply : InventorySystem {

        // The store events.
        public UnityEvent<Inventory> onSupplyUpdated = new UnityEvent<Inventory>();

        // Used to roll for new stuff.
        public StoreRoller roller;

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await state.Init();
            Inventory inventory = await Get();
            onInventoryChanged.Invoke(inventory);
        }

        public override async Task<Inventory> Get() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            Inventory inventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.DailyStore(timeId));
            if (inventory == null) {
                inventory = new Inventory();
                inventory.Add(ItemMaker.GenerateRandomCrates(6));
                inventory.Add(ItemMaker.GenerateRandomSneakers(6));
            }
            
            await Set(inventory);
            return inventory;
        }

        public override async Task Set(Inventory inventory) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<StoreData>(FirebasePath.DailyStore(timeId), store);
            onStoreUpdated.Invoke(store);
        }

        public async Task<Inventory> Remove(Item item) {
            InventoryData inventory = await Get();
            inventory.Add(item);
            await Set(inventory);
            return inventory;
        }

        public async Task<Inventory> Remove(Item item) {
            InventoryData inventory = await Get();
            inventory.Add(item);
            await Set(inventory);
            return inventory;
        }

    }


}
