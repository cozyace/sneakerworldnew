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

        public enum AccountNo {
            One,
            Two
        }

        public AccountNo accountNo;

        async void Start() {
            Invoke("LoginAsAdmin", 1f);
        }

        // Process the logic for logging the player in.
        [Button("Log In As Admin")]
        public async Task LoginAsAdmin() {
            SneakerWorld.Auth.LoginHandler loginHandler = GameObject.FindFirstObjectByType<SneakerWorld.Auth.LoginHandler>();
            if (accountNo == AccountNo.One) {
                await loginHandler.AttemptLogin("taha.akbarally@gmail.com", "gearless");
            }
            else if (accountNo == AccountNo.Two) {
                await loginHandler.AttemptLogin("git6fr5@hotmail.com", "gearless");
            }
        }

        // Process the logic for logging the player in.
        // [Button("Attempt Remove User As Admin")]
        // public async Task AttemptRemoveUserAsAdmin() {
        //     await FirebaseManager.CurrentUser.DeleteAsync();
        // }

    }

}
