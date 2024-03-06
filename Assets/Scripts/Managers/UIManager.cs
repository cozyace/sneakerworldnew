using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main UI")]
    
    [SerializeField] private Animator uiAnimator;
    
    public TMP_Text username;
    public TMP_Text levelText;
    public Image xpFillMask;


    [Header("Currency Texts")]
    [SerializeField] private TMP_Text[] CashTexts;
    [SerializeField] private TMP_Text[] GemsTexts;
    [SerializeField] private Image[] XPFillBars;

    
    [Header("Panels")]
    [SerializeField] private GameObject upgradesPanel;

    [Header("Welcome Screen")]
    public TMP_Text welcomeText;

    [Header("Game Mananager")]
    public GameManager gameManager;

    public void ToggleUI()
    {
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();

        uiAnimator.SetBool("shown", !uiAnimator.GetBool("shown"));
        gameManager.audioManager.ButtonClick();
    }

    private static string FormattedCash(float cash)
    {
        if (cash >= 1000000) return $"{(cash / 1000000f):F2}m";
        if (cash >= 1000) return $"{(cash / 1000f):F2}k";
        return cash.ToString();
    }

    public void UpdateUI(PlayerStats playerStats)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;
        
        username.text = playerStats.username;
        levelText.text = $"{playerStats.level}";
            
        UpdateExperienceBars((float)playerStats.experience / (playerStats.level * gameManager.xpPerLevel));
        UpdateCashTexts(playerStats.cash);
        UpdateGemTexts(playerStats.gems);

        // welcomeText.text = $"Welcome, {playerStats.username}";
    }

    private void UpdateExperienceBars(float fillAmount)
    {
        foreach (Image fillBar in XPFillBars)
            fillBar.fillAmount = fillAmount;
    }

    private void UpdateCashTexts(int value)
    {
        foreach (TMP_Text cashText in CashTexts)
            cashText.text = FormattedCash(value);
    }

    private void UpdateGemTexts(int value)
    {
        foreach (TMP_Text gemsText in GemsTexts)
            gemsText.text = $"{value}";
    }

    public void ShowUpgrades()
    {
        upgradesPanel.SetActive(true);
    }
}
