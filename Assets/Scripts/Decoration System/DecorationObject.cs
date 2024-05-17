using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DecorationObject : MonoBehaviour
{
    //This is the class that'll be on the actual instantiated prefabs of the decorations.
    public string Name;
    public int DecorationID;
    public Vector3 GridPosition;
    
    
    private DecorationManager _DecorationManager;
    private GridController _GridController;
    private SpriteRenderer _Sprite;
    

    private void Awake()
    {
        _Sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _DecorationManager = FindAnyObjectByType<DecorationManager>();
        _GridController = FindAnyObjectByType<GridController>();
    }
    
    //On click/tap on decoration.
    private void OnMouseUpAsButton()
    {
        //Makes sure that you don't have any other menu open that could conflict with clicking on the front desk.
        if (EventSystem.current.IsPointerOverGameObject()) // <-- Also do a check here for if you're in the decoration mode or not. && that you're the owner of this store you're in.
            return;

        //If you're not currently decorating.
        if (!_DecorationManager.IsDecorating)
            return;
        
        _DecorationManager.SelectedDecorationUI.UpdateSelectedDecoration(this);
        //Can use this space for special on-tap effects for decorations as well.

        print("Clicked on decoration, open options for moving/deletion !");
       
    }

    //Moves the decoration on the grid.
    public void MoveOnGrid(int x, int y)
    {
        GridTile previousGrid = _GridController.GetGridByPosition(new Vector2(GridPosition.x, GridPosition.y)).transform.GetComponent<GridTile>();
        GridTile nextGrid = null;
        
        try
        {
            nextGrid = _GridController.GetNonOccupiedGridByPosition(new Vector2(GridPosition.x + x, GridPosition.y + y)).transform.GetComponent<GridTile>();
        } catch
        {
            Debug.LogWarning("No Available Grid to Move To!");
            return;
        }
        
        //Get the current tile, and remove the 'Occupying Object'
        previousGrid.OccupyingObject = null;
        nextGrid.OccupyingObject = gameObject;
        //Move the decoration to the new tile.
        transform.position = nextGrid.transform.position;
        
        GridPosition = new Vector2(GridPosition.x + x, GridPosition.y + y);
    }

    //returns this object to the player's decoration storage.
    public void ReturnToStorage()
    {
        //Recreate the UI version of this object, with the ID.
        _DecorationManager.CreateUIDecoration(DecorationID);
        //Remove from list of spawned decorations.
        _DecorationManager.ActiveDecorations.Remove(this);
        _DecorationManager.SelectedDecorationUI.Deselect();
        Destroy(gameObject);
        
    }

    public void SetSpriteColor(Color color)
    {
        _Sprite.color = color;
    }
}
