// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SneakerWorld.UI {

    using Main;

    [RequireComponent(typeof(Button))]
    public class AdjustMarkupButton : MonoBehaviour {

        public InventorySlot slot;
        private Button button;
        public float amount = 0.5f;

        void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(AdjustMarkup);
        }

        public void AdjustMarkup() {
            Player.instance.inventory.AdjustMarkup(slot.itemId, amount);
        }

    }

}

