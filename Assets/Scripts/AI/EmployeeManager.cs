using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EmployeeManager : MonoBehaviour
{
    [SerializeField] private GameObject EmployeePrefab;

    [SerializeField] private Transform[] EmployeePositions;

    
    public List<GameObject> ActiveEmployees;

    private CustomerQueue _CustomerQueue;
    
    
    private void Awake() => _CustomerQueue = FindObjectOfType<CustomerQueue>();
    
    public void Start()
    {
        SetActiveEmployeeCount(1);
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
    
    public void AddSingleEmployee()
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