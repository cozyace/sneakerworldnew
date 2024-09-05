// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
// Firebase.
using Firebase;
using Firebase.Auth;
using Firebase.Database;
// using Firebase.Crashlytics;
// Json.
using Newtonsoft.Json;

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
        public static FirebaseUser CurrentUser {
            get { return Instance.currentUser; }
            set { Instance.currentUser = value; } // just in case.
        }

        // A reference to the database.
        public DatabaseReference databaseRoot;

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
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            FirebaseApp.LogLevel = LogLevel.Error;

            // Clean the firebase dependencies to be ready to use.
            await FixDependencies();

            // Get the default instances.
            defaultAuth = FirebaseAuth.DefaultInstance;
            Debug.Log($"Found Default Auth: {defaultAuth!=null}");

            databaseRoot = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log($"Found Database Root: {databaseRoot!=null}");

            // Initialize the crash analytics and set the app to debug console.

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
        public static void SendSignupEmailVerification(FirebaseUser user = null) {
            if (user == null) {
                user = CurrentUser;
            }
            Instance.StartCoroutine(Instance.IESendSignupEmailVerification(user));
        }

        // Asynchronously sends an email asking the user to verify their account.
        IEnumerator IESendSignupEmailVerification(FirebaseUser user) {
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
        public static async Task SetDatabaseValue<T>(string path, T value) {
            try {
                if (IsValidType<T>()) {
                    await Instance.databaseRoot.Child(path).SetValueAsync(value);
                }
                else {
                    string json = ToJSON<T>(value);
                    // Dictionary<string, object> dictionary = FromJSON<Dictionary<string, object>>(json); 
                    await Instance.databaseRoot.Child(path).SetRawJsonValueAsync(json);
                }
            } 
            catch(Exception exception) {
                Debug.LogError(exception.Message);
            }
        }

        // Get a generic value from the database.
        public static async Task<T> GetDatabaseValue<T>(string path, bool createIfEmpty = false) {
            try {
                DataSnapshot currentValue = await Instance.databaseRoot.Child(path).GetValueAsync();
                if (currentValue.GetRawJsonValue() != null) {
                    string trimmedValue = currentValue.GetRawJsonValue().Trim('"');

                    if (IsValidType<T>()) {
                        return TryConversion<T>(trimmedValue);
                    }
                    else {
                        return FromJSON<T>(trimmedValue);
                    }
                }
                else {
                    if (createIfEmpty) {
                        await SetDatabaseValue<T>(path, default(T));
                    }
                    return default(T);
                }
            }
            catch(Exception exception) {
                Debug.LogError(exception.Message);
            }
            return default(T);
        }
        
        public static bool IsValidType<T>() {
            if (typeof(T) != typeof(string) && typeof(T) != typeof(int) && typeof(T) != typeof(bool)) {
                return false;
            }
            return true;
        }

        public static string ToJSON<T>(T value) {
            return JsonUtility.ToJson(value);
        }

        public static T FromJSON<T>(string value) {
            return JsonUtility.FromJson<T>(value);
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

        // Get all the keys at the given path.
        public static async Task<List<string>> GetAllKeys(string path) {
            List<string> listOfKeys = new List<string>();
            DataSnapshot currentValue = await FirebaseManager.Instance.databaseRoot.Child(path).GetValueAsync();
            foreach (DataSnapshot child in currentValue.Children) { 
                listOfKeys.Add(child.Key);
            }
            return listOfKeys;
        }

        // Get all the keys at the given path.
        public static async Task<Dictionary<string, T>> GetDictionaryAt<T>(string path) {
            Dictionary<string, T> dict = new Dictionary<string, T>();

            DataSnapshot currentValue = await FirebaseManager.Instance.databaseRoot.Child(path).GetValueAsync();

            foreach (DataSnapshot child in currentValue.Children) { 
                dict.Add(child.Key, DataSnapshotToT<T>(child));
            }
            return dict;
        }

        public static T DataSnapshotToT<T>(DataSnapshot snapshot) {
            if (snapshot != null && snapshot.GetRawJsonValue() != null) {
                string trimmedValue = snapshot.GetRawJsonValue().Trim('"');
                if (IsValidType<T>()) {
                    return TryConversion<T>(trimmedValue);
                }
                else {
                    return FromJSON<T>(trimmedValue);
                }
            }
            return default(T);
        }

    }

}
