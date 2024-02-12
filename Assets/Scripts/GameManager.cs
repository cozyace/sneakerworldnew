using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference dbReference;

    [Header("Managers")]
    public InventoryStats inventoryStats;
    public FirebaseManager firebase;
    public AudioManager audioManager;
    public UIManager uiManager;
    public EmployeeManager employeeManager;
    public AIManager aiManager;
    public UpgradesManager upgradesManager;
    public InventoryManager inventoryManager;

    [Header("Player Stats")]
    [SerializeField] private string userId;
    public PlayerStats playerStats;
    [SerializeField] private int xpIncreaseAmount;
    public int xpPerLevel;

    [Header("Sneakers")] 
    public List<Sneaker> _sneakers;

    [Header("Leaderboards")]
    public GameObject leaderboardPanel;
    public RectTransform listingTransform;
    public Leaderboard leaderboardListing;

    [Header("Friends")]
    public TMPro.TextMeshProUGUI logSearchUsernamesText;
    public UserItem addFriendListing;
    public RectTransform addFriendListingTransform;

    private void Awake()
    {   
        if (firebase == null)
            firebase = FindObjectOfType<FirebaseManager>();

        playerStats = JsonUtility.FromJson<PlayerStats>(PlayerPrefs.GetString("PLAYER_STATS"));

        var data = Resources.Load<TextAsset>("data");
        var splitDataset = data.text.Split('\n' );

        for (int i = 0; i < splitDataset.Length; i++)
        {
            string[] row = splitDataset[i].Split(',');
            Sneaker sneaker = new()
            {
                name = row[0], rarity = row[1].ToEnum<SneakerRarity>(), imagePath = $"{row[2]}.png"
            };
            _sneakers.Add(sneaker);
        }
    }

    private async void Start()
    {
        uiManager.UpdateUI(playerStats);
        userId = firebase.auth.CurrentUser.UserId;
        await LeaderboardRankings();
    }

    public void UpdateUserData(PlayerStats data)
    {
        playerStats = data;
        uiManager.UpdateUI(playerStats);
    } 
    
    private void AddExperience()
    {
        AddExperience(xpIncreaseAmount);
    }

    public void AddExperience(int xp)
    {
        int xpTillNextLevel = (playerStats.level * xpPerLevel) - playerStats.experience;
        if (xp < xpTillNextLevel)
        {
            playerStats.experience += xpIncreaseAmount;
        }
        else if (xp >= xpTillNextLevel)
        {
            playerStats.level++;
            playerStats.experience = xp - xpTillNextLevel;
        }
        uiManager.UpdateUI(playerStats);
    }

    public void AddEmployee()
    {
        employeeManager.AddEmployee();
    }

    public int GetCash()
    {
        return playerStats.cash;
    }

    public void AddCash(int cash)
    {
        playerStats.cash += cash;
        uiManager.UpdateUI(playerStats);
    }

    public void DeductCash(int cash)
    {
        playerStats.cash -= cash;
        uiManager.UpdateUI(playerStats);
    }

    public async Task LeaderboardRankings()
    {
        var rankings = await firebase.CalculateLeaderboardRankings();

        foreach (var rank in rankings)
        {
            Leaderboard _leaderboardListing = Instantiate(leaderboardListing, listingTransform);
            _leaderboardListing.playerName.text = rank.Key;
            _leaderboardListing.cash.text = rank.Value.ToString();
        }

        leaderboardPanel.SetActive(true);
    }

    public async void LeaderboardButton()
    {
        await LeaderboardRankings();
    }

    public void CloseLeaderboard()
    {
        foreach (Transform child in listingTransform)
        {
            Destroy(child.gameObject);
        }

        leaderboardPanel.SetActive(false);
    }

    public async void OnSearchUsers(string _username)
    {
        try
        {
            logSearchUsernamesText.gameObject.SetActive(false);
            foreach (Transform child in addFriendListingTransform)
                if (!child.CompareTag("DONOTDESTROY")) Destroy(child.gameObject);

            List<string> users = await firebase.SearchUsers(_username);
            HashSet<string> uniqueUsers = new();
            
            if (users.Count == 0)
            {
                logSearchUsernamesText.gameObject.SetActive(true);
                logSearchUsernamesText.text = "No matching users found!";
                return;
            }

            foreach (string user in users)
            {
                if (!uniqueUsers.Contains(user)) 
                {
                    uniqueUsers.Add(user);
                    UserItem userItem = Instantiate(addFriendListing, addFriendListingTransform);
                    userItem.usernameText.text = user;
                }
            }
        }
        catch (FirebaseException e)
        {
            logSearchUsernamesText.gameObject.SetActive(true);
            logSearchUsernamesText.text = e.Message;
            Debug.LogError(e.Message);
        }
    }

    public async void SignOutButton()
    {
        aiManager.enabled = uiManager.enabled = employeeManager.enabled = 
            upgradesManager.enabled = inventoryManager.enabled = false;
        await firebase.SaveDataAsync(firebase.auth.CurrentUser.UserId, playerStats);
        firebase.auth.SignOut();
        PlayerPrefs.DeleteAll();
        await firebase.RunCoroutine(firebase.LoadSceneAsynchronously(0));
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            print("Saving data...");
            // Fix later to implement save data every interval
            StartCoroutine(SaveData());
        }
    }

    IEnumerator SaveData()
    {
        PlayerPrefs.SetString("PLAYER_STATS", JsonUtility.ToJson(playerStats));
        yield return null;
    }
}

