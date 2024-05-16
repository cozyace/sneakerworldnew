using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePrefab : MonoBehaviour
{
    public string StoreName;
    
    [Space(10)]
    [Header("Upgrade Spawn Positions")]
    public List<Transform> ShelfSpawnPositions; //All possible upgrade positions for shelves. (count is maximum shelves for this store)
    public List<Transform> CounterSpawnPositions; //All possible upgrade positions for counters. (count is maximum counters for this store)
    
    [Space(10)]
    [Header("Camera Tweaks")]
    //Boundaries for the Camera dependant on the size of the store.
    public Vector2 CameraXMovementClamps;
    public Vector2 CameraYMovementClamps;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
