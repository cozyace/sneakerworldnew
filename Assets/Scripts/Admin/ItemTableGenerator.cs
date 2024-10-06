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
    public class ItemTableGenerator : MonoBehaviour {

        [Button]
        private async Task GenerateItems() {
            Debug.Log("hi");
            FirebaseManager.SetDatabaseValue<Dictionary<Rarity, Vector2>>(FirebasePath.ItemDict("CrateRarityRange"), RarityTables.percentileRanges);
            FirebaseManager.SetDatabaseValue<Dictionary<Condition, int>>(FirebasePath.ItemDict("ConditionRarity"), RarityTables.conditionWeights);
            FirebaseManager.SetDatabaseValue<Dictionary<Edition, int>>(FirebasePath.ItemDict("EditionRarity"), RarityTables.editionWeights);
            FirebaseManager.SetDatabaseValue<Dictionary<Brand, float>>(FirebasePath.ItemDict("BrandValues"), BrandTables.brandValue);
            Debug.Log("okay");
       }

    }

}


