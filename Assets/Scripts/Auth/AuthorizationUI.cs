using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Auth {

    /// <summary>
    /// Responds to events from the login handler.
    /// </summary>
    public class AuthorizationUI : MonoBehaviour {

        [Space(2), Header("Login")]

        // The login handler that this UI tracks.
        public LoginHandler loginHandler;

        // The input field for the username.
        [SeriliazeField]
        private TMP_InputField loginEmail;

        // The input field for the password.
        [SeriliazeField]
        private TMP_InputField loginPassword;

        [Space(2), Header("Signup")]

        // The sign up handler that this UI tracks.
        public SignupHandler signupHandler;

        // The input field for the username.
        [SeriliazeField]
        private TMP_InputField signupEmail;

        // The input field for the password.
        [SeriliazeField]
        private TMP_InputField signupUsername;

        // The input field for the password.
        [SeriliazeField]
        private TMP_InputField signupPassword;

        // The input field for the password.
        [SeriliazeField]
        private TMP_InputField signupConfirmPassword;

        // The screen overlay for loading.
        public LoadingScreen loadingScreen;
        
        // The textmesh to send messages to the player.
        public TextMeshProUGUI logText;


        // Runs once before the first frame.
        private void Start() {

            // Hook up the login events.
            loginHandler.onLoginStartEvent += OnAuthorizationProcessStart;
            loginHandler.onLoginSuccessEvent += OnAuthorizationProcessSucceed;
            loginHandler.onLoginFailedEvent += OnAuthorizationProcessFailed;

            // Hook up the signup events.
            signupHandler.onSignupStartEvent += OnAuthorizationProcessStart;
            signupHandler.onSignupSuccessEvent += OnAuthorizationProcessSucceed;
            signupHandler.onSignupFailedEvent += OnAuthorizationProcessFailed;

        }

        // The UI Process whenever an authorization process starts. 
        public void OnAuthorizationProcessStart() {
            loadingScreen.Enable();
        }

        // The UI Process whenever an authorization process succeeds. 
        public void OnAuthorizationProcessSucceed(string message) {
            SendMessage(message);

            // Clear all the fields of text.
            loginEmail.text = "";
            loginPassword.text = "";
            signupUsername.text = "";
            signupEmail.text = "";
            signupPassword.text = "";
            signupConfirmPassword.text = "";

            loadingScreen.Disable();
        }

        // The UI Process whenever an authorization process fails. 
        public void OnAuthorizationProcessFailed(string message = "Unknown") {
            SendMessage(message);
            loadingScreen.Disable();
        }

        // The UI Process to send a message to the user.
        private void SendMessage(string message) {
            logText.text = message;
        }

        public void ResetDataButton() {
            PlayerPrefs.DeleteAll();
            Debug.Log("All local data has been deleted!");
        }
        
    }

}
