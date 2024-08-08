// // System.
// using System;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;
// using UnityEditor;
// using UnityEngine.Events;

// namespace SneakerWorld.Admin {

//     using UI;
//     using SneakerData = SneakerWorld.Main.SneakerData;

//     /// <summary>
//     /// Allows visualisation of the crates being built.
//     /// </summary>
//     [ExecuteInEditMode]
//     public class SneakerObjectDebugger : MonoBehaviour {

//         // The builder to debug.
//         public SneakerObject sneakerObject;

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
//             if (sneakerObject == null) { return; }

//             // if (featuredItemSlotUI != null) {
//             //     featuredItemSlotUI.Draw(sneakerBuilder.debugData);
//             // }

//         }

//     }

// }
