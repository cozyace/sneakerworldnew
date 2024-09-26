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
    public class FeaturedSupply : InventorySystem {

        // The store events.
        public UnityEvent<Inventory> onFeaturedSupplyUpdated = new UnityEvent<Inventory>();

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await state.Init();
            await Get();
        }

        public override async Task<Inventory> Get() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);
            Debug.Log($"Getting featured store data for {day}, {month}, {year}, {timeId}");

            Inventory inventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.MyFeaturedItemWithDate(timeId));
            if (inventory == null) {
                inventory = await FirebaseManager.GetDatabaseValue<Inventory>(FirebasePath.FeaturedItemWithDate(timeId));
            }

            await Set(inventory);
            return inventory;
        }

        public override async Task Set(Inventory inventory) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<Inventory>(FirebasePath.MyFeaturedItemWithDate(timeId), featuredStoreData);
            onFeaturedStoreUpdated.Invoke(featuredStoreData);
        }

        public async Task<Inventory> Remove(Item item) {
            Inventory inventory = await Get();
            inventory.Remove(item);
            await Set(inventory);
            return inventory;
        }

    }


}
