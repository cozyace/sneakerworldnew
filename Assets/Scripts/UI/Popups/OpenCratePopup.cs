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

        public InventorySlot mainSlot;
        public InventorySlot rewardSlot;

        public void OpenPopup(InventorySlot slot) {
            mainSlot.Draw(slot.itemId, slot.quantity);
            gameObject.SetActive(true);
        }

        public void OpenCrate() {
            CrateData crateData = CrateData.ParseId(mainSlot.itemId);
            if (crateData != null) {
                crateData.GetRandomSneakerFromCrate();
                mainSlot.Draw(mainSlot.itemId, mainSlot.quantity - 1);
            }
        }


    }

}
