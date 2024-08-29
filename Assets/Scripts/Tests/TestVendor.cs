// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Tests {

    using Main;

    public class TestVendor : MonoBehaviour {

        public UnityEvent<List<CrateData>> onFeaturedCratesUpdated = new UnityEvent<List<CrateData>>();
        public UnityEvent<List<CrateData>> onCratesUpdated = new UnityEvent<List<CrateData>>();

        public UnityEvent<List<SneakerData>> onSneakersUpdated = new UnityEvent<List<SneakerData>>();
        public UnityEvent<List<SneakerData>> onFeaturedSneakersUpdated = new UnityEvent<List<SneakerData>>();

        public List<CrateData> featuredCrates = new List<CrateData>();
        public List<CrateData> crates = new List<CrateData>();
        public List<SneakerData> featuredSneakers = new List<SneakerData>();
        public List<SneakerData> sneakers = new List<SneakerData>();


        void Start() {
            featuredCrates = FeaturedCrates();
            crates = RandomCrates(6, false);
            featuredSneakers = FeaturedSneakers();
            sneakers = RandomSneakers(6);
        }

        public void Refresh() {
            onFeaturedCratesUpdated.Invoke(featuredCrates);
            onCratesUpdated.Invoke(crates);
            onFeaturedSneakersUpdated.Invoke(sneakers);
            onSneakersUpdated.Invoke(sneakers);
        }

        [Button]
        public void RerollCrates(int crateCount) {
            // featuredCrates = FeaturedCrates();
            crates = RandomCrates(crateCount, false);
        }

        [Button]
        public void RerollSneakers(int sneakerCount) {
            // featuredSneakers = FeaturedSneakers();
            sneakers = RandomSneakers(sneakerCount);
        }

        // [Button]
        public List<CrateData> FeaturedCrates() {
            List<CrateData> featuredCrates = new List<CrateData>();

            int brandCount = (int)Brand.Count;
            for (int j = 0; j < 2; j++) {
                featuredCrates.Add(new CrateData((Brand)j, Rarity.Legendary));
            }
            
            onFeaturedCratesUpdated.Invoke(featuredCrates);
            return featuredCrates;

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
            
            onCratesUpdated.Invoke(crateData);
            return crateData;

        }

        // public class CrateComparer : IComparer<CrateData> {
        //     public int Compare(string x, string y) {
        //     if (x == null) {
        //     if (y
        // }

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
                    tmp.Add(crateData[index]);
                    cachedIndices.Add(index);
                    i += 1;
                }

                depth += 1;

            }
            tmp.Sort((x, y) => x.level.CompareTo(y.level));

            onCratesUpdated.Invoke(tmp);
            return tmp;
        }

        // [Button]
        public List<SneakerData> FeaturedSneakers() {
            List<SneakerData> sneakers = new List<SneakerData>();
            
            for (int j = 0; j < 2; j++) {
                Brand brand = (Brand)(UnityEngine.Random.Range(0, (int)Brand.Count));
                SneakerData sneaker = new SneakerData(brand, Edition.Original, Condition.Mint);
                sneakers.Add(sneaker);
            }
            
            onFeaturedSneakersUpdated.Invoke(sneakers);
            return sneakers;

        }

        // [Button]
        public List<SneakerData> RandomSneakers(int count) {
            List<CrateData> crateData = RandomCrates(count, true);
            List<SneakerData> sneakers = new List<SneakerData>();
            
            foreach (CrateData crate in crateData) {
                sneakers.Add(crate.GetRandomSneakerFromCrate());
            }

            sneakers.Sort((x, y) => x.level.CompareTo(y.level));
            
            onSneakersUpdated.Invoke(sneakers);
            return sneakers;

        }

    }

}

