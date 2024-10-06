// // System.
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;
// using UnityEngine.Events;
// // Sirenix.
// using Sirenix.OdinInspector;

// namespace SneakerWorld.Tests {

//     using Main;

//     public class TestCrate : MonoBehaviour {

//         public CrateData crateData;

//         [Button]
//         void CalculateDistribution(int count) {

//             Debug.Log($"Price of crate is : {RarityUtils.CratePriceCalculator(crateData.brand, crateData.rarity)}");

//             Dictionary<Edition, int> edCounter = new Dictionary<Edition, int>();
//             Dictionary<Condition, int> condCounter = new Dictionary<Condition, int>();
//             Dictionary<Rarity, int> rarityCounter = new Dictionary<Rarity, int>();

//             for (int i = 0; i < count; i++) {
//                 (Edition, Condition, Rarity) sneaker = RarityUtils.GetSneakerParamsFromCrate(crateData.brand, crateData.rarity);
//                 Add<Edition>(edCounter, sneaker.Item1);
//                 Add<Condition>(condCounter, sneaker.Item2);
//                 Add<Rarity>(rarityCounter, sneaker.Item3);
//             }

//             PrintDictCounter<Edition>(edCounter, "Edition Counter: ", count);
//             PrintDictCounter<Condition>(condCounter, "Condition Counter: ", count);
//             PrintDictCounter<Rarity>(rarityCounter, "Rarity Counter: ", count);

//         }

//         void Add<T>(Dictionary<T, int> dict, T t) {
//             if (!dict.ContainsKey(t)) {
//                 dict.Add(t, 0);
//             }
//             dict[t] += 1;
//         }

//         void PrintDictCounter<T>(Dictionary<T, int> dict, string initMsg, int count)
//             where T : Enum {
            
//             foreach (var kv in dict) {
//                 initMsg += $"|| {kv.Key.ToString()} : {((float)kv.Value / (float)count).ToString()}";
//             }

//             Debug.Log(initMsg);

//         }

//     }

// }

