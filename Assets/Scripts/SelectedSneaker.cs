using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSneaker : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _price;
    [SerializeField] TMP_Text _quantity;
    [SerializeField] TMP_Text _rarity;
    [SerializeField] private Image sneakerImage;
    
    public void UpdateDetails(SneakerInventoryItem sneakerInventoryItem)
    {
        _name.text = sneakerInventoryItem.name;
        _price.text = $"${sneakerInventoryItem.purchasedPrice}";
        _quantity.text = $"{sneakerInventoryItem.quantity}";
        _rarity.text = Enum.GetName(sneakerInventoryItem.rarity.GetType(), sneakerInventoryItem.rarity);
        sneakerImage.sprite = sneakerInventoryItem.sneakerImage.sprite;
    }
}
