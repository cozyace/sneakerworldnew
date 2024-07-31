// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class Inventory : MonoBehaviour {

        // The cached reference to the player.
        private Player player;

        // Initializes the inventory.
        public async Task<bool> Initialize(Player player) {
            // Cache a reference to the player.
            this.player = player;
            try {
                await GetSneakers()
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        public async Task GetSneakers() {
            await FirebaseManager.GetDatabaseValue<SneakerData>(player.id, "inventory/sneakers");
        }
        
        // public async Task GetListings() {
        //     await FirebaseManager.GetDatabaseValue<InventoryData>(player.id, "inventory");
        // }

    }

}
