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
        Bike_Max,
        Bike_Force,
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
            [Edition.RipOff] = 0f,
            [Edition.Second] = 0.5f,
            [Edition.First] = 0.4f,
            [Edition.Original] = 0.1f,
        };

        public static Dictionary<Brand, Vector2Int> brandPriceCalculator = new Dictionary<Brand, Vector2Int>() {
            [Brand.Badidas] = new Vector2Int(5, 2),
            [Brand.Bike] = new Vector2Int(10, 5),
            [Brand.Bike_Max] = new Vector2Int(15, 7),
            [Brand.Bike_Force] = new Vector2Int(20, 10),
        };

        public static Dictionary<Rarity, float> rarityRequirement = new Dictionary<Rarity, float>() {
            [Rarity.Common] = 0.5f,
            [Rarity.Uncommon] = 0.7f,
            [Rarity.Rare] = 0.8f,
            [Rarity.Epic] = 0.9f,
            [Rarity.Legendary] = 0.95f,
        };

        public static Dictionary<Rarity, Vector2> rarityRange = new Dictionary<Rarity, Vector2>() {
            [Rarity.Common] = new Vector2(0f, 0.5f),
            [Rarity.Uncommon] = new Vector2(0.1f, 0.6f),
            [Rarity.Rare] = new Vector2(0.2f, 0.9f),
            [Rarity.Epic] = new Vector2(0.5f, 1f),
            [Rarity.Legendary] = new Vector2(0.8f, 1f),
        };

        public static string GetSneakerFromCrate(Brand brand, Rarity crateRarity) {
            
            Vector2 probDis = rarityRange[crateRarity];

            float conditionProb = UnityEngine.Random.Range(probDis.x, probDis.y);
            float editionProb = UnityEngine.Random.Range(probDis.x, probDis.y);

            (Edition, float) ed = Sample<Edition>(editionProb, editionDict, Edition.Count);
            (Condition, float) cond = Sample<Condition>(conditionProb, conditionDict, Condition.Count);

            float aveProb = (ed.Item2 + cond.Item2) / 2f;
            (Rarity, float) outputRarity = Sample<Rarity>(aveProb, rarityRequirement, Rarity.Count); 

            return $"{ed.Item1}_{cond.Item1}_{outputRarity.Item1}"; 

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

            return (maxEnum, 1f);

        }


        public static void SetRarityPanel(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetRarityCrateIcon(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Store/{rarity.ToString()}Crate");
        }

    }

}
