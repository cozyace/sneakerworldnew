using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import Firebase and Crashlytics
using Firebase;
//using Firebase.Crashlytics;

public class CrashlyticsInit : MonoBehaviour
{
    // Use this for initialization
    public void InitializeCrashlytics () {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // When this property is set to true, Crashlytics will report all
                // uncaught exceptions as fatal events. This is the recommended behavior.
                //Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                print("Crashlytics Initialized");
                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
               Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}",dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
}
