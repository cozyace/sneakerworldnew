// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    using Vendor = Main.Vendor;
    using VendorData = Main.Vendor.VendorData;
    using VendorItem = Main.Vendor.VendorItem;

    using CrateData = Main.CrateData;


    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class StoreUI : MonoBehaviour {

        // How many items are listed in the featured section.
        public const int FEATURED_COUNT = 1;

        // A reference to the inventory.
        [SerializeField]
        private Vendor vendor;

        [SerializeField]
        private GameObject itemSlotPrefab;

        [SerializeField]
        private RectTransform mainViewport;

        // Runs once before the first frame.
        void Start() {
            vendor.onVendorChanged.AddListener(Draw);
        }

        public void Draw(VendorData vendorData) {
            if (!Application.isPlaying) { return; }

            foreach (Transform child in mainViewport.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < vendorData.crates.Count; i++) {
                DrawCrateItemSlot(vendorData.crates[i]);
            }

        }
        
        public async void DrawCrateItemSlot(VendorItem item) {
            ItemSlotUI newItemSlot = Instantiate(itemSlotPrefab, mainViewport.transform).GetComponent<ItemSlotUI>();
            CrateData crateData = await CrateData.GetCrateById(item.itemId);
            newItemSlot.Draw(crateData);

        }

    }

}
