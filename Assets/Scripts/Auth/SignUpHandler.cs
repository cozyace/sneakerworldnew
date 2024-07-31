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
        public const string INVALID_INPUT_MESSAGE = "Invalid format! Check your username and password again.";

        // A message to throw if the user's email is not verified.
        public const string UNVERIFIED_EMAIL_MESSAGE = "User email not verified! Check your inbox.";
        
        // A message to send when the sign up is successful.
        public const string SUCCESSFUL_SIGNUP_MESSAGE = "Sign up successful! Check email for verification."

        // An event to trigger when the log in process has begun.
        public UnityEvent onSignupStartEvent;

        // An event to trigger when the log in process succeeds.
        public UnityEvent<string> onSignupSuccessEvent;

        // An event to trigger when the log in process fails.
        public UnityEvent<string> onSignupFailedEvent;


        // Process the logic for signing the player up.
        [Button("Attempt Sign Up")]
        public async Task SignupAsync(string username, string email, string password, string confirmedPassword) {

            // Start the login event.
            onSignupStartEvent.Invoke();

            try {

                // Check whether the sign up input is valid.
                if (!ValidateSignupInput(username, email, password, confirmedPassword)) {
                    throw new Exception(INVALID_INPUT_MESSAGE);
                }

                AuthResult result = await FirebaseManager.CreateUserInDatabase(email, password);
                Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");

                // Set the new user as the current active user.
                FirebaseManager.SetCurrentUser(result.User);

                // Send an email to the user to verify.
                FirebaseManager.SendSignupEmailVerification();

                // Trigger a successful sign up event.
                onSignupSuccessEvent.Invoke(SUCCESSFUL_SIGNUP_MESSAGE);

                await SaveDataAsync(userId, playerStats);

            }
            catch (FirebaseException exception) {
                Debug.LogError($"Signup failed: {exception.Message}");
                onSignupFailedEvent.Invoke($"{exception.Message}");
            }

        }

        private bool ValidateSignupInput(string username, string email, string password, string confirmedPassword) {
            return !string.IsNullOrEmpty(username) 
                && !string.IsNullOrEmpty(password) 
                && !string.IsNullOrEmpty(password)
                && !string.IsNullOrEmpty(email) 
                && IsValidEmail(email) 
                && password == confirmedPassword;
        }

    }

}
