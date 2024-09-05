// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Serialization;

// public class AIManager : MonoBehaviour
// {
//     [Header("Spawn References")]
//     //Info regarding assets/in-scene objects.
//     [SerializeField] private AI AIPrefab;
//     [SerializeField] private GameObject SpawnPoint;
//     [SerializeField] private GameManager GameManager;
    
//     [Header("Spawn Settings")]
//     //Info regarding the frequency of the AI spawning.
//     [SerializeField] private float MinSpawnDelay;
//     [SerializeField] private float MaxSpawnDelay;
//     [SerializeField] private float SpawnDelay;

//     //AI that haven't received their transaction yet (i.e haven't bought anything)
//     private List<AI> _UnsatisfiedAI = new List<AI>();




//     public void UpdateSpawnDelay(float min, float max)
//     {
//         MinSpawnDelay = min;
//         MaxSpawnDelay = max;
//     }
    
//     private void Start() => SpawnDelay = Random.Range(MinSpawnDelay, MaxSpawnDelay);
        
//     private void Update()
//     {
//         if (SceneManager.GetActiveScene().buildIndex == 1) 
//         {
//             SpawnBots(AIPrefab);
//         }
//     }

//     public void SetSpawnPosition(GameObject g) => SpawnPoint = g;
    
//     private bool ShouldSpawnAI()
//     {
//         //Checks if there's at-least 1 sneaker that's checked as available.
//         bool sneakersAvailable = GameManager.inventoryManager.SneakerUIObjects.Any(sneaker => sneaker.CanAIBuy);
//         //May also have to do a check ^ here, for if the quantity is above 0, not sure if that's checked in other code.

//         //Can define what the maximum AI count within the store will be, just replace the 4 with that value.
//         bool isStoreAtCapacity = _UnsatisfiedAI.Count == GameManager._CustomerQueue.GetStoreCapacityLevel();
        
//         //Only return true if sneakers ARE available, and store is NOT at capacity.
//         return sneakersAvailable && !isStoreAtCapacity;
//     }
    
//     //Called every frame to tick the spawn timer, and spawn the bots themselves.
//     private void SpawnBots(AI botToSpawn)
//     {
//         //If the various other conditions aren't met yet, return early.
//         if (!ShouldSpawnAI())
//             return;
        
//         //If the spawn delay hasn't been satisfied yet, return early.
//         SpawnDelay -= Time.deltaTime;
//         if (!(SpawnDelay <= 0)) return;
        
//         //Reset the spawn delay to a randomized amount within the specified range.
//         SpawnDelay = Random.Range(MinSpawnDelay, MaxSpawnDelay);
        
//         //Instantiate the AI itself, and store it in a local variable.
//         AI spawnedBot = Instantiate(botToSpawn, SpawnPoint.transform);
        
//         GameManager._CustomerQueue.AddCustomerToQueue(spawnedBot);
        
//         //Adds the bot to the list of existing bots, and updates its draw order layer.
//         AddBotToList(spawnedBot);
//     }
    
//     //This updates the list of bots existing in the store, that haven't yet gotten their order.
//     private void AddBotToList(AI botToAdd)
//     {
//         _UnsatisfiedAI.Add(botToAdd);
//     }


//     //Removes an AI from the store, by deleting them from the list & destroying the gameobject.
//     public void DeleteAI(AI ai) => Destroy(ai.gameObject);

//     //Called from the AI class when the AI has purchased their item.
//     public void SatisfyAI(AI ai) => _UnsatisfiedAI.Remove(ai);
// }