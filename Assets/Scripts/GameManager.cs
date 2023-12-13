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

    [Header("User Data")]
    [SerializeField] public UserData userData;
    [SerializeField] private int xpIncreaseAmount;
    public int xpPerLevel;

    [Header("Sneakers")] [SerializeField] public List<Sneaker> _sneakers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        InvokeRepeating(nameof(AddExperience), 0, 60);
        uiManager.UpdateUI(userData);
        FirebaseManager.instance.SetupListeners();

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
    
    public void UpdateUserData(UserData data)
    {
        userData = data;
        uiManager.UpdateUI(userData);
    } 
    
    private void AddExperience()
    {
        AddExperience(xpIncreaseAmount);
    }

    public void AddExperience(int xp)
    {
        int xpTillNextLevel = (userData.level * xpPerLevel) - userData.experience;
        if (xp < xpTillNextLevel)
        {
            userData.experience += xpIncreaseAmount;
        }
        else if (xp >= xpTillNextLevel)
        {
            userData.level++;
            userData.experience = xp - xpTillNextLevel;
        }
        uiManager.UpdateUI(userData);
    }

    public void AddEmployee()
    {
        employeeManager.AddEmployee();
    }

    public int GetCash()
    {
        return userData.cash;
    }

    public void AddCash(int cash)
    {
        userData.cash += cash;
        uiManager.UpdateUI(userData);
    }

    public void DeductCash(int cash)
    {
        userData.cash -= cash;
        uiManager.UpdateUI(userData);
    }

    public void SignOutButton()
    {
        aiManager.enabled = uiManager.enabled = employeeManager.enabled = 
            upgradesManager.enabled = inventoryManager.enabled = false;
        FirebaseManager.instance.SignOutButton();
    }
}