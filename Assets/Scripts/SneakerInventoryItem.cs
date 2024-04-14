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
    public bool IsATradeItem; //Is it a friend trade inventory item.
    public bool IsAMarketItem; //Is it a market inventory item.


    [Header("UI Elements")]
    public TMP_Text ItemNameText;
    public TMP_Text ItemRarityText;
    public TMP_Text ItemQuantityText; //Only enabled during market/friend trade.
    public Image ItemIconImage;
    public Toggle AvailabilityToggle;
    public GameObject MarketInventorySelectButton;
    
    public Image RarityPanel;
    public Sprite[] RarityPanelSprites;

    private void Start()
    {
        //We're invoking this because it will cause an error if done exactly when it's created, as when it's used in the friends inv/market inv, it doesn't get data assigned automatically.
        Invoke("UpdateRarity", 0.35f);
    }

    private void UpdateRarity()
    {
        if (IsATradeItem || IsAMarketItem)
            return;
        
        ItemRarityText.text = rarity.ToString();
        RarityPanel.sprite = RarityPanelSprites[(int)rarity-1];
    }

    public void Initialize(bool isTrade, bool isMarket, string name)
    {
        if (GameManager == null)
            GameManager = FindAnyObjectByType<GameManager>();

        IsATradeItem = isTrade;
        IsAMarketItem = isMarket;
        this.name = name;

        //Market Button should be disabled by default.
        if (IsATradeItem)
        {
            MarketInventorySelectButton.SetActive(true);
            MarketInventorySelectButton.GetComponent<Button>().onClick.AddListener(() => GameManager.inventoryManager.SelectFriendTradeSneaker(this));
        }
        else if (IsAMarketItem)
        {
            MarketInventorySelectButton.SetActive(true);
            MarketInventorySelectButton.GetComponent<Button>().onClick.AddListener(() => GameManager._MarketManager.SelectItemToList(GameManager.inventoryManager.SneakersOwned.Find(x => x.name == name)));
        }
    }

    public void Update()
    {
        if (quantity <= 0)
        {
            AvailabilityToggle.isOn = false;
            AvailabilityToggle.interactable = false;
            
            if (GameManager.inventoryManager.EnabledItems.Contains(name))
                GameManager.inventoryManager.EnabledItems.Remove(name);
        }
    }
    
    public void ToggleSneakerCheckbox(bool isActive)
    {
        if (GameManager == null)
            GameManager = FindAnyObjectByType<GameManager>();
        
        if (quantity <= 0) 
            return;
        
        if (!isActive)
            GameManager.inventoryManager.EnabledItems.Remove(name);
        else if (isActive)
            GameManager.inventoryManager.EnabledItems.Add(name);

        CanAIBuy = isActive;
    }
}
