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

    public bool IsLocalStore = true; //This will be used for checking if this store is the user's, or someone else's.
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
    
    //The public property version of this variable.
    public int StoreLevel 
    {
        get
        {
            return _StoreLevel;
        }
        set
        {
            _StoreLevel = value; 
            InitializeActiveStore(); //Creates the store prefab, and adds all the upgrades.
            _UpgradesManager.UpdateMaximumValues(); //Updates the maximum potential values for upgrades (they're bound by the store level you are)
        }
    }
    
    //How many sale counters are available for employees to be at.
    [SerializeField] [Range(0,3)] private int _CounterCount; //Counter Upgrade
    //How many employees you have working at a counter.
    [SerializeField] [Range(0,3)] private int _EmployeeCount; //TEMP DEMO Upgrade (used until employee menu is in)
    //How many shelves you have (increases amount of items to sell at once)
    [SerializeField] [Range(0,27)] private int _ShelfCount; //Shelves Upgrade
    //The level your store is (decides the layout)
    [SerializeField][Range(0, 1)] private int _StoreLevel; //Store Upgrade


    
    [Header("Non-Property Values")]
    //The average time that it takes for a customer to come into the store, (+- 3 seconds)
    
    [Range(0.5f,10)]public float AverageCustomerTransactionTime = 5f; //Employee speed upgrade. (Will be used for the employee menu later)
    [Range(1,10)] public float AverageCustomerSpawnTime = 1f; //Advertisement Upgrade
    private const float SpawnTimeDiscrepancy = 1.5f; //How much + or - the range of time can be, between customers. (can be changed with employee menu later)
    
    [Range(0,100)] public int ExtraStorageInventorySpace = 0; //Storage upgrade
   

    
    [Space(10)]
    [Header("Store")]
    public StorePrefab ActiveStore;

    [Space(10)]
    [Header("Store Prefabs")]
    public List<StorePrefab> StorePrefabs = new List<StorePrefab>();
  
    

    // Start is called before the first frame update
    private void Start()
    {
        if (!GameManager)
            return;

        _EmployeeManager = GameManager.employeeManager;
        _CustomerQueue = GameManager._CustomerQueue;
        _AIManager = GameManager.aiManager;
        _UpgradesManager = GameManager.upgradesManager;
        
        InitializeActiveStore();
    }

    private void InitializeActiveStore()
    {
        if (ActiveStore != null)//(Might need to make sure no AI are in store currently, so waypoints don't get messed up when regenerated to upgrade)
            Destroy(ActiveStore.gameObject);
        
        //MAKE SURE THE PLAYER'S DATA IS LOADED IN HERE, BEFORE ANY OF THIS.
        
        //Check user's data here, and assign proper values.
        ActiveStore = Instantiate(StorePrefabs[_StoreLevel]);
        ActiveStore.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        
        //Customer Purchase Positions assigned in EmployeeManager.cs  SetActiveSaleCountersCount()  (when counters are created)
        //Employee Spawn Positions assigned in EmployeeManager.cs  SetActiveSaleCountersCount()   (When counters are created)

        //CUSTOMER QUEUE POSITIONS
        foreach (Transform t in ActiveStore.WaitingQueuePositions)
            _CustomerQueue.QueuePositions.Add(t);

        //AWAY FROM DESK WAYPOINT
        _CustomerQueue.LeaveCounterWaypoint = ActiveStore.AwayFromDeskWaypoint;
        
        //SPAWN POSITION
        _AIManager.SetSpawnPosition(ActiveStore.EnterExitWaypoint.gameObject);
        
        //EXIT POSITION ASSIGNED IN AI.cs -- Start()

        Camera.main.GetComponent<CameraController>().XMovementClamps = ActiveStore.CameraXMovementClamps;
        Camera.main.GetComponent<CameraController>().YMovementClamps = ActiveStore.CameraYMovementClamps;
        
        //Give the game a moment to load in the player's data.
        Invoke(nameof(InitializeUpgrades), 0.1f);
        
        Invoke(nameof(InitializeDecorations), 0.15f);
    }

    //Loads in all the 'Upgrades' for the store.
    public void InitializeUpgrades()
    {
        _EmployeeManager.SetActiveSaleCountersCount(CounterCount);
        _EmployeeManager.SetActiveEmployeeCount(EmployeeCount);
        _EmployeeManager.SetActiveSaleShelvesCount(ShelvesCount);
        _AIManager.UpdateSpawnDelay(Math.Clamp(AverageCustomerSpawnTime - SpawnTimeDiscrepancy, 0.25f,100), Math.Clamp(AverageCustomerSpawnTime + SpawnTimeDiscrepancy, 3f,100));
    }

    //Loads all the player's placed decorations from the database, and puts them on the grid.
    public void InitializeDecorations()
    {
        
    }




    
    
}
