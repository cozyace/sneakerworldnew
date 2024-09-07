// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public GameObject openButton;
        public TextMeshProUGUI openText;

        public GameObject skipButton;
        public GameObject closeButton;

        public PurchaseItemPopup purchaseItemPop;

        public GameObject glow;

        public bool skip;

        void Awake() {
            skipButton.GetComponent<Button>().onClick.AddListener(SkipOpen);
            closeButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
            glow.SetActive(false);
        }

        public void OpenPopup(InventorySlot slot) {
            mainSlot.Draw(slot.itemId, slot.quantity, false, 1f);
            // rewardSlot.Clear();
            openText.text = "Open Crate";
            glow.SetActive(false);

            openButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
            
            rewardSlot.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void ClosePopup() {
            openText.text = "Open Crate";

            openButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(false);
            
            rewardSlot.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public async void OpenCrate() {
            // Check if there is space.
            InventoryData inventory = await Player.instance.inventory.GetInventoryData();
            StoreStateData state = await Player.instance.inventory.state.GetState();
            bool hasSpace = inventory.sneakers.Count < state.inventorySneakerMax;
            if (!hasSpace) {
                Player.instance.purchaser.onPurchaseFailedEvent.Invoke("Not enough inventory space!");
                return;
            }

            CrateData crateData = CrateData.ParseId(mainSlot.itemId);
            openButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(true);

            if (crateData != null) {
                rewardSlot.gameObject.SetActive(true);
                StartCoroutine(IEOpenCrate(crateData));
            }
        }

        private IEnumerator IEOpenCrate(CrateData crateData) {
            int i = 0;

            SneakerData sneakerData = crateData.GetRandomSneakerFromCrate();
            glow.SetActive(false);


            while (i < 30 && !skip) {

                sneakerData.quantity = (int)Mathf.Ceil(UnityEngine.Random.Range(100, 50) / (sneakerData.level + 1));
                rewardSlot.Draw(sneakerData.id, sneakerData.quantity, false, 1f);
                i += 1;
                yield return new WaitForSeconds(0.1f);
                sneakerData = crateData.GetRandomSneakerFromCrate();

            }

            sneakerData.quantity = (int)Mathf.Ceil(UnityEngine.Random.Range(100, 50) / (sneakerData.level + 1));
            rewardSlot.Draw(sneakerData.id, sneakerData.quantity, false, 1f);
            
            skip = false;

            UpdateInventory(crateData, sneakerData);
            
            mainSlot.Draw(mainSlot.itemId, mainSlot.quantity - 1);
            openButton.gameObject.SetActive(mainSlot.quantity > 0);
            openText.text = "Open Another";
            skipButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);

            glow.SetActive(true);

            // purchaseItemPop.gameObject.SetActive(true);
            // purchaseItemPop.DrawSuccessfulPurchase("", sneakerData.id, sneakerData.quantity);

            // if (mainSlot.quantity <= 0) {
            //     gameObject.SetActive(false);
            // }
        }

        private async Task UpdateInventory(CrateData crateData, SneakerData sneakerData) {
            await Player.instance.inventory.RemoveItemByID(crateData.id, 1);
            await Player.instance.inventory.AddItemByID(sneakerData.id, sneakerData.quantity);
        }

        public void SkipOpen() {
            skip = true;
        }


    }

}
