// // System.
// using System;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;
// using UnityEngine.Events;

// namespace SneakerWorld.Admin {

//     using UI;
//     using CrateData = SneakerWorld.Main.CrateData;

//     /// <summary>
//     /// Allows visualisation of the crates being built.
//     /// </summary>
//     [ExecuteInEditMode]
//     public class CrateObjectDebugger : MonoBehaviour {

//         // The builder to debug.
//         public CrateObject crateObject;

//         // To test what this looks like.
//         public FeaturedItemSlotUI featuredItemSlotUI = null;

//         // Runs once per frame.
//         void Update() {
//             if (!Application.isPlaying) {
//                 UpdateWhileInEditMode();
//             }
//         }

//         // Runs once per frame only while in edit mode.
//         void UpdateWhileInEditMode() {
//             if (crateObject == null) { return; }
            
//             if (featuredItemSlotUI != null) {
//                 featuredItemSlotUI.Draw(crateObject.ToData());
//             }
//         }

//     }

// }
