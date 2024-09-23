// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public static class PriceCalculator {

        public static Rarity RarityFromCondAndEd(Brand brand, Condition c, Edition ed) {
            float cProb = InvSample<Condition>(c, conditionDict, Condition.Count);
            float eProb = InvSample<Edition>(ed, editionDict, Edition.Count);

            float aveProb = (cProb * eProb);
            float brandAdjustedProb = brandRarityAdjustment[brand] * aveProb;
            (Rarity, float) outputRarity = Sample<Rarity>(brandAdjustedProb, rarityRequirement, Rarity.Count);
            return outputRarity.Item1;
        }


        public static int CalculateCratePrice(Crate crate) {
            // Get the values from the table.
            int basePrice = BrandTables.brandBasePrice[brand];
            float brandValue = BrandTables.brandValue[brand];

            // Calculate the full price.
            float fullPrice = (1f + brandValue * (int)crateRarity) * basePrice;
            return Round(fullPrice);
        }

        public static int GetLevel(Brand brand, Rarity crateRarity) {
            return brandLevelLock[brand] + (int)crateRarity;
        }

        public static int GetSneakerPrice(Sneaker sneaker) {
            int rarityCount = (int)Rarity.Count;

            Rarity[] rarities = new Rarity[rarityCount];
            int[] prices = new int[rarityCount];
            Dictionary<Rarity, int> rp = new Dictionary<Rarity, int>();

            for (int i = 0; i < rarityCount; i++) {
                rarities[i] = (Rarity)i;
                prices[i] = CratePriceCalculator(brand, (Rarity)i);
                rp.Add(rarities[i], prices[i]);
            }

            int count = 0;
            float price = 0f;
            float rarityProb = InvSample<Rarity>(rarity, rarityRequirement, Rarity.Count);
            foreach (var kv in crateRarityRange) {

                // If its possible to get the sneaker from this crate.
                if (rarityProb > kv.Value.x && rarityProb < kv.Value.y) {
                    float amount = (kv.Value.y - rarityProb) / (kv.Value.y - kv.Value.x);
                    price += ((float)rp[kv.Key] / amount);
                    count += 1;
                }

            }

            if (count > 0) {
                return Round((float)price / count);
            }
            return 0;
        }

        // Rounds the prices to a more legible value.
        public static int Round(float price) {
            float roundError = 5f;
            if (price > 100f) {
                roundError = 10f;
            }
            if (price > 500f) {
                roundError = 50f;
            }
            if (price > 1000f) {
                roundError = 100f;
            }
            return (int)(Mathf.Ceil(price / roundError) * roundError);
        }

    }

}
