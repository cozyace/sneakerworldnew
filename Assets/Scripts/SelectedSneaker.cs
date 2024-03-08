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


    private void Start()
    {
        _name.text = "";
        _price.text = "";
        _quantity.text = "";
       // _rarity.text = "";
        sneakerImage.sprite = Resources.Load<Sprite>("Transparent");
    }
    
    public void UpdateDetails(SneakerInventoryItem sneakerInventoryItem)
    {
        _name.text = sneakerInventoryItem.name;
        _price.text = $"${sneakerInventoryItem.purchasePrice}";
        _quantity.text = $"{sneakerInventoryItem.quantity}";
        // if (_rarity.text != null) _rarity.text = Enum.GetName(sneakerInventoryItem.rarity.GetType(), sneakerInventoryItem.rarity);
        sneakerImage.sprite = sneakerInventoryItem.sneakerImage.sprite;
    }
}
