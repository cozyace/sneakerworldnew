// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// An overarching system that is controlled directly from the player.
    /// </summary>
    public abstract class PlayerSystem : MonoBehaviour {

        // Cache a reference to the player.
        [HideInInspector]
        public Player player;

        // Initializes this system.
        public async Task<bool> Initialize(Player player) {
            this.player = player;
            try {
                await TryInitialize();
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        protected abstract Task TryInitialize();

    }

}
