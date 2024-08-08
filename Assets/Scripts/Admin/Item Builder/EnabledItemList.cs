// // System.
// using System;
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// // Firebase.
// using Firebase;
// using Firebase.Auth;
// using Firebase.Database;
// // Sirenix.
// using Sirenix.OdinInspector;

// namespace SneakerWorld.Admin {

//     /// <summary>
//     /// Enables and disables items from the game.
//     /// </summary>
//     public class EnabledItemList : MonoBehaviour {

//         // Name, rarity, enabled/disabled?

//         public List<string> allCrates = new List<string>();

//         [Button]
//         public async void CollateAllCrates() {
//             allCrates = new List<string>();

//             DataSnapshot currentValue = await FirebaseManager.Instance.databaseRoot.Child(FirebasePath.Crates).GetValueAsync();
//             foreach (DataSnapshot child in currentValue.Children) { 
//                 allCrates.Add(child.Key);
//             }

//         }



//     }
    
// }
