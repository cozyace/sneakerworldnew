using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationItemUI : MonoBehaviour
{
    //This class is for the UI prefab that represents a decoration, in the decoration horizontal scroll panel.

    [Header("Data")]
    [Space(5)]
    public int ID;
    public string Name;
    public int Quantity;
    public Sprite Icon;
    public DecorationObject Prefab;

    [Space(10)]
    [Header("UI References")]
    [Space(5)]
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text QuantityText;
    [SerializeField] private Image IconImage;

    private DecorationManager _DecorationManager;

    private void Awake()
    {
        _DecorationManager = FindAnyObjectByType<DecorationManager>();
        
    }
    
    public void UpdateData(int id, string name, int quantity, Sprite icon, DecorationObject prefab)
    {
        ID = id;
        Name = name;
        Quantity = quantity;
        Icon = icon;
        Prefab = prefab;

        if (Quantity <= 0)
        {
            _DecorationManager.StoredDecorations.Remove(this);
            Destroy(gameObject);
        }

        NameText.text = Name;
        QuantityText.text = $"x{Quantity}";
        IconImage.sprite = Icon;
    }

    //Called when this UI is clicked/tapped on.
    public void PlaceItem()
    {
        //Updates this UI's data to change Quantity.
        UpdateData(ID, Name, Quantity-1, Icon, Prefab);
        
        //Instantiates the Prefab version of the decoration.
        _DecorationManager.CreateDecorationObject(ID);
        
    }
}
