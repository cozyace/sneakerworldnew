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
    public class OpenCrateButton : MonoBehaviour {

        public ItemSlot slot;
        private Button button;
        public bool popupState;

        void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(OpenCrate);
        }

        public void OpenCrate() {
            Player.instance.ui.crateRollerOverlay.GetComponent<OpenCratePopup>().OpenCrate();
        }

    }

}

