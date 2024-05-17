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
    
    public TMP_Text Username;

    
    [Header("Currency Texts")]
    [SerializeField] private TMP_Text CashText;
    [SerializeField] private TMP_Text GemText;
    [SerializeField] private Image XPFillBar;
    [SerializeField] private TMP_Text XPLevelText;
    

    [Header("Components")]
    [SerializeField] private GameObject UpgradesButton;
    [SerializeField] private GameObject DecorationButton;

    
    [Header("Game Mananager")]
    public GameManager GameManager;
    

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
        
        Username.text = playerStats.username;
        XPFillBar.fillAmount = (float)playerStats.experience / (playerStats.level * GameManager.xpPerLevel);
        XPLevelText.text = playerStats.level.ToString();
        CashText.text = FormattedCash(playerStats.cash);
        GemText.text = $"{playerStats.gems}";
    }

    public void Update()
    {
        UpgradesButton.SetActive(GameManager._StoreManager.IsLocalStore);
        DecorationButton.SetActive(GameManager._StoreManager.IsLocalStore);
    }
    
}
