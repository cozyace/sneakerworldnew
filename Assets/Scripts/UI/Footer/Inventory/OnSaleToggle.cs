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

    [RequireComponent(typeof(Toggle))]
    public class OnSaleToggle : MonoBehaviour {

        public InventorySlot slot;
        private Toggle toggle;

        void Awake() {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(ToggleOnSale);
        }

        public void ToggleOnSale(bool value) {
            Player.instance.inventory.PutItemOnSale(slot.itemId, value);
        }

    }

}

