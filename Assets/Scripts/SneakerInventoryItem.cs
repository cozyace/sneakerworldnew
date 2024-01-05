using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SneakerInventoryItem : MonoBehaviour
{
    new public string name;
    public int purchasedPrice;
    public int quantity;
    public SneakerRarity rarity;
    public Image sneakerImage;
    public DateTime timestamp;
    public Toggle toggle;
    public bool listed;
    public bool aiCanBuy;
    public GameManager gameManager;
    public bool isSwapItem;

    public void OnClick()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (isSwapItem) 
            gameManager.inventoryManager.OnSneakerSwapClick(this);
        else 
            gameManager.inventoryManager.OnSneakerClick(this);
    }
}
