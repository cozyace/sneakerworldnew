// // Unity.
// using UnityEngine;
// // TMP.
// using TMPro;

// namespace SneakerWorld.UI {

//     using Main;

//     [ExecuteInEditMode]
//     public class LoadingScreen : MonoBehaviour {

//         public int circleCount;
//         public Sprite circleSprite;
//         public Transform loadingTransform;

//         private List<Transform> circles = new List<Transform>();

//         void Update() {
//             if (!Application.isPlaying) {
//                 if (circles.Count > 0) {
                    
//                 }

//                 circles = new Transform[loadingTransform.childCount];
                
//                 foreach (Transform child in loadingTransform) {
//                     Destroy(child.gameObject);
//                 }
                
//             }
//         }

//     }

// }
