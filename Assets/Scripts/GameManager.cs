using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Managers")] 
    public AudioManager audioManager;
    public UIManager uiManager;
    public EmployeeManager employeeManager;
    public AIManager aiManager;
    public UpgradesManager upgradesManager;
    public InventoryManager inventoryManager;

    [Header("Player Stats")]
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] private int xpIncreaseAmount;
    public int xpPerLevel;

    [Header("Sneakers")] 
    [SerializeField] public List<Sneaker> _sneakers;

    [Header("Leaderboards")]
    public GameObject leaderboardPanel;
    public RectTransform listingTransform;

    private bool hasAssignedInitialValues;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        InvokeRepeating(nameof(AddExperience), 0, 60);
        PlayFabController.instance.GetPlayerStats();
        PlayFabController.instance.GetPlayerData();

        var data = Resources.Load<TextAsset>("data");
        var splitDataset = data.text.Split('\n' );

        for (int i = 0; i < splitDataset.Length; i++)
        {
            string[] row = splitDataset[i].Split( ',');
            Sneaker sneaker = new Sneaker()
            {
                name = row[0], rarity = row[1].ToEnum<SneakerRarity>(), imagePath = $"{row[2]}.png"
            };
            _sneakers.Add(sneaker);
        }
    }

    private void Update()
    {
        StartCoroutine(AssignInitialValues(hasAssignedInitialValues));
    }

    private IEnumerator AssignInitialValues(bool hasAssignedValues)
    {
        while (uiManager.username.text == "")
        {
            if (!hasAssignedValues)
            {
                yield return new WaitForSeconds(0.5f);
                UIManager.instance.UpdateUI(playerStats);
                hasAssignedValues = true;
            }
        }
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

    public void LeaderboardButton()
    {
        PlayFabController.instance.GetLeaderboard();
    }

    public void CloseLeaderBoard()
    {
        PlayFabController.instance.CloseLeaderboard();
    }

    public void SignOutButton()
    {
        aiManager.enabled = uiManager.enabled = employeeManager.enabled = 
            upgradesManager.enabled = inventoryManager.enabled = false;
        PlayFabController.instance.SignOutButton();
    }
}