// using System;
// using System.Collections;
// using System.Collections.Generic;
// // Unity.
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Events;
// // TMP.
// using TMPro;

// namespace SneakerWorld.UI {

//     using Main;
//     using Utils;

//     public class RerollPanel : MonoBehaviour {

//         public LerpInt lerpSneakerCost;
//         public TextMeshProUGUI sneakerCostText;

//         public LerpInt lerpCratesCost;
//         public TextMeshProUGUI cratesCostText;
//         // public Image cashIcon;

//         void Awake() {
//             Player.instance.store.roller.onRerollInit.AddListener(SetCost);
//             Player.instance.store.roller.onRerollSneakersEvent.AddListener(DrawSneakerRerollCost);
//             Player.instance.store.roller.onRerollCratesEvent.AddListener(DrawCratesRerollCost);
//         }

//         void FixedUpdate() {
//             sneakerCostText.text = lerpSneakerCost.currValue.ToString();
//             cratesCostText.text = lerpCratesCost.currValue.ToString();
//         }

//         void DrawSneakerRerollCost(int newValue) {
//             lerpSneakerCost.targetValue = newValue;
//         }

//         public void SetCost(int sneakerRerollCost, int cratesRerollCost) {
//             SetSneakerRerollCost(sneakerRerollCost);
//             SetCratesRerollCost(cratesRerollCost);
//         }

//         void SetSneakerRerollCost(int newValue) {
//             lerpSneakerCost.currValue = newValue;
//         }

//         void DrawCratesRerollCost(int newValue) {
//             lerpCratesCost.targetValue = newValue;
//         }

//         void SetCratesRerollCost(int newValue) {
//             lerpCratesCost.currValue = newValue;
//         }

//     }

// }
