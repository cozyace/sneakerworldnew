using System;
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
    public FriendsUIManager _FriendsUIManager;
    public MarketManager _MarketManager;
    
    public SneakerDatabaseObject SneakerDatabase;

    [Header("Player Stats")]
    [SerializeField] private string userId;
    public PlayerStats playerStats;
    [SerializeField] private int xpIncreaseAmount; //How much does the exp needed increase per level?
    public int xpPerLevel; //How much EXP is needed to progress per leveL?
    


    public string[] Notifications;

    private void Awake()
    {
        if (firebase == null)
            firebase = FindFirstObjectByType<FirebaseManager>();

        playerStats = JsonUtility.FromJson<PlayerStats>(PlayerPrefs.GetString("PLAYER_STATS"));
    }

    public void Update()
    {
        UpdateNotifications();
    }
    
    //This is used to grab all notifications the player has in their account's database.
    private async void UpdateNotifications()
    {
        Notifications = await firebase.GetUserNotifications(firebase.userId);

        foreach (string t in Notifications)
        {
            if (t.Contains("EXPIRED"))
            {
                print("EXPIRED Notification Found.");

                //The expiration notifications use , to split the data for parsing. (Because with this one we need to actually grab the shoe ID, and quantity to give the player)
                string[] splitNotification = t.Split(',');
                
                int shoeId = int.Parse(splitNotification[1]); 
                int quantity = int.Parse(splitNotification[2]);

                //This method will also save the data, and refresh the inventory.
                inventoryManager.AddShoesToCollection(new SneakersOwned(SneakerDatabase.Database[shoeId].Name, quantity, SneakerDatabase.Database[shoeId].Value, SneakerDatabase.Database[shoeId].Rarity));
                
                print("Gave player expired shoe.");
            }
            //When your listing has been purchased by another user, update your cash/gems.
            else if (t.Contains("SOLD"))
            {
                print("SOLD Notification Found.");
                PlayerStats newStats = await firebase.LoadDataAsync(firebase.userId);
                playerStats = newStats;
                uiManager.UpdateUI(playerStats);
            }
        }
        
        await firebase.ClearNotifications(firebase.userId);
        //If your listing has expired, this will give you the shoes back.
        
    }

    private async void Start()
    {
        uiManager.UpdateUI(playerStats);
        userId = firebase.auth.CurrentUser.UserId;

        playerStats = await firebase.LoadDataAsync(firebase.userId);
        InvokeRepeating(nameof(SaveToDatabase), 0f, 5f);

        AFKEarningsCheck();
    }

    private async void AFKEarningsCheck()
    {
        //Set the player's data to say that they're logged in.
        await firebase.UpdateIsOnline(firebase.userId, true);
    
        DateTime lastLoggedOutTime = await firebase.GetLastLoggedOut(firebase.userId);

        if (lastLoggedOutTime == new DateTime())
            print("This is your first time logging in!");
        
        TimeSpan timeGone = DateTime.Now - lastLoggedOutTime;
        
        print($"You were gone for {timeGone.TotalMinutes} minutes");
        
        FindFirstObjectByType<WelcomeBackWindowUI>().TriggerWelcomeWindow(timeGone.TotalMinutes);
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

   

    public async void SignOutButton()
    {
        aiManager.enabled = uiManager.enabled = employeeManager.enabled =
            upgradesManager.enabled = inventoryManager.enabled = false;
        await firebase.SaveDataAsync(firebase.auth.CurrentUser.UserId, playerStats);
        firebase.auth.SignOut();
        PlayerPrefs.DeleteAll();
        await firebase.RunCoroutine(firebase.LoadSceneAsync(0));
    }

    public async void SaveToDatabase()
    {
        if (firebase.userId is null or "")
        {
            print("Tried to save while no data present!");
            return;
        }
        
        print("<size=14><color=blue>GAMEMANAGER</color> | Saving Data... </size>");

        if (Notifications.Length == 0)
        {
            await SaveDataAsyc(firebase.userId);
            return;
        }

        for (int i = 0; i < Notifications.Length; i++)
        {
            if(i == Notifications.Length-1)
                if(!Notifications[i].Contains("SOLD"))
                    await SaveDataAsyc(firebase.userId);
        }
        
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

            foreach (SneakersOwned sneaker in inventoryManager.SneakersOwned)
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
    
    
    
    

    private async void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            print($"<size=14><color=blue>GAMEMANAGER</color> | SAVING PLAYER PREFS.</size>");
            StartCoroutine(SaveData());
            await firebase.UpdateLastLoggedOut(firebase.userId);
            await firebase.UpdateIsOnline(firebase.userId, false);
        }
    }

    IEnumerator SaveData()
    {
        PlayerPrefs.SetString("PLAYER_STATS", JsonUtility.ToJson(playerStats));
        yield return null;
    }
}
