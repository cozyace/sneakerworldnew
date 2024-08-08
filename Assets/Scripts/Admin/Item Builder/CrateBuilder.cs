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

//     using CrateData = SneakerWorld.Main.CrateData;
//     using Rarity = SneakerWorld.Main.Rarity;

//     /// <summary>
//     /// A class to allow building crates to store in firebase, from within unity.
//     /// </summary>
//     public class CrateBuilder : MonoBehaviour {

//         // Use this to easily build sneakers.
//         public CrateObject[] crateObjects;

//         // Clean the file names to match their id.
//         [Button]
//         public void CleanFileNames() {
//             for (int i = 0; i < crateObjects.Length; i++) {
//                 var path = AssetDatabase.GetAssetPath(crateObjects[i]);
//                 AssetDatabase.RenameAsset(path, crateObjects[i].ToData().crateId);
//             }
//         }

//         // Add all the crates to the database.
//         [Button]
//         public async Task AddAllCratesToDatabase() {
//             for (int i = 0; i < crateObjects.Length; i++) {
//                 await AddCrateToDatabase(crateObjects[i]);
//             }
//         } 

//         // Add the crate to the database.
//         public async Task AddCrateToDatabase(CrateObject crateObject) {
//             CrateData newData = crateObject.ToData();

//             CrateData currData = await FirebaseManager.GetDatabaseValue<CrateData>(FirebasePath.ForCrateWithId(newData.crateId));
//             if (currData != null) {
//                 // Prompt if sure because it will be overwriting the previous crate with this id.
//             }

//             // If sure.
//             await FirebaseManager.SetDatabaseValue<CrateData>(FirebasePath.ForCrateWithId(newData.crateId), newData);
//         } 

//     }

// }

