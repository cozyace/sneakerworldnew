using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    private GameObject _Grid;

    public bool IsGridActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _Grid = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        _Grid.SetActive(IsGridActive);
    }
}
