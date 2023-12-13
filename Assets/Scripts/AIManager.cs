using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private AI botPrefab;
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    [SerializeField] private float spawnDelay;
    [SerializeField] private GameObject spawnPoint;

    private List<AI> botsInGame = new List<AI>();

    private void Start()
    {
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }
        
    private void Update()
    {
        SpawnBots(botPrefab);
    }

    private void SpawnBots(AI botToSpawn)
    {
        spawnDelay -= Time.deltaTime;
        if (!(spawnDelay <= 0)) return;
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        AI spawnedBot = Instantiate(botToSpawn, spawnPoint.transform);

        UpdateBotsInventory(spawnedBot);
    }

    private void UpdateBotsInventory(AI botToAdd)
    {
        botsInGame.Add(botToAdd);
        UpdateBotsDrawOrderInLayer(botsInGame);
    }

    private void UpdateBotsDrawOrderInLayer(List<AI> botsToSort)
    {
        foreach (var bot in botsToSort)
        {
            bot.IncreaseDrawOrderInLayer();
        }
    }
}