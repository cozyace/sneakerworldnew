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


        void Start() {
            FeaturedCrates();
            RandomCrates(6, false);
            FeaturedSneakers();
            RandomSneakers(6);
        }

        [Button]
        public List<CrateData> FeaturedCrates() {
            List<CrateData> featuredCrates = new List<CrateData>();

            int brandCount = (int)Brand.Count;
            for (int j = 0; j < 2; j++) {
                featuredCrates.Add(new CrateData((Brand)j, Rarity.Legendary));
            }
            
            onFeaturedCratesUpdated.Invoke(featuredCrates);
            return featuredCrates;

        }

        [Button]
        public List<CrateData> AllCrates() {
            List<CrateData> crateData = new List<CrateData>();

            int rarityCount = (int)Rarity.Count;
            int brandCount = (int)Brand.Count;
            for (int j = 0; j < brandCount; j++) {
                for (int i = 0; i < rarityCount; i++) {
                    crateData.Add(new CrateData((Brand)j, (Rarity)i));
                }
            }
            
            onCratesUpdated.Invoke(crateData);
            return crateData;

        }

        [Button]
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

            onCratesUpdated.Invoke(tmp);
            return tmp;
        }

        [Button]
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

        [Button]
        public List<SneakerData> RandomSneakers(int count) {
            List<CrateData> crateData = RandomCrates(count, true);
            List<SneakerData> sneakers = new List<SneakerData>();
            
            foreach (CrateData crate in crateData) {
                sneakers.Add(crate.GetRandomSneakerFromCrate());
            }
            
            onSneakersUpdated.Invoke(sneakers);
            return sneakers;

        }

    }

}

