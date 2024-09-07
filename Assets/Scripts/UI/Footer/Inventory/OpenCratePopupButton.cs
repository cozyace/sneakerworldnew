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
    public class OpenCratePopupButton : MonoBehaviour {

        public InventorySlot slot;
        private Button button;
        public bool popupState;

        void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(SetCratePopup);
        }

        public void SetCratePopup() {
            Player.instance.ui.crateRollerOverlay.GetComponent<OpenCratePopup>().OpenPopup(slot);
        }

    }

}

