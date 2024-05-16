using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoreManager : MonoBehaviour
{
    /* This class is responsible for handling all the managerial-related tasked in relation to the store.
     * It should be able to control, employees, decorations, customers, etc.
     */
    

    [Header("Game Manager Ref")]
    [Space(5)]
    [SerializeField] private GameManager GameManager;
    
    private EmployeeManager _EmployeeManager;
    private CustomerQueue _CustomerQueue;
    private AIManager _AIManager;
    private UpgradesManager _UpgradesManager;
    
    
    [Header("Store Variables")]
    
    //The public property version of this variable.
    public int EmployeeCount 
    {
        get
        {
            return _EmployeeCount;
        }
        set
        {
            if (CounterCount >= EmployeeCount)
            {
                _EmployeeCount = value;
                _EmployeeManager.SetActiveEmployeeCount(EmployeeCount);
            }
            else
            {
                print("Can't add that many employees, not enough counters!");
            }
        }
    }
    
    //The public property version of this variable.
    public int CounterCount 
    {
        get
        {
            return _CounterCount;
        }
        set
        {
            _CounterCount = value; 
            _EmployeeManager.SetActiveSaleCountersCount(CounterCount);
        }
    }
    
    //The public property version of this variable.
    public int ShelvesCount 
    {
        get
        {
            return _ShelfCount;
        }
        set
        {
            _ShelfCount = value; 
            _EmployeeManager.SetActiveSaleShelvesCount(ShelvesCount);
        }
    }
    
    //How many sale counters are available for employees to be at.
    [SerializeField] [Range(0,3)] private int _CounterCount;
    //How many employees you have working at a counter.
    [SerializeField] [Range(0,3)] private int _EmployeeCount;
    //How many shelves you have (increases amount of items to sell at once)
    [SerializeField] [Range(0,27)] private int _ShelfCount;
    
    
    
    //The average time that it takes for a customer to come into the store, (+- 3 seconds)
    [Range(1,10)] public float AverageCustomerSpawnTime = 1f;
    private const float SpawnTimeDiscrepancy = 1.5f; //How much + or - the range of time can be, between customers.

    public int ExtraStorageInventorySpace = 0;
    public float AverageCustomerTransactionTime = 1f;


    // Start is called before the first frame update
    private void Start()
    {
        if (!GameManager)
            return;

        _EmployeeManager = GameManager.employeeManager;
        _CustomerQueue = GameManager._CustomerQueue;
        _AIManager = GameManager.aiManager;
        
        //Give the game a moment to load in the player's data.
        Invoke(nameof(InitializeStoreManager), 0.1f);
    }

    public void InitializeStoreManager()
    {
        _EmployeeManager.SetActiveEmployeeCount(EmployeeCount);
        _EmployeeManager.SetActiveSaleCountersCount(CounterCount);
        _EmployeeManager.SetActiveSaleShelvesCount(ShelvesCount);
        _AIManager.UpdateSpawnDelay(Math.Clamp(AverageCustomerSpawnTime - SpawnTimeDiscrepancy, 0.25f,100), Math.Clamp(AverageCustomerSpawnTime + SpawnTimeDiscrepancy, 3f,100));
    }




    
    
}
