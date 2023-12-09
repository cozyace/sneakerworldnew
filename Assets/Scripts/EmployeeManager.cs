using System;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    [SerializeField] private GameObject employeePrefab;
    [SerializeField] private Transform[] employeePositions;
    public List<GameObject> employees;

    public void AddEmployee()
    {
        GameObject employee = Instantiate(employeePrefab, employeePositions[employees.Count % employeePositions.Length].position, Quaternion.identity);
        employees.Add(employee);
    }
}