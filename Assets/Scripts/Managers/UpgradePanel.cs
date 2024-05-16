using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text LevelText;
    [SerializeField] private TMP_Text PriceText;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] public Button BuyButton;
    
    [Header("Values")]
    public int Level;
    public int Price;
    public int MaximumLevel;
    
    public void SetInitialValues(string startingDescription)
    {
        SetPrice(100);
        SetCurrentLevel(0);
        SetDescription(startingDescription);
    }

    public void BuyUpgrade()
    {
        Level += 1;
        LevelText.text = $"Level {Level}";
        Price = Level * 200;
        PriceText.text = $"{Price}";
    }

    private void SetPrice(int value)
    {
        PriceText.text = $"{value}";
        Price = value;
    }

    private void SetCurrentLevel(int value)
    {
        LevelText.text = $"Level {value}";
        Level = value;
    }

    public void SetDescription(string value)
    {
        DescriptionText.text = value;
    }

    public void SetMaxedOut()
    {
        PriceText.text = "MAXED OUT";
        BuyButton.interactable = false;
    }
}
