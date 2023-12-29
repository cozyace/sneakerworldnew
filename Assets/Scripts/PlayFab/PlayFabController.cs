using Authentication;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.EditorModels;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LoginResult = PlayFab.ClientModels.LoginResult;
using PlayFabError = PlayFab.PlayFabError;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController instance;

    private string userEmail;
    private string userPassword;
    private string confirmPassword;
    private string username;

    [Header("Reference Manager")]
    [SerializeField] private DefaultValues defaults;

    [Header("Login References")]
    [SerializeField] private GameObject loginUI;
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private TMP_Text loginOutputText;
    [SerializeField] private Button loginButton;

    [Header("Register References")]
    [SerializeField] private GameObject registerUI;
    [SerializeField] private TMP_InputField registerUsername;
    [SerializeField] private TMP_InputField registerEmail;
    [SerializeField] private TMP_InputField registerPassword;
    [SerializeField] private TMP_InputField registerConfirmPassword;
    [SerializeField] private TMP_Text registerOutputText;
    [SerializeField] private Button registerButton;

    [Header("Leaderboards")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject leaderboardListing;
    [SerializeField] private RectTransform listingTransform;

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
    }

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "8C815";
        }

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");

            LoginButton();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) AssignObjects();
        if (SceneManager.GetActiveScene().buildIndex == 1) AssignMainObjects();
    }

    private void AssignObjects()
    {
        loginEmail = defaults.loginEmail;
        loginPassword = defaults.loginPassword;
        loginOutputText = defaults.loginOutputText;
        loginButton = defaults.loginButton;
        loginButton.onClick.AddListener(() => { LoginButton(); });

        registerUsername = defaults.registerUsername;
        registerEmail = defaults.registerEmail;
        registerPassword = defaults.registerPassword;
        registerConfirmPassword = defaults.registerConfirmPassword;
        registerOutputText = defaults.registerOutputText;
        registerButton = defaults.registerButton;
        registerButton.onClick.AddListener(() => { SignupButton(); });
    }

    private void AssignMainObjects()
    {
        GameManager.instance.aiManager.enabled = 
        GameManager.instance.uiManager.enabled = 
        GameManager.instance.employeeManager.enabled =
        GameManager.instance.upgradesManager.enabled = 
        GameManager.instance.inventoryManager.enabled =
        true;

    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        SceneManager.LoadSceneAsync(1);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong!  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());

        loginOutputText.text = error.GenerateErrorReport();
    }

    private void OnSignupSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Signup successful!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayerPrefs.SetString("USERNAME", username);

        registerOutputText.text = "Account created successfully!";
    }

    private void OnSignupFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong!  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());

        registerOutputText.text = error.GenerateErrorReport();
    }

    public void SetPlayerDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest()
            { DisplayName = displayName }, 
            result => Debug.Log("Set display name successfully"),
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    public void SetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void SetPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void SetConfirmPassword(string passwordIn)
    {
        confirmPassword = passwordIn;
    }

    public void SetUsername(string usernameIn)
    {
        username = usernameIn;
    }

    public void LoginButton()
    {
        Debug.Log("useremail " + userEmail);
        Debug.Log("password " + loginPassword);

        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void SignupButton()
    {
        if (userPassword != confirmPassword)
            registerOutputText.text = "Passwords don't match!";

        else
        {
            var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnSignupSuccess, OnSignupFailure);
            SetPlayerDisplayName(PlayerPrefs.GetString("USERNAME"));
        }
    }

    public void SignOutButton()
    {
        SaveData();
        HardReset();
        SceneManager.LoadSceneAsync(0);
        ClearInputs();
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

    private void ClearUI()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }

    private void SetPlayerStats(PlayerStats data)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStatistics",
            FunctionParameter = new { data.level, data.experience, data.cash, data.gems },
            GeneratePlayStreamEvent = true,
        },
        result => { Debug.Log("Successfully updated cloud statistics"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    public void GetPlayerStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatus,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetPlayerProfile(
            new GetPlayerProfileRequest(), 
            result => GameManager.instance.playerStats.username = result.PlayerProfile.DisplayName, 
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetStatus(GetPlayerStatisticsResult result)
    {
        PlayerStats playerStats = GameManager.instance.playerStats;
        
        foreach (var eachStat in result.Statistics)
        { 
            Debug.Log($"Statistic {eachStat.StatisticName}, {eachStat.Value}");

            switch (eachStat.StatisticName)
            {
                case "level":
                    playerStats.level = eachStat.Value; 
                    break;

                case "experience":
                    playerStats.experience = eachStat.Value;
                    break;

                case "cash":
                    playerStats.cash = eachStat.Value;
                    break;

                case "gems":
                    playerStats.gems = eachStat.Value;
                    break;
            }
        }
    }

    private void HardReset()
    {
        PlayerPrefs.DeleteAll();
    }

    public void GetLeaderboard()
    {
        SaveData();

        if (defaults == null)
        {
            defaults = FindObjectOfType<DefaultValues>();
        }

        leaderboardPanel = defaults.leaderboardPanel;
        listingTransform = defaults.listingTransform;
        leaderboardPanel.SetActive(true);

        var requestLeaderboard = new GetLeaderboardRequest { 
            StartPosition = 0, 
            StatisticName = "cash",  
            MaxResultsCount = 50
        };
        PlayFabClientAPI.GetLeaderboard(
            requestLeaderboard, 
            OnGetLeaderboard,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetLeaderboard(GetLeaderboardResult result)
    {
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject listing = Instantiate(leaderboardListing, listingTransform);
            Leaderboard leaderboard = listing.GetComponent<Leaderboard>();
            leaderboard.playerName.text = player.DisplayName;
            leaderboard.cash.text = player.StatValue.ToString();
        }
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel = GameManager.instance.leaderboardPanel;
        listingTransform = GameManager.instance.listingTransform;

        int childCount = listingTransform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var leaderboard = listingTransform.GetChild(i);

            for (int j = 0; j < leaderboard.childCount; j++)
            {
                Destroy(leaderboard.GetChild(j).gameObject);
            }

            Destroy(leaderboard.gameObject);
        }

        leaderboardPanel.SetActive(false);
    }

    private void SaveData()
    {
        SetPlayerStats(GameManager.instance.playerStats);
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            SaveData();
    }
}