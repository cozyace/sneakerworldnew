// System.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class Vendor : MonoBehaviour {

        [System.Serializable]
        public class VendorItem {
            public string itemId;
            public int quantity;
            public VendorItem(string itemId, int quantity=0) {
                this.itemId = itemId;
                this.quantity = quantity;
            }
        }

        // Stores the inventory data retrieved from the database.
        [System.Serializable]
        public class VendorData {
            public List<VendorItem> sneakers = new List<VendorItem>(); 
            public List<VendorItem> crates = new List<VendorItem>(); 
        }

        // Triggers an event whenever the inventory changes.
        public UnityEvent<VendorData> onVendorChanged = new UnityEvent<VendorData>();

        // Regularly updating stock within the inventory.
        public VendorData debugStock;

        // Cache a reference to the player.
        public Player player;

        void Start() {
            StartCoroutine(IEQueryVendor());
        }

        [Button]
        public async void Draw() {
            VendorData data = await GetVendorData();
            onVendorChanged.Invoke(data);
        }


        // Initializes the inventory.
        public async Task<bool> Initialize(Player player) {
            // Cache a reference to the player.
            this.player = player;
            try {
                StartCoroutine(IEQueryVendor());
                await default(Task);
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        private IEnumerator IEQueryVendor() {
            while (true) {
                UpdateVendor();
                yield return new WaitForSeconds(3f);
            }
        }

        public async Task UpdateVendor() {
            debugStock = await GetVendorData();
        }

        public async Task<VendorData> GetVendorData() {
            return await FirebaseManager.GetDatabaseValue<VendorData>(FirebasePath.Vendor);
        }

        public async Task RemoveSneakerByID(string sneakerId, int quantity = 1) {
            // How does this work if there is not a sneaker already?
            VendorData currentVendor = await FirebaseManager.GetDatabaseValue<VendorData>(FirebasePath.Vendor);
            Debug.Log($"Managed to find vendor: {currentVendor!=null}");

            // Make sure everything exists.
            if (currentVendor == null) {
                currentVendor = new VendorData();
            }
            if (currentVendor.sneakers == null) {
                currentVendor.sneakers = new List<VendorItem>();
            }

            // Add the item.
            VendorItem item = currentVendor.sneakers.Find(item => item.itemId == sneakerId);
            if (item != null && item.quantity > quantity) {
                
                // Deduct the quantity.
                item.quantity -= quantity;
                await FirebaseManager.SetDatabaseValue<VendorData>(FirebasePath.Vendor, currentVendor);

                // Trigger any listeners.
                onVendorChanged.Invoke(currentVendor);
            }

        }

        public async Task<int> CheckStockForSneakerWithId(string sneakerId) {
            return (await FirebaseManager.GetDatabaseValue<VendorItem>(FirebasePath.VendorSneakersWithId(sneakerId))).quantity;
        }

        public async Task<bool> CheckHasStockForSneakerWithId(string sneakerId, int quantity = 1) {
            int stock = await CheckStockForSneakerWithId(sneakerId);
            return stock >= quantity;
        }

        //
        [Button]
        public async void RefreshVendorInventory() {
            try {

                VendorData vendorData = new VendorData();
                List<VendorItem> crates = new List<VendorItem>();

                // allCrates = FirebaseManager.GetAllKeys(FirebasePath.Crates);
                Dictionary<string, CrateData> allCrates = await FirebaseManager.GetDictionaryAt<CrateData>(FirebasePath.Crates);
                foreach (KeyValuePair<string, CrateData> kv in allCrates) {
                    Debug.Log(kv.Key);
                    Debug.Log(kv.Value.name);
                }
                
                int depth = 0;
                while (crates.Count < 9 && depth < 50) {
                    CrateData randomCrate = allCrates.ElementAt(UnityEngine.Random.Range(0, allCrates.Count)).Value;
                    // For now, keep the logic here simple.
                    Debug.Log(randomCrate.name);

                    crates.Add(new VendorItem(randomCrate.id, UnityEngine.Random.Range(10, 15)));
                    depth += 1;
                }

                vendorData.crates = crates;
                vendorData.sneakers = new List<VendorItem>();

                await FirebaseManager.SetDatabaseValue<VendorData>(FirebasePath.Vendor, vendorData);
                onVendorChanged.Invoke(vendorData);

            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }
            
        }

    }

}
