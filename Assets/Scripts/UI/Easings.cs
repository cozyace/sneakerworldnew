// // System.
// using System;
// using System.Collections;
// // Unity.
// using UnityEngine;
// using UnityEngine.UI;

// namespace SneakerWorld.UI {

//     using Main;
//     using Utils;

//     public static class Easings : MonoBehaviour {

//         private float easeInOutBack(float x) {
//             const c1 = 1.70158;
//             const c2 = c1 * 1.525;

//             return x < 0.5
//             ? (Math.pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
//             : (Math.pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
//         }

//     }

// }
