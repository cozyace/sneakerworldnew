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
    public class ItemGenerator : MonoBehaviour {

        [Button]
        private async Task GenerateItems() {
            Debug.Log("hi");
            FirebaseManager.SetDatabaseValue<Dictionary<Condition, float>>(FirebasePath.ItemDict("ConditionRarity"), RarityUtils.conditionDict);
            FirebaseManager.SetDatabaseValue<Dictionary<Edition, float>>(FirebasePath.ItemDict("EditionRarity"), RarityUtils.editionDict);
            FirebaseManager.SetDatabaseValue<Dictionary<Brand, float>>(FirebasePath.ItemDict("BrandRarity"), RarityUtils.brandRarityAdjustment);
            FirebaseManager.SetDatabaseValue<Dictionary<Rarity, Vector2>>(FirebasePath.ItemDict("CrateRarityRange"), RarityUtils.crateRarityRange);
            // FirebaseManager.SetDatabaseValue<Dictionary<Edition, float>>(FirebasePath.ItemDict("EditionRarity"), RarityUtils.editionDict);
            Debug.Log("okay");
       }


        // private string[] BrandToStringArray() {
        //     int count = Brand.Count;
        //     string[] output = new string[count];
        //     for (int i = 0; i < count; i++) {
        //         output[i] = (Brand)i;
        //     }
        //     return output;
        // }

        // private string[] ConditionToStringArray() {
        //     int count = Condition.Count;
        //     string[] output = new string[count];
        //     for (int i = 0; i < count; i++) {
        //         output[i] = (Condition)i;
        //     }
        //     return output;
        // }

        // private string[] EditionToStringArray() {
        //     int count = Edition.Count;
        //     string[] output = new string[count];
        //     for (int i = 0; i < count; i++) {
        //         output[i] = (Edition)i;
        //     }
        //     return output;
        // }

        // private string[] RarityToStringArray() {
        //     int count = Rarity.Count;
        //     string[] output = new string[count];
        //     for (int i = 0; i < count; i++) {
        //         output[i] = (Rarity)i;
        //     }
        //     return output;
        // }

    }

}


