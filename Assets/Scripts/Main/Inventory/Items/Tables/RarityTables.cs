// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public static class RarityTables {

        public static Dictionary<Rarity, Vector2> percentileRanges = new Dictionary<Rarity, Vector2>() {
            [Rarity.Common] = new Vector2(0f, 0.5f),
            [Rarity.Uncommon] = new Vector2(0.1f, 0.6f),
            [Rarity.Rare] = new Vector2(0.2f, 0.9f),
            [Rarity.Epic] = new Vector2(0.5f, 1f),
            [Rarity.Legendary] = new Vector2(0.8f, 1f),
        };

        public static Dictionary<Condition, int> conditionWeights = new Dictionary<Condition, float>() {
            [Condition.Tattered] = 20,
            [Condition.Worn] = 20,
            [Condition.Decent] = 40,
            [Condition.New] = 10,
            [Condition.Mint] = 2,
        };

        public static Dictionary<Edition, int> editionWeights = new Dictionary<Edition, float>() {
            [Edition.RipOff] = 2,
            [Edition.Second] = 50,
            [Edition.First] = 30,
            [Edition.Original] = 2,
        };

        public static Dictionary<ItemColor, int> colorWeights = new Dictionary<ItemColor, int>() {
            [Recolor.Brown] = 5,
            [Recolor.Blue] = 10,
            [Recolor.Green] = 15,
            [Recolor.Purple] = 20,
            [Recolor.Red] = 25,
            [Recolor.Teal] = 30,
            [Recolor.Yellow] = 50,
        };

        public static Dictionary<Rarity, float> rarityWeights = new Dictionary<Rarity, float>() {
            [Rarity.Common] = 50,
            [Rarity.Uncommon] = 30,
            [Rarity.Rare] = 20,
            [Rarity.Epic] = 10,
            [Rarity.Legendary] = 2,
        };

        // Convert the weights to normalized percentages.
        public static PercentileDictionary<Condition> conditionPercentiles;
        public static PercentileDictionary<Edition> editionPercentiles;
        public static PercentileDictionary<ItemColor> colorPercentiles;
        public static PercentileDictionary<Rarity> rarityPercentiles;

    }

}
