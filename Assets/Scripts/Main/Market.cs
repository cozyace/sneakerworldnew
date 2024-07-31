// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class Market : MonoBehaviour {

        // The cached reference to the player.
        private Player player;

        // Initializes the inventory.
        public async Task<bool> Initialize(Player player) {
            // Cache a reference to the player.
            this.player = player;
            try {
                await GetFeaturedEvent()
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        public async Task GetFeaturedEvent() {
            await FirebaseManager.GetDatabaseValue<MarketEvent>("market", "featured");
        }
        
    }

}
