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
//     using Brand = SneakerWorld.Main.Brand;
//     using Line = SneakerWorld.Main.Line;

//     /// <summary>
//     /// A class to allow building crates to store in firebase, from within unity.
//     /// </summary>
//     [ExecuteInEditMode, CreateAssetMenu(fileName="crate_name_rarity", menuName="Items/Crate")]
//     public class CrateObject : ScriptableObject {

//         // Generated by the object.
//         public string debugId = "";

//         // The details of this crate.
//         public Brand brand;
//         public Edition line;
//         public string description;
//         public Rarity rarity;
//         public int price;
//         public int level;
//         // public Sprite icon;

//         // Runs only while in edit mode when a value is changed.
//         void OnValidate() {
//             // name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
//             debugId = CrateData.GetCrateId(brand, line, rarity);
//         }

//         // Create the actual crate data from the settings of this builder.
//         public CrateData ToData() {
//             CrateData crateData = new CrateData();
//             crateData.brand = brand;
//             crateData.line = line;
//             crateData.description = description;
//             crateData.rarity = rarity;
//             crateData.price = price;
//             // crateData.imagePath = AssetDatabase.GetAssetPath(icon).Replace("Assets/Resources/", "").Replace(".png", "");
//             return crateData;
//         }

//     }

// }

