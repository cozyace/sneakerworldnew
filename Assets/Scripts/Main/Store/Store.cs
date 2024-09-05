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
    public class Store : PlayerSystem {


        // The message to throw up if the player does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to reroll!";

        // The message to throw up if the player does not have enough funds.
        public const string SUCCESSFUL_REROLL_MESSAGE = "Successfully rerolled!";

        // The store events.
        public UnityEvent<StoreData> onStoreUpdated = new UnityEvent<StoreData>();
        public UnityEvent<FeaturedStoreData> onFeaturedStoreUpdated = new UnityEvent<FeaturedStoreData>();

        // Reroll events.
        public UnityEvent<int> onRerollEvent = new UnityEvent<int>();

        // Used to roll for new stuff.
        public StoreRoller roller;

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await GetStore();
            await GetFeaturedStore();
        }

        public async Task<StoreData> GetStore() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            StoreData store = await FirebaseManager.GetDatabaseValue<StoreData>(FirebasePath.DailyStore(timeId));
            if (store == null) {
                store = new StoreData();
                store.crates = roller.RandomCrates(6);
                store.sneakers = roller.RandomSneakers(6);
            }
            
            await SetStore(store);
            return store;
        }

        public async Task SetStore(StoreData store) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<StoreData>(FirebasePath.DailyStore(timeId), store);
            onStoreUpdated.Invoke(store);
        }

        public async Task<FeaturedStoreData> GetFeaturedStore() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);
            Debug.Log($"Getting featured store data for {day}, {month}, {year}, {timeId}");

            FeaturedStoreData featuredStore = await FirebaseManager.GetDatabaseValue<FeaturedStoreData>(FirebasePath.MyFeaturedItemWithDate(timeId));
            if (featuredStore == null) {
                featuredStore = await FirebaseManager.GetDatabaseValue<FeaturedStoreData>(FirebasePath.FeaturedItemWithDate(timeId));
            }

            await SetFeaturedStore(featuredStore);
            return featuredStore;
        }

        public async Task SetFeaturedStore(FeaturedStoreData featuredStoreData) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<FeaturedStoreData>(FirebasePath.MyFeaturedItemWithDate(timeId), featuredStoreData);
            onFeaturedStoreUpdated.Invoke(featuredStoreData);
        }

        public async Task<StoreData> RemoveRegularItemById(string itemId, int quantity) {
            StoreData store = await GetStore();
            if (store == null) {
                return null;
            }

            InventoryItem sneaker = store.sneakers.Find(s => s.itemId == itemId);
            if (sneaker != null) {
                sneaker.quantity -= quantity;
            }
            InventoryItem crate = store.crates.Find(c => c.itemId == itemId);
            if (crate != null) {
                crate.quantity -= quantity;
            }
            await SetStore(store);

            onStoreUpdated.Invoke(store);
            return store;
        }

        public async Task<FeaturedStoreData> RemoveFeaturedItemById(string itemId, int quantity) {
            FeaturedStoreData fStore = await GetFeaturedStore();
            if (fStore == null) {
                return null;
            }

            InventoryItem sneaker = fStore.sneakers.Find(s => s.itemId == itemId);
            if (sneaker != null) {
                sneaker.quantity -= quantity;
            }
            InventoryItem crate = fStore.crates.Find(c => c.itemId == itemId);
            if (crate != null) {
                crate.quantity -= quantity;
            }
            await SetFeaturedStore(fStore);

            onFeaturedStoreUpdated.Invoke(fStore);
            return fStore;
        }

    }

}
