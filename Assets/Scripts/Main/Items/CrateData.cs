// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// Stores the crate data retrieved from the database.
    /// </summary>
    [System.Serializable]
    public class CrateData {

        // The prefix used to identify crates.
        public const string CRATE_ID_PREFIX = "crate";

        // Wrapper class to store the items within the crate.
        [System.Serializable]
        public class CrateItem {
            public string sneakerId;
            public int quantity;
            public float distributionValue;
        }

        // The id of this crate.
        public string crateId => GetCrateId();
        public string name => GetName();

        // The details of this crate.
        public Brand brand;
        public string description;
        public Rarity rarity;
        public int price;
        // public string imagePath;
        public int level;

        // The required level to purchase this crate.
        public CrateItem[] items;

        
        // Uses the cumulative distribution to get a random sneaker.
        public CrateItem GetRandomItemFromCrate() {
            if (items.Length <= 0) { return null; }

            // Accumulate the distribution of the sneakers.
            float[] cumulativeDistribution = AccumulateDistribution();

            // Get a random sample from somewhere within the cumulative distribution.
            float sampleDistributionAt = UnityEngine.Random.Range(0f, cumulativeDistribution[cumulativeDistribution.Length - 1]);

            // Find what sneaker that sample corresponds to.
            for (int i = 0; i < cumulativeDistribution.Length; i++) {
                if (cumulativeDistribution[i] > sampleDistributionAt) {
                    return items[i];
                }
            }

            return null;

        }

        // Cumulates the distribution of the sneakers.
        public float[] AccumulateDistribution() {
            float[] cumulativeDistribution = new float[items.Length];

            cumulativeDistribution[0] = items[0].distributionValue;
            for (int i = 1; i < items.Length; i++) {
                cumulativeDistribution[i] = items[i].distributionValue + cumulativeDistribution[i-1];
            }

            return cumulativeDistribution;
        }

        public string GetName() {
            return GetName(brand);
        }

        public static string GetName(Brand brand) {
            return $"{brand.ToString()}";
        }

        public string GetCrateId() {
            return GetCrateId(brand, rarity);
        }

        public static string GetCrateId(Brand brand, Rarity rarity) {
            return $"{CRATE_ID_PREFIX}-{GetName(brand).ToLower().Replace(" ", "_")}-{rarity.ToString().ToLower().Replace(" ", "_")}"; 
        }

        // Get the data corresponding to this crate Id.
        public static async Task<CrateData> GetCrateById(string crateId) {
            string path = FirebasePath.ForCrateWithId(crateId);
            return await FirebaseManager.GetDatabaseValue<CrateData>(path);
        }

    }

}

