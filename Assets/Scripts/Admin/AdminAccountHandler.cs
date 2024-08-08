// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Admin {

    using AuthResult = Firebase.Auth.AuthResult;
    
    public class AdminAccountHandler : MonoBehaviour {

        async void Start() {
            Invoke("LoginAsAdmin", 1f);
        }

        // Process the logic for logging the player in.
        [Button("Log In As Admin")]
        public async Task LoginAsAdmin() {
            SneakerWorld.Auth.LoginHandler loginHandler = GameObject.FindFirstObjectByType<SneakerWorld.Auth.LoginHandler>();
            await loginHandler.AttemptLogin("taha.akbarally@gmail.com", "gearless");
        }

        // Process the logic for logging the player in.
        // [Button("Attempt Remove User As Admin")]
        // public async Task AttemptRemoveUserAsAdmin() {
        //     await FirebaseManager.CurrentUser.DeleteAsync();
        // }


    }

}
