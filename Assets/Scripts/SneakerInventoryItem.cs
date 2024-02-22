using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SneakerInventoryItem : MonoBehaviour
{
    new public string name;
    public int purchasePrice;
    public int quantity;
    public SneakerRarity rarity;
    public Image sneakerImage;
    public DateTime timestamp;
    public Toggle toggle;
    public bool listed;
    public bool aiCanBuy;
    public GameManager gameManager;
    public bool isSwapItem;

    [Header("UI Elements")]
    public TMP_Text nameText;
    public GameObject commonTag;
    public GameObject uncommonTag;
    public GameObject rareTag;
    public GameObject epicTag;
    public GameObject legendaryTag;

    private void Start()
    {
        switch (rarity)
        {
            case SneakerRarity.Common:
                commonTag.SetActive(true);
                break;
            case SneakerRarity.Uncommon:
                uncommonTag.SetActive(true);
                break;
            case SneakerRarity.Rare:
                rareTag.SetActive(true);
                break;
            case SneakerRarity.Epic:
                epicTag.SetActive(true);
                break;
            case SneakerRarity.Legendary:
                legendaryTag.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnClick()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (isSwapItem) 
            gameManager.inventoryManager.OnSneakerSwapClick(this);
            
        else 
            gameManager.inventoryManager.OnSneakerClick(this);
    }

    public void ToggleSneaker()
    {
        if (quantity <= 0) 
        {
            if (toggle.isOn) toggle.isOn = false;
            toggle.interactable = false;
            return;
        }

        if (aiCanBuy) 
        {
            aiCanBuy = false;
            toggle.isOn = false;
        }  
        else
        {
            aiCanBuy = true;
            toggle.isOn = true;
        } 
    }
}
