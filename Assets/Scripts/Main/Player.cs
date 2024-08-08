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

        // The components this script manages.
        // public Store store;
        public Inventory inventory;
        public Wallet wallet;
        // public Status status;
        // private FriendList friends = null;

        void Start() {
            GameObject.FindFirstObjectByType<SneakerWorld.Auth.LoginHandler>().onLoginSuccessEvent.AddListener(Init);
        }

        async void Init(string message) {
            await Initialize();
        }

        // Initializes the user.
        public async Task<bool> Initialize() {
            try {
                // await friends.Initialize(this);
                // await store.Initialize(this);
                // await status.Initialize(this);
                await inventory.Initialize(this);
                await wallet.Initialize(this);
                return true;
            }
            catch (Exception exception) {
                Debug.Log(exception.Message);
            }
            return false;
        }

    }

}
