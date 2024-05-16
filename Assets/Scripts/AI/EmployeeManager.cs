using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EmployeeManager : MonoBehaviour
{
    [Header("Employees")]
    [Space(5)]
    [SerializeField] private GameObject EmployeePrefab;

    [SerializeField] private List<Vector2> EmployeePositions;
    public List<GameObject> ActiveEmployees;
[Space(10)]
    
    [Header("Upgrades")]
[Space(5)]
    public List<GameObject> SaleCounters;
    public List<GameObject> SaleShelves;

    //These can be reassigned depending on the store. (for styling and such).
    public GameObject SaleCounterPrefab;
    public GameObject ShelfPrefab;
    
    private CustomerQueue _CustomerQueue;
    private GameManager _GameManager;






    private void Awake()
    {
        _GameManager = FindAnyObjectByType<GameManager>();
        _CustomerQueue = FindFirstObjectByType<CustomerQueue>();
    }
    

    public void SetActiveSaleCountersCount(int count)
    {
        foreach(GameObject obj in SaleCounters)
            Destroy(obj);
        
        SaleCounters.Clear();
        _CustomerQueue.CounterPositions.Clear();
        EmployeePositions.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject saleCounter = Instantiate(SaleCounterPrefab, _GameManager._StoreManager.ActiveStore.SaleCountersParent);
            saleCounter.transform.position = _GameManager._StoreManager.ActiveStore.CounterSpawnPositions[i].position;
            SaleCounters.Add(saleCounter);
            _CustomerQueue.CounterPositions.Add(new TransactionCounterPosition(saleCounter.transform.GetChild(1).transform, null, false)); 
            EmployeePositions.Add(saleCounter.transform.GetChild(0).transform.position);
        }
    }

    public void SetActiveSaleShelvesCount(int count)
    {
        foreach(GameObject obj in SaleShelves)
            Destroy(obj);
        
        SaleShelves.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject saleShelf = Instantiate(ShelfPrefab, _GameManager._StoreManager.ActiveStore.ShelvesParent);
            saleShelf.transform.position = _GameManager._StoreManager.ActiveStore.ShelfSpawnPositions[i].position;
            SaleShelves.Add(saleShelf);
        }
    }
    
    
    
    
    //This will be called via the upgrade class, to change the amount of active employees, should only ever go up, not down.
    public void SetActiveEmployeeCount(int count)
    {
        //Get rid of any still existing employees, if they exist.
        if(ActiveEmployees.Count > 0)
            RemoveAllEmployees();
        
        if(_CustomerQueue.CounterPositions.Count == 0)
            print("No counter positions found!");
        
        for (int i = 0; i < _CustomerQueue.CounterPositions.Count; i++)
        {
            _CustomerQueue.CounterPositions[i] = new TransactionCounterPosition(_CustomerQueue.CounterPositions[i].Position, _CustomerQueue.CounterPositions[i].ActiveCustomer, count > i);
            
            if (count <= i)
                continue;
            
            AddSingleEmployee();
        }
    }
    
    private void AddSingleEmployee()
    {
        GameObject employee = Instantiate(EmployeePrefab, EmployeePositions[ActiveEmployees.Count % EmployeePositions.Count], Quaternion.identity);
        ActiveEmployees.Add(employee);
    }

    private void RemoveAllEmployees()
    {
        foreach (GameObject employee in ActiveEmployees)
        {
            Destroy(employee);
        }
        
        ActiveEmployees.Clear();
    }
}