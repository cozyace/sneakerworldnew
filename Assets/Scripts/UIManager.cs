using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [Header("Main UI")]
    [SerializeField] private Animator uiAnimator;
    public TMP_Text username;
    public TMP_Text levelText;
    public TMP_Text cashTextMain;
    public TMP_Text gemsTextMain;
    public Image xpFillMask;
    
    [Header("Store UI")]
    public TMP_Text cashTextStore;
    public TMP_Text gemsTextStore;
    
    [Header("Panels")]
    [SerializeField] private GameObject upgradesPanel;

    [Header("Welcome Screen")]
    public TMP_Text welcomeText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    public void ToggleUI()
    {
        uiAnimator.SetBool("shown", !uiAnimator.GetBool("shown"));
        GameManager.instance.audioManager.ButtonClick();
    }

    private string FormattedCash(float cash)
    {
        if (cash >= 1000000) return $"{(cash / 1000000f):F2}m";
        if (cash >= 1000) return $"{(cash / 1000f):F2}k";
        return cash.ToString();
    }

    public void UpdateUI(PlayerStats playerStats)
    {
        username.text = playerStats.username;
        levelText.text = $"{playerStats.level}";
        if (xpFillMask != null) xpFillMask.fillAmount = (float) playerStats.experience / (playerStats.level * GameManager.instance.xpPerLevel);
        cashTextMain.text = FormattedCash(playerStats.cash);
        gemsTextMain.text = $"{playerStats.gems}";
        cashTextStore.text = FormattedCash(playerStats.cash);
        gemsTextStore.text = $"{playerStats.gems}";
        welcomeText.text = $"Welcome, {playerStats.username}";
    }

    public void ShowUpgrades()
    {
        upgradesPanel.SetActive(true);
    }
}
