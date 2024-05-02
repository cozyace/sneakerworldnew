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
    [SerializeField] private TMP_Text _multiplier;
    [SerializeField] TMP_Text _rarity;
    [SerializeField] private Image sneakerImage;

    private string _Name;
    

    private void Start()
    {
        ResetElements();
    }

    public string GetName()
    {
        return _Name;
    }
    
    public void UpdateDetails(SneakerInventoryItem sneakerInventoryItem)
    {
        _Name = sneakerInventoryItem.Name;
        _name.text = sneakerInventoryItem.Name;
        _price.text = $"${sneakerInventoryItem.PurchasePrice}";
        _quantity.text = $"{sneakerInventoryItem.Quantity}";
        _multiplier.text = (0.5f * (int)sneakerInventoryItem.Rarity) + "x";
        // if (_rarity.text != null) _rarity.text = Enum.GetName(sneakerInventoryItem.rarity.GetType(), sneakerInventoryItem.rarity);
        sneakerImage.sprite = sneakerInventoryItem.ItemIconImage.sprite;
    }

    public void ResetElements()
    {
        _name.text = "";
        _price.text = "";
        _quantity.text = "";
        sneakerImage.sprite = Resources.Load<Sprite>("Transparent");
    }
}
