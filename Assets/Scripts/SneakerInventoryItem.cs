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
    public string Name;
    public int PurchasePrice;
    public int Quantity;
    public SneakerRarity Rarity;
    public DateTime TimeStamp;
    
    private GameManager _GameManager;
    
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
        
        ItemRarityText.text = Rarity.ToString();
        RarityPanel.sprite = RarityPanelSprites[(int)Rarity-1];
    }

    public void Initialize(bool isTrade, bool isMarket, string shoeName)
    {
        if (_GameManager == null)
            _GameManager = FindAnyObjectByType<GameManager>();

        IsATradeItem = isTrade;
        IsAMarketItem = isMarket;
        Name = shoeName;
        

        //Market Button should be disabled by default.
        if (IsATradeItem)
        {
            MarketInventorySelectButton.SetActive(true);
            MarketInventorySelectButton.GetComponent<Button>().onClick.AddListener(() => _GameManager.inventoryManager.SelectFriendTradeSneaker(this));
        }
        else if (IsAMarketItem)
        {
            MarketInventorySelectButton.SetActive(true);
            MarketInventorySelectButton.GetComponent<Button>().onClick.AddListener(() => _GameManager._MarketManager.SelectItemToList(_GameManager.inventoryManager.SneakersOwned.Find(x => x.name == shoeName)));
        }
    }

    public void Update()
    {
        if (Quantity <= 0)
        {
            AvailabilityToggle.isOn = false;
            AvailabilityToggle.interactable = false;
            
            if (_GameManager.inventoryManager.EnabledItems.Contains(Name))
                _GameManager.inventoryManager.EnabledItems.Remove(Name);
        }
    }
    
    public void ToggleSneakerCheckbox(bool isActive)
    {
        if (_GameManager == null)
            _GameManager = FindAnyObjectByType<GameManager>();

        if (_GameManager.inventoryManager.EnabledItems.Count == _GameManager.inventoryManager.GetTotalShelfInventoryCount())
        {
            if(isActive) AvailabilityToggle.isOn = false;
            return;
        }
        
        if (Quantity <= 0) //This shouldn't happen ever.
            return;
        
        if (!isActive)
            _GameManager.inventoryManager.EnabledItems.Remove(Name);
        else if (isActive)
            _GameManager.inventoryManager.EnabledItems.Add(Name);

        CanAIBuy = isActive;
        
        _GameManager.inventoryManager.UpdateTotalShoeCount();
    }
}
