using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    [Header("Employee Upgrades")]
    [SerializeField] private UpgradePanel employees;
    public float waitTime;
    [SerializeField] private UpgradePanel advertisement;
    [SerializeField] private UpgradePanel improveStore;

    private void Start()
    {
        employees.SetInitialValues($"Waiting time ~ {waitTime}s");
    }

    public void UpgradeEmployees()
    {
        if (GameManager.instance.GetCash() >= employees.price)
        {
            GameManager.instance.DeductCash(employees.price);
            employees.BuyUpgrade();
            waitTime = Math.Max(0, waitTime - 0.5f);
            employees.SetDescription($"Waiting time ~ {waitTime}s");
            if (employees.level % 5 == 0)
            {
                GameManager.instance.AddEmployee();
            }

            if (employees.level == 20)
            {
                employees.buyButton.interactable = false;
                employees.SetMaxedOut();
            }
        }
    }
}
