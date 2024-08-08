// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public static class EnumExtensions {
        public static int ToInt(this Enum enumValue) {
            return Convert.ToInt32(enumValue);
        }
    }

    public enum Brand {
        Badidas,
        Bike,
        // Bike_Max,
        // Bike_Force,
        Count
    }

    public enum Condition {
        Tattered,
        Worn,
        Decent,
        New,
        Mint,
        Count
    }

    public enum Edition {
        RipOff,
        Second,
        First,
        Original,
        Count,
    }

    public enum Rarity {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Count,
    }

    public static class RarityUtils {


        public static Dictionary<Condition, float> conditionDict = new Dictionary<Condition, float>() {
            [Condition.Tattered] = 0.2f,
            [Condition.Worn] = 0.2f,
            [Condition.Decent] = 0.4f,
            [Condition.New] = 0.15f,
            [Condition.Mint] = 0.05f,
        };

        public static Dictionary<Edition, float> editionDict = new Dictionary<Edition, float>() {
            [Edition.RipOff] = 0.05f,
            [Edition.Second] = 0.5f,
            [Edition.First] = 0.4f,
            [Edition.Original] = 0.1f,
        };

        public static Dictionary<Brand, float> brandRarityAdjustment = new Dictionary<Brand, float>() {
            [Brand.Badidas] = 0.7f,
            [Brand.Bike] = 0.8f,
            // [Brand.Bike_Max] = 0.85f,
            // [Brand.Bike_Force] = 0.95f,
        };

        public static Dictionary<Rarity, float> rarityRequirement = new Dictionary<Rarity, float>() {
            [Rarity.Common] = 0.2f,
            [Rarity.Uncommon] = 0.4f,
            [Rarity.Rare] = 0.2f,
            [Rarity.Epic] = 0.15f,
            [Rarity.Legendary] = 0.05f,
        };

        public static Dictionary<Rarity, Vector2> crateRarityRange = new Dictionary<Rarity, Vector2>() {
            [Rarity.Common] = new Vector2(0f, 0.5f),
            [Rarity.Uncommon] = new Vector2(0.1f, 0.6f),
            [Rarity.Rare] = new Vector2(0.2f, 0.9f),
            [Rarity.Epic] = new Vector2(0.5f, 1f),
            [Rarity.Legendary] = new Vector2(0.8f, 1f),
        };

        public static string GetSneakerFromCrate(Brand brand, Rarity crateRarity) {

            Vector2 probDis = crateRarityRange[crateRarity];

            float conditionProb = UnityEngine.Random.Range(probDis.x, probDis.y);
            float editionProb = UnityEngine.Random.Range(probDis.x, probDis.y);

            (Edition, float) ed = Sample<Edition>(editionProb, editionDict, Edition.Count);
            (Condition, float) cond = Sample<Condition>(conditionProb, conditionDict, Condition.Count);

            // float aveProb = (ed.Item2 + cond.Item2) / 2f;
            float aveProb = (conditionProb * editionProb);
            float brandAdjustedProb = brandRarityAdjustment[brand] * aveProb;
            (Rarity, float) outputRarity = Sample<Rarity>(brandAdjustedProb, rarityRequirement, Rarity.Count);

            return $"{ed.Item1}_{cond.Item1}_{outputRarity.Item1}";

        }

        public static (Edition, Condition, Rarity) GetSneakerParamsFromCrate(Brand brand, Rarity crateRarity) {

            Vector2 probDis = crateRarityRange[crateRarity];

            float conditionProb = UnityEngine.Random.Range(probDis.x, probDis.y);
            float editionProb = UnityEngine.Random.Range(probDis.x, probDis.y);

            (Edition, float) ed = Sample<Edition>(editionProb, editionDict, Edition.Count);
            (Condition, float) cond = Sample<Condition>(conditionProb, conditionDict, Condition.Count);

            // float aveProb = (conditionProb * editionProb);
            float aveProb = (ed.Item2 * cond.Item2);
            float brandAdjustedProb = brandRarityAdjustment[brand] * aveProb;
            (Rarity, float) outputRarity = Sample<Rarity>(brandAdjustedProb, rarityRequirement, Rarity.Count);

            return (ed.Item1, cond.Item1, outputRarity.Item1);

        }

        public static Rarity RarityFromCondAndEd(Brand brand, Condition c, Edition ed) {
            float cProb = InvSample<Condition>(c, conditionDict, Condition.Count);
            float eProb = InvSample<Edition>(ed, editionDict, Edition.Count);

            float aveProb = (cProb * eProb);
            float brandAdjustedProb = brandRarityAdjustment[brand] * aveProb;
            (Rarity, float) outputRarity = Sample<Rarity>(brandAdjustedProb, rarityRequirement, Rarity.Count);
            return outputRarity.Item1;
        }

        public static int CratePriceCalculator(Brand brand, Rarity crateRarity) {
            float skew = 0.8f;

            int BASE_PRICE = 10;
            float PER_RARITY = 2f;

            float brandValue = brandRarityAdjustment[brand];

            float basePrice = BASE_PRICE * skew * Mathf.Pow(2f, 1f + PER_RARITY * brandValue);
            float additionalPrice = Mathf.Pow(1f + brandValue, (PER_RARITY * (int)crateRarity));

            float fullPrice = basePrice + additionalPrice;

            float roundError = 5f;
            if (fullPrice > 100f) {
                roundError = 10f;
            }
            if (fullPrice > 500f) {
                roundError = 50f;
            }
            if (fullPrice > 1000f) {
                roundError = 100f;
            }

            return (int)(Mathf.Ceil(fullPrice / roundError) * roundError);
        }


        public static (T, float) Sample<T>(float prob, Dictionary<T, float> dict, T maxEnum)
            where T : Enum {

            T[] enums = new T[maxEnum.ToInt()];
            float[] dis = new float[maxEnum.ToInt()];
            float[] cumDis = new float[maxEnum.ToInt()];

            foreach (var kv in dict) {
                enums[kv.Key.ToInt()] = kv.Key;
                dis[kv.Key.ToInt()] = kv.Value;
            }

            cumDis[0] = dis[0];
            for (int i = 1; i < dis.Length; i++) {
                cumDis[i] = dis[i] + cumDis[i-1];
            }

            for (int i = 0; i < cumDis.Length; i++) {
                if (prob < cumDis[i]) {
                    return (enums[i], cumDis[i]);
                }
            }

            return (enums[enums.Length-1], 1f);

        }

        public static float InvSample<T>(T t, Dictionary<T, float> dict, T maxEnum)
            where T : Enum {

            int index = 0;
            float[] dis = new float[maxEnum.ToInt()];
            float[] cumDis = new float[maxEnum.ToInt()];

            foreach (var kv in dict) {
                if (kv.Key.Equals(t)) {
                    index = kv.Key.ToInt();
                }
                dis[kv.Key.ToInt()] = kv.Value;
            }

            cumDis[0] = dis[0];
            for (int i = 1; i < dis.Length; i++) {
                cumDis[i] = dis[i] + cumDis[i-1];
            }

            return cumDis[index];

        }


    }

}
