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
    

    public Transform ShelvesParent;
    public Transform SaleCountersParent;

    [Space(5)]
    [Header("Waypoints")]
    public List<Transform> WaitingQueuePositions; //The waypoints for the customers to wait in line.
    public Transform EnterExitWaypoint; //The waypoint used for entering/exiting the store.
    public Transform AwayFromDeskWaypoint; //The waypoint used to walk away from the counters.
    
    
    [Space(10)]
    [Header("Camera Tweaks")]
    //Boundaries for the Camera dependant on the size of the store.
    public Vector2 CameraXMovementClamps;
    public Vector2 CameraYMovementClamps;
    
    
}
