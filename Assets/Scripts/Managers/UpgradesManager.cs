using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradesManager : MonoBehaviour
{
  
    [Header("Employee Upgrades")]
    [SerializeField] private UpgradePanel EmployeesUpgrade; //The amount of employees working.
    [SerializeField] private UpgradePanel CounterUpgrade; //The amount of sale counters.
    [SerializeField] private UpgradePanel AdvertisementUpgrade; 
    [SerializeField] private UpgradePanel ImproveStoreUpgrade;
    [SerializeField] private UpgradePanel StorageUpgrade;
    [SerializeField] private UpgradePanel ShelvesUpgrade;
    
    [Header("Game Manager")]
    public GameManager GameManager;


    private void Start()
    {
        Invoke(nameof(SetInitialPanelValues), 1f);
    }
    
    
    private void SetInitialPanelValues()
    {
        EmployeesUpgrade.SetInitialValues($"Waiting time ~ {GameManager._StoreManager.AverageCustomerTransactionTime}s");
        CounterUpgrade.SetInitialValues($"{GameManager._StoreManager.CounterCount} Counters");
        AdvertisementUpgrade.SetInitialValues($"{GameManager._StoreManager.AverageCustomerSpawnTime} Average Spawn Time");
        ImproveStoreUpgrade.SetInitialValues($"Store Level - UNFINISHED");
        StorageUpgrade.SetInitialValues($"{GameManager.inventoryManager.GetTotalInventoryCount()} Total Inventory");
        ShelvesUpgrade.SetInitialValues($"{GameManager.inventoryManager.GetTotalShelfInventoryCount()} Total Shelf Spaces");
        //FINISH REST OF DESCRIPTIONS HERE.
    }
    
    private void SetDescriptions()
    {
        EmployeesUpgrade.SetDescription($"Waiting time ~ {GameManager._StoreManager.AverageCustomerTransactionTime}s");
        CounterUpgrade.SetDescription($"{GameManager._StoreManager.CounterCount} Counters");
        AdvertisementUpgrade.SetDescription($"{GameManager._StoreManager.AverageCustomerSpawnTime} Average Spawn Time");
        ImproveStoreUpgrade.SetDescription($"Store Level - UNFINISHED");
        StorageUpgrade.SetDescription($"{GameManager.inventoryManager.GetTotalInventoryCount()} Total Inventory");
        ShelvesUpgrade.SetDescription($"{GameManager.inventoryManager.GetTotalShelfInventoryCount()} Total Shelf Spaces");
        //FINISH REST OF DESCRIPTIONS HERE.
    }


    public void UpgradeItem(UpgradePanel panel)
    {
        if (GameManager.GetCash() < panel.Price)
            return;
        
        GameManager.DeductCash(panel.Price);
        panel.BuyUpgrade();

        switch (panel.name)
        {
            case "DeskCount" :
                GameManager._StoreManager.CounterCount++;
                
                //This is a temporary measure, will add some sort of employee selection system.
                GameManager.AddEmployee();
                break;
            case "EmployeeSkill" :
                GameManager._StoreManager.AverageCustomerTransactionTime = Math.Max(0, GameManager._StoreManager.AverageCustomerTransactionTime  - 0.5f);
                break;
            case "Advertisement":
                GameManager._StoreManager.AverageCustomerSpawnTime = Math.Max(0, GameManager._StoreManager.AverageCustomerTransactionTime  - 0.5f);
                break;
            case "ImproveStore" :
                break;
            case "Storage" :
                GameManager._StoreManager.ExtraStorageInventorySpace += 15;
                break;
            case "Shelves" :
                GameManager._StoreManager.ShelvesCount++;
                break;
        }

        SetDescriptions();

        if (panel.Level == panel.MaximumLevel)
        {
            panel.BuyButton.interactable = false;
            panel.SetMaxedOut();
        }
    }

}
