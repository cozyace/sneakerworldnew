using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Firebase")]
    public DatabaseReference dbReference;

    [Header("Managers")]
    public FirebaseManager firebase;
    public AudioManager audioManager;
    public UIManager uiManager;
    public EmployeeManager employeeManager;
    public AIManager aiManager;
    public UpgradesManager upgradesManager;
    public InventoryManager inventoryManager;
    public CustomerQueue _CustomerQueue;

    public SneakerDatabaseObject SneakerDatabase;

    [Header("Player Stats")]
    [SerializeField] private string userId;
    public PlayerStats playerStats;
    [SerializeField] private int xpIncreaseAmount;
    public int xpPerLevel;

    [Header("Leaderboards")]
    public GameObject leaderboardPanel;
    public RectTransform listingTransform;
    public Leaderboard leaderboardListing;

    [Header("Friends")]
    public List<string> searchedUsers;
    public List<string> _friends;
    public List<string> _friendRequests;
    public List<string> _friendRequestsSent;
    public TMPro.TextMeshProUGUI logSearchUsernamesText;
    public UserItem friendListing;
    public UserItem addFriendListing;
    public RectTransform friendListingTransform;
    public RectTransform addFriendListingTransform;
    public string _friendUsernameSelected;
    private List<string> instantiatedFriendNames = new();
    private List<UserItem> instantiatedFriendListings = new();
    private List<string> instantiatedFriendRequestsNames = new();
    private List<GameObject> instantiatedFriendRequestsListings = new();
    public GameObject tradePanel;

    public string Notifications;

    private void Awake()
    {
        if (firebase == null)
            firebase = FindObjectOfType<FirebaseManager>();

        playerStats = JsonUtility.FromJson<PlayerStats>(PlayerPrefs.GetString("PLAYER_STATS"));
    }

    public void Update()
    {
        UpdateNotifications();
    }
    
    //This is used to grab all notifications the player has in their account's database.
    public async void UpdateNotifications()
    {
        Notifications = await firebase.GetUserNotifications(firebase.userId);
        if (Notifications.Contains("Your listing of") || Notifications.Contains("You've listed"))
        {
            print("Listing Notification Found.");
            PlayerStats newStats = await firebase.LoadDataAsync(firebase.userId);

            playerStats = newStats;
            
            uiManager.UpdateUI(playerStats);

            await firebase.ClearNotifications(firebase.userId);
        }
        
    }

    private async void Start()
    {
        uiManager.UpdateUI(playerStats);
        userId = firebase.auth.CurrentUser.UserId;
        await LeaderboardRankings();
        _friends = await firebase.UpdateFriendsAsync();
        _friendRequests = await firebase.UpdateFriendRequestsAsync();
        _friendRequestsSent = await firebase.UpdateFriendRequestsSentAsync();
        SetupDatabaseListeners();
        playerStats = await firebase.LoadDataAsync(firebase.userId);
        InvokeRepeating(nameof(SaveToDatabase), 0f, 5f);
    }

    private void SetupDatabaseListeners()
    {
        var snapshot = firebase.dbReference.Child($"users/{userId}/friends");
        snapshot.ChildAdded += ListenForFriendRequests;
        snapshot.ChildRemoved += ListenForFriendRequests;
        snapshot.ChildChanged += ListenForFriends;
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
        employeeManager.SetActiveEmployeeCount(employeeManager.ActiveEmployees.Count + 1);
    }

    public int GetCash()
    {
        return playerStats.cash;
    }

    public int GetGems()
    {
        return playerStats.gems;
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

    public void DeductGems(int gems)
    {
        playerStats.gems -= gems;
        uiManager.UpdateUI(playerStats);
    }

    public async Task LeaderboardRankings()
    {
        var rankings = await firebase.CalculateLeaderboardRankingsAsync();

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

    public async Task UpdateFriendRequests()
    {
        _friendRequests.Clear();
        _friendRequestsSent.Clear();
        _friendRequests = await firebase.UpdateFriendRequestsAsync();
        _friendRequestsSent = await firebase.UpdateFriendRequestsSentAsync();
    }

    public async Task UpdateFriends()
    {
        _friends.Clear();
        _friends = await firebase.UpdateFriendsAsync();
    }

    public async void ListenForFriendRequests(object sender, ChildChangedEventArgs args)
    {
        ClearUI();
        await UpdateFriendRequests();
        await ListFriends();
    }

    public async void ListenForFriends(object sender, ChildChangedEventArgs args)
    {
        ClearUI();
        await UpdateFriends();
        await ListFriends();
    }

    public async void OnSearchUsers(string _username)
    {
        try
        {
            logSearchUsernamesText.gameObject.SetActive(false);
            foreach (GameObject child in instantiatedFriendRequestsListings)
                DestroyImmediate(child);

            instantiatedFriendRequestsNames.Clear();
            searchedUsers.Clear();
            searchedUsers = await firebase.SearchUsersAsync(_username);

            foreach (string friend in _friends)
            {
                string username = await firebase.GetUsernameFromUserIdAsync(friend);

                if (searchedUsers.Contains(username))
                    searchedUsers.Remove(username);
            }

            foreach (string friendRequest in _friendRequests)
            {
                string username = await firebase.GetUsernameFromUserIdAsync(friendRequest);

                if (searchedUsers.Contains(username))
                    searchedUsers.Remove(username);
            }

            if (searchedUsers.Count == 0)
            {
                logSearchUsernamesText.gameObject.SetActive(true);
                logSearchUsernamesText.text = "No matching users found!";
                return;
            }

            foreach (string user in searchedUsers)
            {
                if (!instantiatedFriendRequestsNames.Contains(user))
                {
                    instantiatedFriendRequestsNames.Add(user);
                    UserItem userItem = Instantiate(addFriendListing, addFriendListingTransform);
                    userItem.usernameText.text = user;

                    if (_friendRequestsSent.Contains(await firebase.GetUserIdFromUsernameAsync(user)))
                        UpdateAddFriendUI(userItem);

                    instantiatedFriendRequestsListings.Add(userItem.gameObject);
                }
            }
        }
        catch (FirebaseException e)
        {
            logSearchUsernamesText.gameObject.SetActive(true);
            logSearchUsernamesText.text = e.Message;
        }
    }

    public async Task ListFriends()
    {
        foreach (UserItem child in instantiatedFriendListings)
            Destroy(child);

        instantiatedFriendNames.Clear();

        foreach (var friend in _friends)
        {
            string username = await firebase.GetUsernameFromUserIdAsync(friend);

            if (!instantiatedFriendNames.Contains(username))
            {
                instantiatedFriendNames.Add(username);
                UserItem userItem = Instantiate(friendListing, friendListingTransform);
                userItem.usernameText.text = username;
                UpdateFriendsUI(userItem);
                instantiatedFriendListings.Add(userItem);
            }
        }

        foreach (var friend in _friendRequests)
        {
            string username = await firebase.GetUsernameFromUserIdAsync(friend);

            if (!instantiatedFriendNames.Contains(username))
            {
                instantiatedFriendNames.Add(username);
                UserItem userItem = Instantiate(friendListing, friendListingTransform);
                userItem.usernameText.text = username;
                instantiatedFriendListings.Add(userItem);
            }
        }
    }

    public void UpdateAddFriendUI(UserItem item)
    {
        if (item.addFriendButton != null) item.addFriendButton.SetActive(false);
        if (item.cancelFriendRequestButton != null) item.cancelFriendRequestButton.SetActive(true);
    }

    public void UpdateFriendsUI(UserItem item)
    {
        if (item.acceptFriendRequestButton != null) item.acceptFriendRequestButton.SetActive(false);
        if (item.declineFriendRequestButton != null) item.declineFriendRequestButton.SetActive(false);
        if (item.tradeButtton != null) item.tradeButtton.SetActive(true);
    }

    public void UpdateCancelRequestUI(UserItem item)
    {
        if (item.tradeButtton != null) item.tradeButtton.SetActive(false);
        if (item.acceptFriendRequestButton != null) item.acceptFriendRequestButton.SetActive(false);
        if (item.cancelFriendRequestButton != null) item.cancelFriendRequestButton.SetActive(false);
        if (item.declineFriendRequestButton != null) item.declineFriendRequestButton.SetActive(false);
        if (item.addFriendButton != null) item.addFriendButton.SetActive(true);
    }

    public void CloseLeaderboard()
    {
        foreach (Transform child in listingTransform)
        {
            Destroy(child.gameObject);
        }

        leaderboardPanel.SetActive(false);
    }

    private void ClearUI()
    {
        foreach (UserItem child in instantiatedFriendListings)
            Destroy(child);

        foreach (GameObject child in instantiatedFriendRequestsListings)
            Destroy(child);
    }

    public async void SignOutButton()
    {
        aiManager.enabled = uiManager.enabled = employeeManager.enabled =
            upgradesManager.enabled = inventoryManager.enabled = false;
        await firebase.SaveDataAsync(firebase.auth.CurrentUser.UserId, playerStats);
        firebase.auth.SignOut();
        PlayerPrefs.DeleteAll();
        await firebase.RunCoroutine(firebase.LoadSceneAsync(0));
    }

    private async void SaveToDatabase()
    {
        await SaveDataAsyc(firebase.userId);
        //playerStats = await firebase.LoadDataAsync(firebase.userId);
    }

    private async Task SaveDataAsyc(string _userId)
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        var data = new Dictionary<string, object>()
        {
            ["username"] = playerStats.username,
            ["level"] = playerStats.level,
            ["experience"] = playerStats.experience,
            ["cash"] = playerStats.cash,
            ["gems"] = playerStats.gems,
        };

        try
        {
            await dbReference.Child($"users/{_userId}").UpdateChildrenAsync(data);

            foreach (SneakersOwned sneaker in inventoryManager.sneakersOwned)
            {
                var sneakerData = new Dictionary<string, object>
                {
                    ["name"] = sneaker.name,
                    ["quantity"] = sneaker.quantity,
                    ["purchasePrice"] = sneaker.purchasePrice,
                    ["rarity"] = (int)sneaker.rarity
                };

                await dbReference.Child($"users/{_userId}/sneakers/{sneaker.name}").UpdateChildrenAsync(sneakerData);
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError(e.Message);
        }
    }
    
    
    
    

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            print("Saving data...");
            StartCoroutine(SaveData());
        }
    }

    IEnumerator SaveData()
    {
        PlayerPrefs.SetString("PLAYER_STATS", JsonUtility.ToJson(playerStats));
        yield return null;
    }
}
