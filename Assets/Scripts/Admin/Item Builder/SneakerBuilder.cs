// // System.
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;
// using UnityEditor;
// // Sirenix.
// using Sirenix.OdinInspector;

// namespace SneakerWorld.Admin {

//     using SneakerData = SneakerWorld.Main.SneakerData;
//     using Rarity = SneakerWorld.Main.Rarity;

//     /// <summary>
//     /// A class to allow building sneakers to store in firebase, from within unity.
//     /// </summary>
//     public class SneakerBuilder : MonoBehaviour {

//         // Use this to easily build sneakers.
//         public SneakerObject[] sneakerObjects;

//         // Add all the sneakers to the database.
//         [Button]
//         public async Task AddAllSneakersToDatabase() {
//             for (int i = 0; i < sneakerObjects.Length; i++) {
//                 await AddSneakerToDatabase(sneakerObjects[i]);
//             }
//         } 

//         // Add the sneaker to the database.
//         public async Task AddSneakerToDatabase(SneakerObject sneakerObject) {
//             SneakerData newData = sneakerObject.ToData();

//             SneakerData currData = await FirebaseManager.GetDatabaseValue<SneakerData>(FirebasePath.ForCrateWithId(newData.sneakerId));
//             if (currData != null) {
//                 // Prompt if sure because it will be overwriting the previous crate with this id.
//             }

//             // If sure.
//             await FirebaseManager.SetDatabaseValue<SneakerData>(FirebasePath.ForSneakerWithId(newData.sneakerId), newData);
//         } 

//     }

// }

