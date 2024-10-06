// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.UI {

    using Main;

    public class PurchaseItemButton : MonoBehaviour {

        private Button button;
        public ItemSlot slot;
        public bool featured;

        void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(AttemptPurchase);
        }

        public void AttemptPurchase() {
            Player.instance.purchaser.StartPurchase(slot.itemId, slot.quantity, featured);
            Debug.Log($"Attempting purchase of : {slot.itemId}");
        }
        

    }

}

