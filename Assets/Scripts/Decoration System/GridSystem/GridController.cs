using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GridController : MonoBehaviour
{
    [SerializeField] private int Width;
    [SerializeField] private int Height;
    [SerializeField] private Vector2 BaseOffset; //the base offset for the grid's spawn position.
    private const float SpacingMultiplier = 2.25f;
    
    [SerializeField] private GridTile TilePrefab;

    [SerializeField] private SpriteMask SpriteMask;

    public List<GridData> GridPositions = new List<GridData>();
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        
        Invoke(nameof(AddRemainingGridsToDictionary), 1f);
    }
    private void GenerateGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GridTile spawnedTile = Instantiate(TilePrefab, new Vector3(BaseOffset.x + x * SpacingMultiplier , BaseOffset.y + y * 0.5f), Quaternion.identity);

                if (y % 2 != 0)
                {
                    spawnedTile.transform.position = new Vector3(SpacingMultiplier/2 + BaseOffset.x + x  * SpacingMultiplier , BaseOffset.y + y * 0.5f);
                    spawnedTile.UpdateColor(Color.gray);
                }
                
                spawnedTile.transform.SetParent(SpriteMask.transform);
                spawnedTile.name = $"{x};{y}";
                spawnedTile.GridPosition = new Vector2(x, y);
            }
        }
    }

    //After the unavailable grids are deleted, store all existing grids in dictionairy.
    private void AddRemainingGridsToDictionary()
    {
        foreach (Transform t in SpriteMask.transform)
        {
            int xParsed = int.Parse(t.name.Split(';')[0]);
            int yParsed = int.Parse(t.name.Split(';')[1]);
            
            GridPositions.Add(new GridData(xParsed, yParsed, t, t.GetComponent<GridTile>()));
        }
    }

    public Transform GetGridByPosition(Vector2 pos)
    {
        return GridPositions.Find(x => Math.Abs(x.X - pos.x) < 0.1f && Math.Abs(x.Y - pos.y) < 0.1f && !x.Tile.IsBlocked).Grid;
    }
    
    public Transform GetNonOccupiedGridByPosition(Vector2 pos)
    {
        return GridPositions.Find(x => Math.Abs(x.X - pos.x) < 0.1f && Math.Abs(x.Y - pos.y) < 0.1f && !x.Tile.IsBlocked && x.Tile.OccupyingObject == null).Grid;
    }
    
    public Transform GetGridByRealPosition(Vector2 pos, float tolerance)
    {
        return GridPositions.First(x => Math.Abs(x.Grid.position.x - pos.x) < tolerance && Math.Abs(x.Grid.position.y - pos.y) < tolerance  && !x.Tile.IsBlocked && x.Tile.OccupyingObject == null).Grid;
    }

    public Transform GetEmptyGridClosestToCameraCentre()
    {
        return GetGridByRealPosition(Camera.main.transform.position, 1f);
    }
    
    //Make a method for getting a random grid, but closest to the centre of the camera.

}

[Serializable]
public struct GridData
{
    public int X;
    public int Y;

    public Transform Grid;
    public GridTile Tile;

    public GridData(int x, int y, Transform grid, GridTile tile)
    {
        X = x;
        Y = y;
        Grid = grid;
        Tile = tile;
    }
}
