using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashPopup : MonoBehaviour
{
    [SerializeField] private int cash = 0;
    public TMP_Text cashAmountText;
    public GameManager gameManager;
    private bool hasAddedCash;

    private void Start()
    {
        StartCoroutine(StartCoundown());
    }

    private IEnumerator StartCoundown()
    {
        if (!hasAddedCash)
        {
            if (gameManager == null)
                gameManager = FindAnyObjectByType<GameManager>();

            gameManager.AddCash(cash);
            hasAddedCash = true;
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void SetPopup(int cashAmount)
    {
        cash = cashAmount;
        cashAmountText.text = cashAmount.ToString();
    }
}