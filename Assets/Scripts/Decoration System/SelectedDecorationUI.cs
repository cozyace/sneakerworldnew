using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedDecorationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text NameText; //The text with the name of the decoration
    
    [SerializeField] private Button LeftMoveButton;
    [SerializeField] private Button RightMoveButton;
    [SerializeField] private Button UpMoveButton;
    [SerializeField] private Button DownMoveButton;
    [SerializeField] private Button StoreButton;

    public DecorationObject SelectedDecoration;

    private GridController _GridController;
    private GameObject _RootObject; //For enabling/disabling this UI
    
    
    private void Awake()
    {
        _GridController = FindAnyObjectByType<GridController>();
        _RootObject = transform.GetChild(0).gameObject;
    }
    
    //Called when you click on a decoration object.
    public void UpdateSelectedDecoration(DecorationObject decoration)
    {
        _RootObject.SetActive(true);
        
        //if there was a previous decoration, reset its color.
        if(SelectedDecoration)
            SelectedDecoration.SetSpriteColor(Color.white);
        
        SelectedDecoration = decoration;
        NameText.text = decoration.Name;
       
        SelectedDecoration.SetSpriteColor(new Color(1,1,1,0.7f));
        
        //Remove any pre-existing hooks.
        LeftMoveButton.onClick.RemoveAllListeners();
        RightMoveButton.onClick.RemoveAllListeners();
        UpMoveButton.onClick.RemoveAllListeners();
        DownMoveButton.onClick.RemoveAllListeners();
        StoreButton.onClick.RemoveAllListeners();
        
        //Hooks the movement buttons up to the DecorationObject 'Move' method.
        LeftMoveButton.onClick.AddListener(()=> decoration.MoveOnGrid(-1,0));
        RightMoveButton.onClick.AddListener(()=> decoration.MoveOnGrid(1,0));
        UpMoveButton.onClick.AddListener(()=> decoration.MoveOnGrid(0,2));
        DownMoveButton.onClick.AddListener(()=> decoration.MoveOnGrid(0,-2));
        
        //Hooks the 'store' button to the DecorationObject 'StoreDecoration' method.
        StoreButton.onClick.AddListener(decoration.ReturnToStorage);
    }

    public void Deselect()
    {
        _RootObject.SetActive(false);
        
        if(SelectedDecoration)
            SelectedDecoration.SetSpriteColor(Color.white);
        
        SelectedDecoration = null;
    }
}
