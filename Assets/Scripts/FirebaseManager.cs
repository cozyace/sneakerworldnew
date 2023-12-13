using System;
using System.Collections;
using System.Collections.Generic;
using Authentication;
using Firebase;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;
using TMPro;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [Header("Firebase")]
    public DatabaseReference dbReference;
    public FirebaseAuth auth;
    public FirebaseUser user;
    
    [Header("Login References")]
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_Text loginOutputText;
    
    [Header("Register References")]
    [SerializeField] private TMP_InputField registerUsername;
    [SerializeField] private TMP_InputField registerEmail;
    [SerializeField] private TMP_InputField registerPassword;
    [SerializeField] private TMP_InputField registerConfirmPassword;
    [SerializeField] private TMP_Text registerOutputText;

    private Coroutine loginCoroutine, signupCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        StartCoroutine(CheckAndFixDependencies());
    }

    private IEnumerator CheckAndFixDependencies()
    {
        var checkAndFixDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(predicate: () => checkAndFixDependenciesTask.IsCompleted);
        var dependencyResult = checkAndFixDependenciesTask.Result;
        if (dependencyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.Log($"Could not resolve all Firebase dependencies: {dependencyResult}");
        }
    }

    private void InitializeFirebase()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(CheckAutoLogin());
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void AssignObjects()
    {
        loginEmail = GameObject.FindGameObjectWithTag("loginEmail").GetComponent<TMP_InputField>();
        loginPassword = GameObject.FindGameObjectWithTag("loginPassword").GetComponent<TMP_InputField>();
        loginOutputText = GameObject.FindGameObjectWithTag("loginOutputText").GetComponent<TMP_Text>();

        registerUsername = GameObject.FindGameObjectWithTag("username").GetComponent<TMP_InputField>();
        registerEmail = GameObject.FindGameObjectWithTag("signupEmail").GetComponent<TMP_InputField>();
        registerPassword = GameObject.FindGameObjectWithTag("signupPassword").GetComponent<TMP_InputField>();
        registerConfirmPassword = GameObject.FindGameObjectWithTag("confirmPassword").GetComponent<TMP_InputField>();
        registerOutputText = GameObject.FindGameObjectWithTag("signupOutputText").GetComponent<TMP_Text>();
    }

    
    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if (user != null)
        {
            var reloadTask = user.ReloadAsync();
            yield return new WaitUntil(predicate: () => reloadTask.IsCompleted);
            if (reloadTask.Exception != null)
            {
                AuthUIManager.instance.LoginScreen();
            }
            else
            {
                AutoLogin();
            }
        }
        else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }
    
    private void AutoLogin()
    {
        if (user != null)
        {
            AuthUIManager.instance.LoadingScreen();
            AuthUIManager.instance.ChangeScene(1);
        }
        else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }
    
    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void SetupListeners()
    {
        dbReference.Child("users").Child(user.UserId).ValueChanged += (sender, args) =>
        {
            var userData = new UserData();
            var snapshot = args.Snapshot;
            userData.username = snapshot.Child("username").Value.ToString();
            userData.level = int.Parse(snapshot.Child("level").Value.ToString());
            userData.experience = int.Parse(snapshot.Child("experience").Value.ToString());
            userData.cash = int.Parse(snapshot.Child("cash").Value.ToString());
            userData.gems = int.Parse(snapshot.Child("gems").Value.ToString());

            if (SceneManager.GetActiveScene().buildIndex == 1)
                GameManager.instance.UpdateUserData(userData);
        };
    }
    
    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }
    
    private void ClearInputs()
    {
        loginEmail.text = "";
        loginPassword.text = "";
        registerUsername.text = "";
        registerEmail.text = "";
        registerPassword.text = "";
        registerConfirmPassword.text = "";
    }
    
    public void LoginButton()
    {
        if (loginCoroutine != null) StopCoroutine(loginCoroutine);
        loginCoroutine = StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));
    }
        
    public void RegisterButton()
    {
        if (signupCoroutine != null) StopCoroutine(signupCoroutine);
        signupCoroutine = StartCoroutine(RegisterLogic(registerUsername.text, registerEmail.text, registerPassword.text,
            registerConfirmPassword.text));
    }

    public void SignOutButton()
    {
        SaveData();
        auth.SignOut();
        AuthUIManager.instance.ChangeScene(0);
        AuthUIManager.instance.LoginScreen();
        AssignObjects();
        ClearInputs();
    }
    
    private IEnumerator LoginLogic(string email, string password)
    {
        print("Login button is working!");
        print(email); print(password);

        Credential credential = EmailAuthProvider.GetCredential(email, password);
        var loginTask = auth.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = error switch
            {
                AuthError.MissingEmail => "Please Enter Your Email",
                AuthError.MissingPassword => "Please Enter Your Password",
                AuthError.InvalidEmail => "Invalid Email",
                AuthError.WrongPassword => "Incorrect Password",
                AuthError.UserNotFound => "Account Does Not Exist",
                _ => "Unknown Error, Please Try Again"
            };

            loginOutputText.text = output;
        }
        else
        {
            if (user.IsEmailVerified)
            {
                AuthUIManager.instance.LoadingScreen();
                AuthUIManager.instance.ChangeScene(1);
            }
            else
            {
                // AuthUIManager.instance.ChangeScene(0);

                AuthUIManager.instance.LoadingScreen();
                AuthUIManager.instance.ChangeScene(1);
            }
        }
    }

    private IEnumerator RegisterLogic(string username, string email, string password, string confirmPassword)
        {
            if (username == "")
            {
                registerOutputText.text = "Please Enter A Username";
            }
            else if (password != confirmPassword)
            {
                registerOutputText.text = "Passwords Do Not Match";
            }
            else
            {
                var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
                yield return new WaitUntil(predicate: () => registerTask.IsCompleted);
                if (registerTask.Exception != null)
                {
                    FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;

                    string output = error switch
                    {
                        AuthError.InvalidEmail => "Invalid Email",
                        AuthError.EmailAlreadyInUse => "Email Already In Use",
                        AuthError.WeakPassword => "Weak Password",
                        AuthError.MissingEmail => "Please Enter Your Email",
                        AuthError.MissingPassword => "Please Enter Your Password",
                        _ => "Unknown Error, Please Try Again"
                    };

                    registerOutputText.text = output;
                }
                else
                {
                    UserProfile profile = new UserProfile
                    {
                        DisplayName = username
                    };
                    var defaultUserTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);
                    if (defaultUserTask.Exception != null)
                    {
                        user.DeleteAsync();
                        FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();
                        AuthError error = (AuthError)firebaseException.ErrorCode;

                        string output = error switch
                        {
                            AuthError.Cancelled => "Update User Cancelled",
                            AuthError.SessionExpired => "Session Expired",
                            _ => "Unknown Error, Please Try Again"
                        };
                        registerOutputText.text = output;
                    }
                    else
                    {
                        Debug.Log($"Firebase User Created Successfully {user.DisplayName} ({user.UserId})");
                        yield return StartCoroutine(AddInitialUserData());
                        AuthUIManager.instance.LoginScreen();
                        ClearInputs();
                        loginOutputText.text = "Successfully registered!";
                    }
                }
            }
        }
    
    private IEnumerator AddInitialUserData()
    {
        yield return UpdateUsernameDatabase(user.DisplayName);
        yield return UpdateLevel(1);
        yield return UpdateExperience(20);
        yield return UpdateCash(9999);
        yield return UpdateGems(10);
    }
    
    public void SaveData()
    {
        var userData = GameManager.instance.userData;
        StartCoroutine(UpdateUsernameDatabase(userData.username));
        StartCoroutine(UpdateLevel(userData.level));
        StartCoroutine(UpdateExperience(userData.experience));
        StartCoroutine(UpdateCash(userData.cash));
        StartCoroutine(UpdateGems(userData.gems));
    }
    
    private IEnumerator UpdateUsernameDatabase(string username)
    {
        var dbTask = dbReference.Child("users").Child(user.UserId).Child("username").SetValueAsync(username);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update username in database with {dbTask.Exception}");
        }
    }

    private IEnumerator UpdateLevel(int level)
    {
        var dbTask = dbReference.Child("users").Child(user.UserId).Child("level").SetValueAsync(level);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update level in database with {dbTask.Exception}");
        }
    }
    
    private IEnumerator UpdateExperience(int xp)
    {
        var dbTask = dbReference.Child("users").Child(user.UserId).Child("experience").SetValueAsync(xp);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update experience in database with {dbTask.Exception}");
        }
    }
    
    private IEnumerator UpdateCash(int cash)
    {
        var dbTask = dbReference.Child("users").Child(user.UserId).Child("cash").SetValueAsync(cash);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update cash in database with {dbTask.Exception}");
        }
    }

    private IEnumerator UpdateGems(int gems)
    {
        var dbTask = dbReference.Child("users").Child(user.UserId).Child("gems").SetValueAsync(gems);
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null)
        {
            Debug.LogError($"Failed to update gems in database with {dbTask.Exception}");
        }
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            SaveData();
    }
}