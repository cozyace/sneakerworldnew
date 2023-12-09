using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SneakerInventoryItem : MonoBehaviour
{
    public string name;
    public int purchasedPrice;
    public int quantity;
    public SneakerRarity rarity;
    public Image sneakerImage;
    public DateTime timestamp;
    public Toggle toggle;
    public bool listed;
    public bool aiCanBuy;

    public void OnClick()
    {
        GameManager.instance.inventoryManager.OnSneakerClick(this);
    }
}
