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

        // 
        public const string SUCCESSFUL_REROLL_MESSAGE = "Successfully rerolled!";

        // Stores data from the personal store.
        [System.Serializable]
        public class RegularStoreData {
            public List<CrateData> crates = new List<CrateData>();
            public List<SneakerData> sneakers = new List<SneakerData>();
        }

        // Stores data from the featured store.
        [System.Serializable]
        public class FeaturedStoreData {
            public List<CrateData> featuredCrates = new List<CrateData>();
            public List<SneakerData> featuredSneakers = new List<SneakerData>();
        }

        // The crate events.
        public UnityEvent<List<CrateData>> onFeaturedCratesUpdated = new UnityEvent<List<CrateData>>();
        public UnityEvent<List<CrateData>> onCratesUpdated = new UnityEvent<List<CrateData>>();

        // The sneaker events.
        public UnityEvent<List<SneakerData>> onSneakersUpdated = new UnityEvent<List<SneakerData>>();
        public UnityEvent<List<SneakerData>> onFeaturedSneakersUpdated = new UnityEvent<List<SneakerData>>();


        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await GetStore();
            await GetFeaturedStore();
        }

        public async Task<RegularStoreData> GetStore() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            RegularStoreData regularStore = await FirebaseManager.GetDatabaseValue<RegularStoreData>(FirebasePath.DailyStore(timeId));
            if (regularStore == null) {
                regularStore = new RegularStoreData();
                regularStore.crates = RandomCrates(6, false);
                regularStore.sneakers = RandomSneakers(6);
                await SetStore(regularStore);
            }
            onCratesUpdated.Invoke(regularStore.crates);
            onSneakersUpdated.Invoke(regularStore.sneakers);
            return regularStore;
        }

        public async Task SetStore(RegularStoreData regularStore) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<RegularStoreData>(FirebasePath.DailyStore(timeId), regularStore);
            onCratesUpdated.Invoke(regularStore.crates);
            onSneakersUpdated.Invoke(regularStore.sneakers);
        }

        public async Task<FeaturedStoreData> GetFeaturedStore() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);
            Debug.Log($"Getting featured store data for {day}, {month}, {year}, {timeId}");

            bool newDay = false;

            FeaturedStoreData featuredStore = await FirebaseManager.GetDatabaseValue<FeaturedStoreData>(FirebasePath.MyFeaturedItemWithDate(timeId));
            if (featuredStore == null) {
                featuredStore = await FirebaseManager.GetDatabaseValue<FeaturedStoreData>(FirebasePath.FeaturedItemWithDate(timeId));
                foreach (SneakerData s in featuredStore.featuredSneakers) {
                    s.quantity = 1;
                    s.featured = true;
                }
                foreach (CrateData c in featuredStore.featuredCrates) {
                    c.quantity = 1;
                    c.featured = true;
                }
            }

            await SetFeaturedStore(featuredStore);
            onFeaturedCratesUpdated.Invoke(featuredStore.featuredCrates);
            onFeaturedSneakersUpdated.Invoke(featuredStore.featuredSneakers);
            return featuredStore;
        }

        public async Task SetFeaturedStore(FeaturedStoreData featuredStoreData) {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            string timeId = FeaturedItem.GetTimeId(day, month, year);

            await FirebaseManager.SetDatabaseValue<FeaturedStoreData>(FirebasePath.MyFeaturedItemWithDate(timeId), featuredStoreData);
            onFeaturedCratesUpdated.Invoke(featuredStoreData.featuredCrates);
            onFeaturedSneakersUpdated.Invoke(featuredStoreData.featuredSneakers);
        }

        public void RerollSneakers() {
            AsyncRerollSneakers(6);
        }

        [Button]
        public async Task AsyncRerollSneakers(int sneakerCount) {
            int price = 100;

            try {
                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                RegularStoreData regularStore = await GetStore();
                regularStore.sneakers = RandomSneakers(sneakerCount);
                await SetStore(regularStore);

                Debug.Log(SUCCESSFUL_REROLL_MESSAGE);
                // onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, itemId, quantity);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                // onPurchaseFailedEvent.Invoke(exception.Message);
            }
            
        }

        public void RerollCrates() {
            AsyncRerollCrates(6);
        }

        [Button]
        public async Task AsyncRerollCrates(int crateCount) {
            int price = 100;

            try {
                // Check the player can afford the crate.
                bool hasFunds = await player.wallet.Debit(price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                RegularStoreData regularStore = await GetStore();
                regularStore.crates = RandomCrates(crateCount, false);
                await SetStore(regularStore);

                Debug.Log(SUCCESSFUL_REROLL_MESSAGE);
                // onPurchaseSuccessEvent.Invoke(SUCCESSFUL_PURCHASE_MESSAGE, itemId, quantity);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                // onPurchaseFailedEvent.Invoke(exception.Message);
            }
        }

        public async Task<RegularStoreData> RemoveRegularItemById(string itemId, int quantity) {
            RegularStoreData regularStore = await GetStore();
            if (regularStore == null) {
                return null;
            }

            SneakerData sneaker = regularStore.sneakers.Find(s => s.id == itemId);
            if (sneaker != null) {
                sneaker.quantity -= quantity;
            }
            CrateData crate = regularStore.crates.Find(c => c.id == itemId);
            if (crate != null) {
                crate.quantity -= quantity;
            }
            await SetStore(regularStore);

            onCratesUpdated.Invoke(regularStore.crates);
            onSneakersUpdated.Invoke(regularStore.sneakers);
            return regularStore;
        }

        public async Task<FeaturedStoreData> RemoveFeaturedItemById(string itemId, int quantity) {
            FeaturedStoreData featuredStore = await GetFeaturedStore();
            if (featuredStore == null) {
                return null;
            }

            SneakerData sneaker = featuredStore.featuredSneakers.Find(s => s.id == itemId);
            if (sneaker != null) {
                sneaker.quantity -= quantity;
            }
            CrateData crate = featuredStore.featuredCrates.Find(c => c.id == itemId);
            if (crate != null) {
                crate.quantity -= quantity;
            }
            await SetFeaturedStore(featuredStore);

            onFeaturedSneakersUpdated.Invoke(featuredStore.featuredSneakers);
            onFeaturedCratesUpdated.Invoke(featuredStore.featuredCrates);
            return featuredStore;
        }



        // [Button]
        public List<CrateData> AllCrates() {
            List<CrateData> crateData = new List<CrateData>();

            int rarityCount = (int)Rarity.Count;
            int brandCount = (int)Brand.Count;
            for (int j = 0; j < brandCount; j++) {
                for (int i = 0; i < rarityCount; i++) {
                    crateData.Add(new CrateData((Brand)j, (Rarity)i));
                }
            }

            crateData.Sort((x, y) => x.price.CompareTo(y.price));
            return crateData;
        }

        // [Button]
        public List<SneakerData> RandomSneakers(int count) {
            List<CrateData> crateData = RandomCrates(count, true);
            List<SneakerData> sneakers = new List<SneakerData>();
            
            foreach (CrateData crate in crateData) {
                SneakerData sneaker = crate.GetRandomSneakerFromCrate();
                SneakerData existingSneaker = sneakers.Find(s => s.id == sneaker.id);

                int depth = 0;
                while (existingSneaker != null && depth < 200) {
                    sneaker = crate.GetRandomSneakerFromCrate();
                    existingSneaker = sneakers.Find(s => s.id == sneaker.id);
                    depth += 1;
                }

                sneaker.quantity = UnityEngine.Random.Range(5, 10); 
                sneakers.Add(sneaker);
            }

            sneakers.Sort((x, y) => x.level.CompareTo(y.level));
            return sneakers;

        }

        // [Button]
        public List<CrateData> RandomCrates(int count, bool allowRepeats) {

            List<CrateData> crateData = AllCrates();
            List<CrateData> tmp = new List<CrateData>();
            List<int> cachedIndices = new List<int>();
            int crateCount = crateData.Count;

            int i = 0;
            int depth = 0; 
            while (i < count && depth < 200) {

                int index = UnityEngine.Random.Range(0, crateCount);
                if (allowRepeats || !cachedIndices.Contains(index)) {
                    crateData[index].quantity = UnityEngine.Random.Range(5, 10);
                    tmp.Add(crateData[index]);
                    cachedIndices.Add(index);
                    i += 1;
                }

                depth += 1;

            }
            tmp.Sort((x, y) => x.level.CompareTo(y.level));
            return tmp;
        }

    }

}
