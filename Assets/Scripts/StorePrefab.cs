using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePrefab : MonoBehaviour
{
    public string StoreName;
    public List<Transform> ShelfSpawnPositions; //All possible upgrade positions for shelves. (count is maximum shelves for this store)
    public List<Transform> CounterSpawnPositions; //All possible upgrade positions for counters. (count is maximum counters for this store)
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
