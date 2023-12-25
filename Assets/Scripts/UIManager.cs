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

    public void UpdateUI(UserData userData)
    {
        username.text = userData.username;
        levelText.text = $"{userData.level}";
        xpFillMask.fillAmount = (float) userData.experience / (userData.level * GameManager.instance.xpPerLevel);
        cashTextMain.text = FormattedCash(userData.cash);
        gemsTextMain.text = $"{userData.gems}";
        cashTextStore.text = FormattedCash(userData.cash);
        gemsTextStore.text = $"{userData.gems}";
        welcomeText.text = $"Welcome, {userData.username}";
    }

    public void ShowUpgrades()
    {
        upgradesPanel.SetActive(true);
    }
}
