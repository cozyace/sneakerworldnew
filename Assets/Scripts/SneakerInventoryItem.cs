using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SneakerInventoryItem : MonoBehaviour
{
    [Header("Item Data")]
    new public string name;
    public int purchasePrice;
    public int quantity;
    public SneakerRarity rarity;
    public DateTime timestamp;
    
    
    private GameManager GameManager;
    
    public bool CanAIBuy;
    public bool IsATradeItem;


    [Header("UI Elements")]
    public TMP_Text ItemNameText;
    public TMP_Text ItemRarityText;
    public Image ItemIconImage;
    public Toggle AvailabilityToggle;
    
    public Image RarityPanel;
    public Sprite[] RarityPanelSprites;

    private void Start()
    {
        ItemRarityText.text = rarity.ToString();
        RarityPanel.sprite = RarityPanelSprites[(int)rarity-1];
    }

    public void OnClick()
    {
        if (GameManager == null)
            GameManager = FindAnyObjectByType<GameManager>();

        if (IsATradeItem) 
            GameManager.inventoryManager.OnSneakerSwapClick(this);
            
        else 
            GameManager.inventoryManager.OnSneakerClick(this);
    }

    public void ToggleSneaker()
    {
        if (GameManager == null)
            GameManager = FindAnyObjectByType<GameManager>();
        
        if (quantity <= 0) 
        {
            if (AvailabilityToggle.isOn) AvailabilityToggle.isOn = false;
            AvailabilityToggle.interactable = false;
            return;
        }

        if (CanAIBuy)
        {
            GameManager.inventoryManager.EnabledItems.Remove(name);
        }
        else if (!CanAIBuy)
        {
            GameManager.inventoryManager.EnabledItems.Add(name);
        }

        CanAIBuy = !CanAIBuy;
        AvailabilityToggle.isOn = !AvailabilityToggle.isOn;
    }
}
