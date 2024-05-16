using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EmployeeManager : MonoBehaviour
{
    [SerializeField] private GameObject EmployeePrefab;

    [SerializeField] private Transform[] EmployeePositions;

    
    public List<GameObject> ActiveEmployees;

    public List<GameObject> SaleCounters;
    public List<GameObject> SaleShelves;
    
    private CustomerQueue _CustomerQueue;
    
    
    
    
    
    
    private void Awake() => _CustomerQueue = FindFirstObjectByType<CustomerQueue>();
    
    public void Start()
    {
        SetActiveEmployeeCount(1);
    }

    public void SetActiveSaleCountersCount(int count)
    {
        for (int i = 0; i < SaleCounters.Count; i++)
                SaleCounters[i].SetActive(i < count);
    }

    public void SetActiveSaleShelvesCount(int count)
    {
        for(int i=0; i<SaleShelves.Count; i++)
                SaleShelves[i].SetActive(i < count);
    }

    public int GetActiveSaleCountersCount()
    {
        for(int i=0; i<SaleCounters.Count; i++)
            if (SaleCounters[i].activeSelf == false)
                return i;

        return 0;
    }
    
    //This will be called via the upgrade class, to change the amount of active employees, should only ever go up, not down.
    public void SetActiveEmployeeCount(int count)
    {
        //Get rid of any still existing employees, if they exist.
        if(ActiveEmployees.Count > 0)
            RemoveAllEmployees();
        
        for (int i = 0; i < 3; i++)
        {
            _CustomerQueue.CounterPositions[i] = new TransactionCounterPosition(_CustomerQueue.CounterPositions[i].Position, _CustomerQueue.CounterPositions[i].ActiveCustomer, count > i);
            
            if (count <= i)
                continue;
            
            AddSingleEmployee();
        }
    }
    
    private void AddSingleEmployee()
    {
        GameObject employee = Instantiate(EmployeePrefab, EmployeePositions[ActiveEmployees.Count % EmployeePositions.Length].position, Quaternion.identity);
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