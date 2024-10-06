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
    public class UpgradeStoreButton : MonoBehaviour {

        private Button button;

        void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(UpgradeStore);
        }

        public void UpgradeStore() {
            Player.instance.store.state.UpgradeStore();
        }

    }

}

