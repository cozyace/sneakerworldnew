// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    /// <summary>
    /// Listen to the wallet and draws it.
    /// </summary>
    public class WalletValueDrawer : MonoBehaviour {

        // The values.
        public LerpInt lerpInt;

        // The ui components.
        public TextMeshProUGUI textMesh;

        // animation.
        public Image icon;
        public SizeAnimation anim;

        // Runs once every fixed interval.
        void FixedUpdate() {
            textMesh.text = lerpInt.currValue.ToString();
        }

        // Draw the cash balance.
        public void DrawBalance(int value) {
            lerpInt.currValue = value;
        }

        // Draw the transaction.
        public void DrawTransaction(int prevValue, int deltaValue) {
            lerpInt.targetValue = prevValue + deltaValue;
            anim.Play();
        }

    }

}
