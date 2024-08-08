// System.
using System;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.Auth {

    /// <summary>
    /// Responds to events from the login handler.
    /// </summary>
    public class AuthorizationUI : MonoBehaviour {

        [Space(2), Header("Login")]

        // The login handler that this UI tracks.
        public LoginHandler loginHandler;

        // The input field for the username.
        [SerializeField]
        private TMP_InputField loginEmail;

        // The input field for the password.
        [SerializeField]
        private TMP_InputField loginPassword;

        [Space(2), Header("Signup")]

        // The sign up handler that this UI tracks.
        public SignupHandler signupHandler;

        // The input field for the username.
        [SerializeField]
        private TMP_InputField signupEmail;

        // The SerializeField field for the password.
        [SerializeField]
        private TMP_InputField signupUsername;

        // The input field for the password.
        [SerializeField]
        private TMP_InputField signupPassword;

        // The input field for the password.
        [SerializeField]
        private TMP_InputField signupConfirmPassword;

        // The screen overlay for loading.
        // public Overlay loadingOverlay;
        
        // The textmesh to send messages to the player.
        public TextMeshProUGUI logText;


        // Runs once before the first frame.
        private void Start() {

            // Hook up the login events.
            loginHandler.onLoginStartEvent.AddListener(OnAuthorizationProcessStart);
            loginHandler.onLoginSuccessEvent.AddListener(OnAuthorizationProcessSucceed);
            loginHandler.onLoginFailedEvent.AddListener(OnAuthorizationProcessFailed);

            // Hook up the signup events.
            signupHandler.onSignupStartEvent.AddListener(OnAuthorizationProcessStart);
            signupHandler.onSignupSuccessEvent.AddListener(OnAuthorizationProcessSucceed);
            signupHandler.onSignupFailedEvent.AddListener(OnAuthorizationProcessFailed);

        }

        // The UI Process whenever an authorization process starts. 
        public void OnAuthorizationProcessStart() {
            // loadingOverlay.Enable();
        }

        // The UI Process whenever an authorization process succeeds. 
        public void OnAuthorizationProcessSucceed(string message) {
            CreatePopup(message);

            // Clear all the fields of text.
            loginEmail.text = "";
            loginPassword.text = "";
            signupUsername.text = "";
            signupEmail.text = "";
            signupPassword.text = "";
            signupConfirmPassword.text = "";

            // loadingOverlay.Disable();
        }

        // The UI Process whenever an authorization process fails. 
        public void OnAuthorizationProcessFailed(string message = "Unknown") {
            CreatePopup(message);
            // loadingOverlay.Disable();
        }

        // The UI Process to send a message to the user.
        private void CreatePopup(string message) {
            logText.text = message;
        }

        public void ResetDataButton() {
            PlayerPrefs.DeleteAll();
            Debug.Log("All local data has been deleted!");
        }
        
    }

}
