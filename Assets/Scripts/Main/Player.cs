// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    using FirebaseUser = Firebase.Auth.FirebaseUser;
    using AuthResult = Firebase.Auth.AuthResult;

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class Player : MonoBehaviour {

        // The firebase user.
        public FirebaseUser player => FirebaseManager.user;

        // The friends of this user.
        [SerializeField]
        private FriendList friends = null;

        // The inventory of this user.
        [SerializeField]
        private Inventory inventory = null;

        // Initializes the user.
        public async Task<bool> Initialize(AuthResult auth) {
            try {
                await SetOnlineStatus(true);
                await friends.Initialize(this);
                await inventory.Initialize(this);
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

        void UpdatePlayerProfile() {
            UserProfile profile = new()
            {
                DisplayName = username
            };

            playerStats.username = username;
            await result.User.UpdateUserProfileAsync(profile);
            userId = result.User.UserId;
        }

        public void Deactivate() {
            SetOnlineStatus(false);
        }

        public async Task SetLastLoggedOut() {
            DateTime currentTime = DateTime.Now;
            await FirebaseManager.SetDatabaseValue<DateTime>(playerId, "lastLoggedOut", currentTime);
        }

        public async Task SetOnlineStatus(bool isOnline) {
            await FirebaseManager.SetDatabaseValue<bool>(playerId, "isOnline", isOnline);
        }

    }

}
