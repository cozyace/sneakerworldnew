// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Auth {

    using AuthResult = Firebase.Auth.AuthResult;
    using FirebaseException = Firebase.FirebaseException;

    /// <summary>
    /// Handles all the login logic.
    /// </summary>
    public class LoginHandler : MonoBehaviour {

        // A message to throw if the the password or email is not a valid string.
        public const string INVALID_INPUT_MESSAGE = "Invalid email or password.";

        // A message to throw if the user's email is not verified.
        public const string UNVERIFIED_EMAIL_MESSAGE = "User email not verified! Check your inbox.";

        // A message to send when the sign up is successful.
        public const string SUCCESSFUL_LOGIN_MESSAGE = "Login successful!"

        // An event to trigger when the log in process has begun.
        public UnityEvent onLoginStartEvent;

        // An event to trigger when the log in process succeeds.
        public UnityEvent<string> onLoginSuccessEvent;

        // An event to trigger when the log in process fails.
        public UnityEvent<string> onLoginFailedEvent;


        // Process the logic for logging the player in.
        [Button("Attempt Login")]
        public async Task LoginAsync(string email, string password) {

            // Start the login event.
            onLoginStartEvent.Invoke();
            
            try {

                // Check whether the login input is valid.
                if (!ValidateLoginInput(email, password)) {
                    throw new Exception(INVALID_INPUT_MESSAGE);
                }

                AuthResult result = await FirebaseManager.CheckForUserInDatabase(email, password);
                if (!result.User.IsEmailVerified) {
                    throw new Exception(UNVERIFIED_EMAIL_MESSAGE);
                }

                // SneakerWorldUser(result.User, result.User.UserId)
                FirebaseManager.SetCurrentUser(result.User);

                // Trigger a successful login event.
                onLoginSuccessEvent.Invoke(SUCCESSFUL_LOGIN_MESSAGE);

                // await RunCoroutine(LoadDataEnum(() => StartCoroutine(LoadSceneAsync(1))));
                
            }
            catch (FirebaseException exception) {
                // Trigger a failed login event.
                Debug.LogError($"Login failed: {message}");
                onLoginFailedEvent.Invoke(exception.Message);
            }

        }
    
        private bool ValidateLoginInput(string email, string password) {
            return !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(password);
        }

    }

}
