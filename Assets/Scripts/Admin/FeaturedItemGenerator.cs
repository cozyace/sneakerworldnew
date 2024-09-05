// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
// Unity.
using UnityEngine;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Admin {

    using Main;

    /// <summary>
    /// A simple data structure to store / construct market events with.
    /// </summary>
    [System.Serializable]
    public class FeaturedItemGenerator : MonoBehaviour {

        [Button]
        private async Task GenerateFeaturedItemsForAMonth(int month, int year) {
            for (int i = 1; i < 32; i++) {
                GenerateFeaturedItem(i, month, year);
            }
        }


        [Button]
        private async Task GenerateFeaturedItem(int day, int month, int year) {
            FeaturedStoreData featuredStoreData = new FeaturedStoreData();
            featuredStoreData.sneakers = FeaturedSneakers();
            featuredStoreData.crates = FeaturedCrates();

            FirebaseManager.SetDatabaseValue<FeaturedStoreData>(FirebasePath.FeaturedItemWithDate(FeaturedItem.GetTimeId(day, month, year)), featuredStoreData);
        }

        public List<InventoryItem> FeaturedSneakers() {
            List<InventoryItem> sneakers = new List<InventoryItem>();
            
            for (int j = 0; j < 2; j++) {
                Brand brand = (Brand)(UnityEngine.Random.Range(0, (int)Brand.Count));
                SneakerData sneaker = new SneakerData(brand, Edition.Original, Condition.Mint);
                sneakers.Add(new InventoryItem(sneaker.id, 1));
            }
            
            return sneakers;

        }

        public List<InventoryItem> FeaturedCrates() {
            List<InventoryItem> featuredCrates = new List<InventoryItem>();

            int brandCount = (int)Brand.Count;
            for (int j = 0; j < 2; j++) {
                CrateData crate = new CrateData((Brand)j, Rarity.Legendary);
                featuredCrates.Add(new InventoryItem(crate.id, 1));
            }
            
            return featuredCrates;

        }

    }

}


