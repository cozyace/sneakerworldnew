using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DecorationManager : MonoBehaviour
{
    private GameObject _Grid;
    private DecorationDatabase _Database;
    private GridController _GridController;
    [HideInInspector] public SelectedDecorationUI SelectedDecorationUI;
    
    public bool IsDecorating = false;

    [Space(10)]
    [Header("Player's Decorations")]
    [Space(5)]
    //All decorations that are currently in the player's store.
    public List<DecorationObject> ActiveDecorations = new List<DecorationObject>();
    
    //All the decorations the player has in their 'inventory'
    public List<DecorationItemUI> StoredDecorations = new List<DecorationItemUI>();
   
    [Space(10)]
    [Header("References")]
    [Space(5)]
    [SerializeField] private Transform UIDecorationsListParent;
    [SerializeField] private Transform DecorationObjectsParent;
    [SerializeField] private DecorationItemUI UIDecorationPrefab;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _Grid = transform.GetChild(0).gameObject;
        _Database = GetComponent<DecorationDatabase>();
        _GridController = FindAnyObjectByType<GridController>();

        //TEST
        CreateUIDecoration(0);
        CreateUIDecoration(1);
        CreateUIDecoration(0);
        
        
        //Load in all the player's decorations, into the ActiveDecorations & StoredDecorations.
    }

    private void LoadStoredDecorations()
    {
        //Grabs all of the currently UN-placed decorations from the user's data.
        //Adds them to the StoredDecorations list.
        
        //loop through each of the stored decorations
        //create each of the UI representations of the decorations.
    }

    private void LoadPlacedDecorations()
    {
        //Grabs all the currently placed decorations from the player's data.
        //Adds them to the ActiveDecorations list.
        
        //Loop through each of the placed decorations
        //Spawn their prefab in.
        //set their position
    }



    //This method is for instantiating the UI version of a decoration.
    public void CreateUIDecoration(int decorationID)
    {
        //Get player's decoration inventory (excluding the things that are already placed)
        //loop through each decoration, and combine all the ones that are duplicates.

        bool isDuplicate = false; //If this decoration's UI representation already exists.
        DecorationItemUI uiItem = null; //The UI object for the decoration, stored early in case it already exists.
        
        //Checks if a duplicate exists, to increase quantity on the UI object.
        foreach (Transform t in UIDecorationsListParent)
        {
            if (t.name != _Database.Decorations[decorationID].Name)
                continue;
            
            isDuplicate = true;
            uiItem = t.GetComponent<DecorationItemUI>();
        }

        if (isDuplicate)
        {
            uiItem.UpdateData(decorationID, _Database.Decorations[decorationID].Name, uiItem.Quantity+1, _Database.Decorations[decorationID].Icon, _Database.Decorations[decorationID].Prefab);
            return;
        }
      
        //If it isn't a duplicate, continue below.
        
        uiItem = Instantiate(UIDecorationPrefab, UIDecorationsListParent);
        uiItem.name = _Database.Decorations[decorationID].Name;
        uiItem.UpdateData(decorationID, _Database.Decorations[decorationID].Name, 1, _Database.Decorations[decorationID].Icon, _Database.Decorations[decorationID].Prefab);
        
        if(!StoredDecorations.Contains(uiItem))
            StoredDecorations.Add(uiItem);
    }

    //This method is for instantiating the actual object version of the decoration.
    public void CreateDecorationObject(int decorationID)
    {
        DecorationObject decorationObject = Instantiate(_Database.Decorations[decorationID].Prefab, DecorationObjectsParent);
        decorationObject.Name = _Database.Decorations[decorationID].Name;
        decorationObject.DecorationID = decorationID;

        GridTile placementGrid = _GridController.GetEmptyGridClosestToCameraCentre().GetComponent<GridTile>();
        placementGrid.OccupyingObject = decorationObject.gameObject;
        
        decorationObject.transform.position = placementGrid.transform.position; //Change this later, should pick a random empty grid to place in, close to the camera preferably.
        decorationObject.GridPosition = placementGrid.GridPosition;
        //Pick a random grid to place it on initially.
        
        if(!ActiveDecorations.Contains(decorationObject))
            ActiveDecorations.Add(decorationObject);
        
        SelectedDecorationUI.UpdateSelectedDecoration(decorationObject);
    }
    

    //Toggles whether the player is decorating or not.
    public void SetDecoratingActive(bool isActive)
    {
        IsDecorating = isActive;
        _Grid.SetActive(isActive);
    }
    

    
}
