// // System.
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;

// namespace SneakerWorld.Main {

//     /// <summary>
//     /// A place for customers to come and buy items in the store.
//     /// </summary>
//     public class CustomerQueue : MonoBehaviour {

//         public List<Shelf> shelves = new List<Shelf>();

//         public float customerInterval;

//         [SerializeField]
//         private float customerTimer = 0f;

//         public Player player;
        
//         // Once a customer enters the shop, it should have to wait at a sales counter before it can make an attempt to purchase.
//         // A sales counter should handle that queue, and process the transaction when the customer is next in the queue.
//         // A customer should also have a timer for waiting agreeability, if they have to wait longer than this number,
//         // They get angry and leave.
//         // public List<SalesCounter> salesCounters = new List<SalesCounter>();

//         // If a customer leaves unsatisfied (e.g. has too long of a wait time, or the mark up is too high)
//         // Then the customer leaves a bad review and the customer interval time goes up (less customer frequency)
//         // Spending on "marketing" for your store can lower customer interval time
//         // Increasing store size and adding decorations and amenities can also lower customer interval time
//         // Attempting to appease the customer can also lower customer interval time. 
//         // (i.e. there will be a board where they make the reviews, you can attempt to bribe them with shoes and if the bribe is accepted then they will take it down)
//         // If the bribe is rejected then they may post another bad review and bribing them agains get harder.
//         // For a steep cost, it may also be possible to click a button that just says "Hire someone to deal with this." which makes the review go away.

//         // Initializes the user.
//         public async Task<bool> Initialize(Player player) {
//             customerTimer = customerInterval;
//             this.player = player;
//             return await default(Task<bool>);
//         }

//         void FixedUpdate() {
//             if (player == null) { return; }
//             UpdateCustomerTimer(Time.fixedDeltaTime);
//         }

//         void UpdateCustomerTimer(float dt) {
//             customerTimer -= dt;
//             if (customerTimer <= 0f) {
//                 Customer.New(this);
//                 customerTimer = customerInterval;
//             }
//         }

//         void ProcessHypotheticalTimePassing(float dt) {

//         }

//         //
//         public string PickRandomItemFromShelf() {
//             return shelves[UnityEngine.Random.Range(0, shelves.Count - 1)].itemIds[UnityEngine.Random.Range(0, shelves.Count - 1)];
//         }

//         //
//         public float GetReputation() {
//             return 0.5f;
//         }

//     }

// }
