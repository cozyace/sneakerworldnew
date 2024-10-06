// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    /// <summary>
    /// Listen to the wallet and draws it.
    /// </summary>
    public class UserInterface : MonoBehaviour {

        // public CrateHandlerOverlay crateHandlerOverlay;
        // public PurchaseHandlerOverlay purchaseHandlerOverlay;
        // public ConnectionHandlerOverlay connectionHandlerOverlay;
        // // public GameObject purchaseHandlerOverlay;

        // void Start() {
        //     connectionHandlerOverlay.EnableOverlay(true);
        //     crateHandlerOverlay.EnableOverlay(false);
        //     purchaseHandlerOverlay.EnableOverlay(false);
        // }

        public GameObject storePanel;
        public GameObject inventoryPanel;

        public GameObject crateRollerOverlay;
        public GameObject purchaseHandlerOverlay;
        public GameObject connectionHandlerOverlay;
        // public GameObject purchaseHandlerOverlay;

        public GameObject messagePopup;
        public GameObject cashPopup;

        void Start() {
            connectionHandlerOverlay.SetActive(true);
            crateRollerOverlay.SetActive(false);
            purchaseHandlerOverlay.SetActive(true);
            storePanel.SetActive(true);
            inventoryPanel.SetActive(true);

            cashPopup.SetActive(true);
            messagePopup.SetActive(true);
        }

    }

}
