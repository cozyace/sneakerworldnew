﻿using Authentication;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference dbReference;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public string userId;

    [Header("Login")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Sign up")]
    public TMP_InputField SignupUsername;
    public TMP_InputField SignupEmail;
    public TMP_InputField SignupPassword;
    public TMP_InputField SignupConfirmPassword;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Extra")]
    public GameObject loginScreen;
    public GameObject signupScreen;
    public GameObject sceneLoadScreen;
    public GameObject loadingScreen;
    public TextMeshProUGUI logText;
    public PlayerStats playerStats;
    public Image loadingBar;

    private async void Start()
    {
        await InitializeDefaultValues();

        if (SceneManager.GetActiveScene().buildIndex == 0 && PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            loginEmail.text = PlayerPrefs.GetString("EMAIL");
            loginPassword.text = PlayerPrefs.GetString("PASSWORD");

            await LoginAsync();
        }
    }

    private async Task InitializeDefaultValues()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        await FirebaseApp.CheckAndFixDependenciesAsync();

        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public async Task LoginAsync()
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = loginEmail.text;
        string password = loginPassword.text;

        if (!ValidateLoginInput(email, password))
        {
            ShowLogMsg("Invalid email or password");
            return;
        }

        try
        {
            Credential credential = EmailAuthProvider.GetCredential(email, password);
            AuthResult result = await auth.SignInAndRetrieveDataWithCredentialAsync(credential);

            loadingScreen.SetActive(false);

            if (!result.User.IsEmailVerified)
            {
                ShowLogMsg("Please verify your email.");
                throw new Exception("User email not verified! Check your inbox.");
            }

            user ??= result.User;
            userId = result.User.UserId;
            PlayerPrefs.SetString("EMAIL", email);
            PlayerPrefs.SetString("PASSWORD", password);
            ShowLogMsg("Login successful!");
            
            await RunCoroutine(LoadDataEnum(() => StartCoroutine(LoadSceneAsynchronously(1))));
        }

        catch (FirebaseException e)
        {
            loadingScreen.SetActive(false);
            Debug.LogError($"Login failed: {e.Message}");
            ShowLogMsg(e.Message);
        }
    }

    public async Task SignupAsync()
    {
        loadingScreen.SetActive(true);

        string username = SignupUsername.text;
        string email = SignupEmail.text;
        string password = SignupPassword.text;
        string signupConfirmPassword = SignupConfirmPassword.text;

        if (!ValidateSignupInput(username, email, password, signupConfirmPassword))
        {
            ShowLogMsg("Invalid format! Check your username and password again.");
            return;
        }

        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            UserProfile profile = new()
            {
                DisplayName = username
            };

            playerStats.username = username;
            await result.User.UpdateUserProfileAsync(profile);
            userId = result.User.UserId;

            loadingScreen.SetActive(false);

            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            SignupUsername.text = "";
            SignupEmail.text = "";
            SignupPassword.text = "";
            SignupConfirmPassword.text = "";

            SendEmailVerification();

            ShowLogMsg("Sign up Successful! Check your email to verify your account.");
            PlayerPrefs.SetString("EMAIL", email);
            PlayerPrefs.SetString("PASSWORD", password);
            await SaveDataAsync(userId, playerStats);
        }

        catch (FirebaseException e)
        {
            loadingScreen.SetActive(false);
            Debug.LogError($"CreateUserWithEmailAndPasswordAsync encountered an error: {e.Message}");
            ShowLogMsg($"{e.Message}");
        }
    }

    public void SendEmailVerification()
    {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    IEnumerator SendEmailForVerificationAsync()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                print("Email send error");
                FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;

                switch (error)
                {
                    case AuthError.None:
                        break;
                    case AuthError.Unimplemented:
                        break;
                    case AuthError.Failure:
                        break;
                    case AuthError.InvalidCustomToken:
                        break;
                    case AuthError.CustomTokenMismatch:
                        break;
                    case AuthError.InvalidCredential:
                        break;
                    case AuthError.UserDisabled:
                        break;
                    case AuthError.AccountExistsWithDifferentCredentials:
                        break;
                    case AuthError.OperationNotAllowed:
                        break;
                    case AuthError.EmailAlreadyInUse:
                        break;
                    case AuthError.RequiresRecentLogin:
                        break;
                    case AuthError.CredentialAlreadyInUse:
                        break;
                    case AuthError.InvalidEmail:
                        break;
                    case AuthError.WrongPassword:
                        break;
                    case AuthError.TooManyRequests:
                        break;
                    case AuthError.UserNotFound:
                        break;
                    case AuthError.ProviderAlreadyLinked:
                        break;
                    case AuthError.NoSuchProvider:
                        break;
                    case AuthError.InvalidUserToken:
                        break;
                    case AuthError.UserTokenExpired:
                        break;
                    case AuthError.NetworkRequestFailed:
                        break;
                    case AuthError.InvalidApiKey:
                        break;
                    case AuthError.AppNotAuthorized:
                        break;
                    case AuthError.UserMismatch:
                        break;
                    case AuthError.WeakPassword:
                        break;
                    case AuthError.NoSignedInUser:
                        break;
                    case AuthError.ApiNotAvailable:
                        break;
                    case AuthError.ExpiredActionCode:
                        break;
                    case AuthError.InvalidActionCode:
                        break;
                    case AuthError.InvalidMessagePayload:
                        break;
                    case AuthError.InvalidPhoneNumber:
                        break;
                    case AuthError.MissingPhoneNumber:
                        break;
                    case AuthError.InvalidRecipientEmail:
                        break;
                    case AuthError.InvalidSender:
                        break;
                    case AuthError.InvalidVerificationCode:
                        break;
                    case AuthError.InvalidVerificationId:
                        break;
                    case AuthError.MissingVerificationCode:
                        break;
                    case AuthError.MissingVerificationId:
                        break;
                    case AuthError.MissingEmail:
                        break;
                    case AuthError.MissingPassword:
                        break;
                    case AuthError.QuotaExceeded:
                        break;
                    case AuthError.RetryPhoneAuth:
                        break;
                    case AuthError.SessionExpired:
                        break;
                    case AuthError.AppNotVerified:
                        break;
                    case AuthError.AppVerificationFailed:
                        break;
                    case AuthError.CaptchaCheckFailed:
                        break;
                    case AuthError.InvalidAppCredential:
                        break;
                    case AuthError.MissingAppCredential:
                        break;
                    case AuthError.InvalidClientId:
                        break;
                    case AuthError.InvalidContinueUri:
                        break;
                    case AuthError.MissingContinueUri:
                        break;
                    case AuthError.KeychainError:
                        break;
                    case AuthError.MissingAppToken:
                        break;
                    case AuthError.MissingIosBundleId:
                        break;
                    case AuthError.NotificationNotForwarded:
                        break;
                    case AuthError.UnauthorizedDomain:
                        break;
                    case AuthError.WebContextAlreadyPresented:
                        break;
                    case AuthError.WebContextCancelled:
                        break;
                    case AuthError.DynamicLinkNotActivated:
                        break;
                    case AuthError.Cancelled:
                        break;
                    case AuthError.InvalidProviderId:
                        break;
                    case AuthError.WebInternalError:
                        break;
                    case AuthError.WebStorateUnsupported:
                        break;
                    case AuthError.TenantIdMismatch:
                        break;
                    case AuthError.UnsupportedTenantOperation:
                        break;
                    case AuthError.InvalidLinkDomain:
                        break;
                    case AuthError.RejectedCredential:
                        break;
                    case AuthError.PhoneNumberNotFound:
                        break;
                    case AuthError.InvalidTenantId:
                        break;
                    case AuthError.MissingClientIdentifier:
                        break;
                    case AuthError.MissingMultiFactorSession:
                        break;
                    case AuthError.MissingMultiFactorInfo:
                        break;
                    case AuthError.InvalidMultiFactorSession:
                        break;
                    case AuthError.MultiFactorInfoNotFound:
                        break;
                    case AuthError.AdminRestrictedOperation:
                        break;
                    case AuthError.UnverifiedEmail:
                        break;
                    case AuthError.SecondFactorAlreadyEnrolled:
                        break;
                    case AuthError.MaximumSecondFactorCountExceeded:
                        break;
                    case AuthError.UnsupportedFirstFactor:
                        break;
                    case AuthError.EmailChangeNeedsVerification:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                print("Email successfully sent!");
            }
        }
    }

    private void ShowLogMsg(string msg)
    {
        logText.text = msg;
        logText.GetComponent<Animation>().Play();
    }

    public async Task SaveDataAsync(string _userId, PlayerStats _playerStats)
    {
        string json = JsonUtility.ToJson(_playerStats);
        await dbReference.Child("users").Child(_userId).SetRawJsonValueAsync(json);
    }

    IEnumerator LoadDataEnum(Action callback)
    {
        var _stats = dbReference.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate: () => _stats.IsCompleted);

        try
        {
            DataSnapshot snapshot = _stats.Result;
            if (snapshot == null)
            {
                print("Error fetching data. No data was found!");
                yield break;
            }

            string jsonData = snapshot.GetRawJsonValue();
            if (jsonData == null)
            {
                print("Error fetching data. No data was found!");
                yield break;
            }

            print($"Loaded Stats for {user.DisplayName}");
            ShowLogMsg($"Loaded Data for {user.DisplayName}");

            PlayerPrefs.SetString("PLAYER_STATS", jsonData);
            callback?.Invoke();
        }
        catch (FirebaseException e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    public IEnumerator LoadSceneAsynchronously(int sceneIndex)
    {
        if (loadingScreen != null) loginScreen.SetActive(false);
        if (signupScreen != null) signupScreen.SetActive(false);
        if (loadingScreen != null) loadingScreen.SetActive(false);
        if (sceneLoadScreen != null) sceneLoadScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        if (loadingScreen != null) loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingBar.fillAmount = progress;

            yield return null;
        }
    }

    public async Task<Dictionary<string, int>> CalculateLeaderboardRankings()
    {
        DataSnapshot userDocs = await dbReference.Child("users").GetValueAsync();
        Dictionary<string, int> userCash = new();

        foreach (DataSnapshot childSnapshot in userDocs.Children)
        {
            string username = childSnapshot.Child("username").Value.ToString();
            int cash = Convert.ToInt32(childSnapshot.Child("cash").Value);
            userCash.TryAdd(username, cash);
        }

        var rankedUsers = userCash
            .OrderByDescending(u => u.Value)
            .ToDictionary(u => u.Key, u => u.Value);

        return rankedUsers;
    }

    public async Task<List<string>> SearchUsers(string _username) 
    {
        DataSnapshot userDocs = await dbReference.Child("users").GetValueAsync();
        List<string> matchingUsers = new();

        foreach (DataSnapshot childSnapshot in userDocs.Children)
        {
            string username = childSnapshot.Child("username").Value.ToString();

            if (username != user.DisplayName && username.Contains(_username))
            {
                matchingUsers.Add(username);
            }
        }

        return matchingUsers;
    }

    private bool ValidateLoginInput(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        return true;
    }

    private bool ValidateSignupInput(string username, string email, string password, string confirmPassword)
    {

        if (string.IsNullOrEmpty(username))
        {
            return false;
        }

        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        if (!IsValidEmail(email))
        {
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        if (!IsValidPassword(password))
        {
            return false;
        }

        if (password != confirmPassword)
        {
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
    }

    private bool IsValidPassword(string password)
    {
        return password.Length >= 6;
        //     &&
        //    password.Any(char.IsUpper) &&
        //    password.Any(char.IsLower) &&
        //    password.Any(char.IsDigit);
    }

    public Task RunCoroutine(IEnumerator coroutine)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(RunCoroutineAndComplete(coroutine, tcs));
        return tcs.Task;
    }

    private IEnumerator RunCoroutineAndComplete(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
        yield return StartCoroutine(coroutine);
        tcs.SetResult(true);
    }

    public async void OnLoginButtonClick()
    {
        await LoginAsync();
    }

    public async void OnSignupButtonClick()
    {
        await SignupAsync();
    }

    public void ResetDataButton()
    {
        PlayerPrefs.DeleteAll();
        ShowLogMsg("All local data has been deleted!");
    }
}
