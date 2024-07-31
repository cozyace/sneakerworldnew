// System.
using System;
using System.Collections;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
// Firebase.
using Firebase;
using Firebase.Auth;
using Firebase.Database;
// using Firebase.Crashlytics;

namespace SneakerWorld {

    /// <summary>
    /// Allows the game logic to connect to the back-end database.
    /// </summary>
    public class FirebaseManager : MonoBehaviour {

        // The singleton.
        public static FirebaseManager Instance = null; 

        // The authorization.
        private FirebaseAuth defaultAuth;

        // The firebase user.
        private FirebaseUser currentUser;
        public FirebaseManager user {
            get { return currentUser; }
            set { currentUser = value; } // just in case.
        }

        // A reference to the database.
        public DatabaseReference databaseRoot;

        // The users ID.
        public string userId;

        // Runs once on instantiation.
        private async void Awake() {
            if (Instance == null) {
                await OnInstantiate();
            }
            else {
                Destroy(gameObject);
            }
        }

        // Sets the parameters necessary when instantiating firebase.
        private async Task OnInstantiate() {
            // Set the singleton.
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Clean the firebase dependencies to be ready to use.
            await FixDependencies();

            // Get the default instances.
            defaultAuth = FirebaseAuth.DefaultInstance;
            Debug.Log($"Found Default Auth: {defaultAuth!=null}");

            databaseRoot = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log($"Found Database Root: {databaseRoot!=null}");

            // Initialize the crash analytics and set the app to debug console.
            FirebaseApp.LogLevel = LogLevel.Debug;

        }

        // Initializes the crash analytics.
        private async Task FixDependencies() {
            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available) {
                    // Crashlytics uses the DefaultInstance, so it needs to exist.
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    Debug.Log("Crashlytics Initialized");
                }
                else {
                    Debug.LogError(System.String.Format(
                        $"Could not resolve all Firebase dependencies: {dependencyStatus}"
                    ));
                }
            });
        }

        // Set the current user.
        public static void SetCurrentUser(FirebaseUser user) {
            Instance.currentUser = user;
        }

        // Checks for the user in the database.
        public static async Task<AuthResult> CheckForUserInDatabase(string email, string password) {
            Credential credential = EmailAuthProvider.GetCredential(email, password);
            return await Instance.defaultAuth.SignInAndRetrieveDataWithCredentialAsync(credential);
        }

        // Creates a the user in the database.
        public static async Task<AuthResult> CreateUserInDatabase(string email, string password) {
            return await Instance.defaultAuth.CreateUserWithEmailAndPasswordAsync(email, password);
        }

        // Sends an email asking the user to verify their account.
        public static async void SendEmailVerification(FirebaseUser user) {
            Instance.StartCoroutine(SendEmailForVerificationAsync());
        }

        // Asynchronously sends an email asking the user to verify their account.
        IEnumerator SendEmailForVerificationAsync(FirebaseUser user) {
            if (user == null) {
                yield return null;
            }

            // Start a task sending an email, and wait for it to complete.
            Task sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            // Debug whether the email was sent or not.
            if (sendEmailTask.Exception != null) {
                Debug.Log("Email send error");
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;
            }
            else {
                Debug.Log("Email successfully sent!");
            }

        }

        // Sets a generic value in the database.
        public static async Task SetDatabaseValue<T>(string userID, string valueName, T value) {
            try {
                await Instance.databaseRoot.Child($"users/{userID}/{valueName}").SetValueAsync(value);
            } 
            catch(FirebaseException exception) {
                Debug.Log(exception.Message);
            }
        }

        // Get a generic value from the database.
        public static async Task<T> GetDatabaseValue<T>(string userID, string valueName) {
            try {
                DataSnapshot currentValue = await Instance.databaseRoot.Child($"users/{userID}/{valueName}").GetValueAsync();
                if (currentValue.GetRawJsonValue() != null) {
                    string trimmedValue = currentValue.GetRawJsonValue().Trim('"');
                    T result = TryConversion<T>(trimmedValue);
                    return result;
                }
            }
            catch(FirebaseException exception) {
                Debug.Log(exception.Message);
            }
            return default(T);
        }

        // Helper method to safely convert values from the database. 
        public static T TryConversion<T>(string input) {
            T result = default(T);
            try {
                result = (T)Convert.ChangeType(input, typeof(T));
            }
            catch {
                Debug.Log("Value was of invalid type.");
            }
            return result;
        }

    }

}
