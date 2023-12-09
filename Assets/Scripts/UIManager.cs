using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [Header("Main UI")]
    [SerializeField] private Animator uiAnimator;
    public TMP_Text levelText;
    public TMP_Text cashTextMain;
    public TMP_Text gemsTextMain;
    public Image xpFillMask;
    
    [Header("Store UI")]
    public TMP_Text cashTextStore;
    public TMP_Text gemsTextStore;
    
    [Header("Panels")]
    [SerializeField] private GameObject upgradesPanel;

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

    public void UpdateUI(UserData userData)
    {
        levelText.text = $"{userData.level}";
        xpFillMask.fillAmount = (float) userData.experience / (userData.level * GameManager.instance.xpPerLevel);
        cashTextMain.text = $"{userData.cash}";
        gemsTextMain.text = $"{userData.gems}";
        cashTextStore.text = $"{userData.cash}";
        gemsTextStore.text = $"{userData.gems}";
    }

    public void ShowUpgrades()
    {
        upgradesPanel.SetActive(true);
    }
}
