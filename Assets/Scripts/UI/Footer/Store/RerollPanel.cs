using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    public class RerollPanel : MonoBehaviour {

        public LerpInt lerpCost;
        public TextMeshProUGUI costText;
        // public Image cashIcon;

        void Awake() {
            Player.instance.store.onRerollEvent.AddListener(DrawCost);
        }

        void FixedUpdate() {
            costText.text = lerpCost.currValue.ToString();
        }

        void DrawCost(int newValue) {
            lerpCost.targetValue = newValue;
        }

    }

}
