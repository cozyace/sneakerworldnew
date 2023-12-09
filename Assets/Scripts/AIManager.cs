using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject botPrefab;
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    [SerializeField] private float spawnDelay;
    [SerializeField] private GameObject spawnPoint;

    private void Start()
    {
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
        
    private void Update()
    {
        spawnDelay -= Time.deltaTime;
        if (!(spawnDelay <= 0)) return;
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Instantiate(botPrefab, spawnPoint.transform);
    }
}