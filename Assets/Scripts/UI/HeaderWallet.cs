// // System.
// using System;
// using System.Collections;
// // Unity.
// using UnityEngine;
// using UnityEngine.UI;
// // TMP.
// using TMPro;

// namespace SneakerWorld.UI {

//     using Main;
//     using Utils;

//     /// <summary>
//     /// Listen to the wallet and draws it.
//     /// </summary>
//     public class HeaderWalletPanel : MonoBehaviour {

//         // The values.
//         public LerpInt cashValue;
//         public LerpInt gemsValue;

//         // The ui components.
//         public TextMeshProUGUI cashText;
//         public TextMeshProUGUI gemsText;

//         // animation.
//         public Image icon;
//         public SizeAnimation sizeAnimation;

//         // Runs once before instantiation.
//         public void Awake() {
//             // Listen to the wallet events.
//             Player.instance.wallet.onInitEvent.AddListener(DrawBalance);
//             Player.instance.wallet.onCashTransactionEvent.AddListener(DrawCashTransaction);
//             Player.instance.wallet.onGemTransactionEvent.AddListener(DrawGemTransaction);
//         }

//         // Runs once every fixed interval.
//         void FixedUpdate() {
//             cashText.text = cashValue.currValue.ToString();
//             gemsText.text = gemsValue.currValue.ToString();
//         }

//         // Draw the cash balance.
//         public void DrawBalance(int cash, int gems) {
//             cashValue.currValue = cash;
//             gemsValue.currValue = gems;
//         }

//         // Draw the transaction.
//         public void DrawCashTransaction(int prevValue, int deltaValue) {
//             cashValue.targetValue = prevValue + deltaValue;
//         }

//         // Draw the gems transaction.
//         public void DrawGemTransaction(int prevValue, int deltaValue) {
//             gemsValue.targetValue = prevValue + deltaValue;
//         }

//     }

// }
