// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    public class OpenCratePopup : MonoBehaviour {

        public CrateInventorySlot slot;

        public void Open(CrateInventorySlot slot) {
            Open(slot.itemId, slot.quantity);
        }

        public void Open(string crateId, int maxQuantity) {
            Debug.Log(crateId);
            slot.Draw(crateId, maxQuantity);
            gameObject.SetActive(true);
        }

    }

}
