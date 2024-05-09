using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSneaker : MonoBehaviour
{
    [SerializeField] TMP_Text _price;
    [SerializeField] TMP_Text _quantity;
    [SerializeField] private TMP_Text _multiplier;

    private string _Name;



    public string GetName() => _Name;
   
    public void UpdateDetails(SneakerInventoryItem sneakerInventoryItem)
    {
        print("UPDATING - " + sneakerInventoryItem.Name);
        _Name = sneakerInventoryItem.Name;
        _price.text = $"${sneakerInventoryItem.PurchasePrice}";
        _quantity.text = $"{sneakerInventoryItem.Quantity}";
        _multiplier.text = (0.5f * (int)sneakerInventoryItem.Rarity) + "x";
    }

    public void ResetElements()
    {
        print("Reset");
        _price.text = "";
        _quantity.text = "";
        _multiplier.text = "";
    }
}
