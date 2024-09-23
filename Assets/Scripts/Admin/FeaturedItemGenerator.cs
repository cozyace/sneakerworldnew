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
            Inventory inventory = new Inventory();
            inventory.Add(FeaturedSneakers());
            inventory.Add(FeaturedCrates());

            FirebaseManager.SetDatabaseValue<FeaturedStoreData>(FirebasePath.FeaturedItemWithDate(FeaturedItem.GetTimeId(day, month, year)), inventory);
        }

        public List<Item> FeaturedSneakers() {
            List<Item> sneakers = new List<Item>();
            
            for (int j = 0; j < 2; j++) {
                // Make the featured sneaker.
                Item sneaker = new Item(ItemType.Sneaker, 1);
                sneaker.AddId<Brand>(ItemMaker.RandomEnum<Brand>(Brand.Count));
                sneaker.AddId<Edition>(Edition.Original);
                sneaker.AddId<Condition>(Condition.Mint);
                // Add it to the pile.
                sneakers.Add(sneaker);
            }
            
            return sneakers;

        }

        public List<Item> FeaturedCrates() {
            List<Item> crates = new List<Item>();

            int brandCount = (int)Brand.Count;
            for (int j = 0; j < 2; j++) {
                // Make the featured crate.
                Item crate = new Item(ItemType.Crate, 1);
                crate.AddId<Brand>(ItemMaker.RandomEnum<Brand>(Brand.Count));
                crate.AddId<Rarity>(Rarity.Legendary);
                // Add it to the pile.
                crates.Add(crate);
            }
            
            return crates;

        }

    }

}


