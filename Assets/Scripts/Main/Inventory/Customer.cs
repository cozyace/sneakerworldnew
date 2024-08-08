// // System.
// using System;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;

// namespace SneakerWorld.Main {

//     public class Customer : MonoBehaviour {

//         // The message to throw up if the player does not have enough funds.
//         public const string DID_NOT_HAVE_SNEAKER_STOCKED = "Does not have the sneaker you're looking for!";

//         // The sneaker that the customer is looking to purchase.
//         public string lookingToPurchase;

//         // This is what level of mark up the customer will agree to.
//         public float markupAgreeableness;


//         // Cache a reference to the inventory and the player.
//         private Inventory inventory;
//         // private Player player => inventory.player;


//         // Create a new customer.
//         public static void New(Inventory inventory) {
//             Customer customer = null; // Instantiate(customerPrefab);
//             customer.SetInventory(inventory);
//             customer.PickRandomItemFromShelf(inventory);
//             customer.DecideMarkupAgreeability(inventory);
//         }

//         // Cache a reference to the inventory the customer is in.
//         public void SetInventory(Inventory inventory) {
//             this.inventory = inventory;
//         }

//         //
//         public void PickRandomItemFromShelf(Inventory inventory) {
//             lookingToPurchase = inventory.PickRandomItemFromShelf();
//         }

//         //
//         public void DecideMarkupAgreeability(Inventory inventory) {
//             float reputation = inventory.GetReputation();
//             markupAgreeableness = UnityEngine.Random.Range(0f, reputation); 
//         }

//         // The customer attempts to make a purchase.
//         public async Task AttemptMakePurchase() {
//             string sneakerId = lookingToPurchase;
//             try {

//                 // Check the amount of stock left of this sneaker.
//                 bool hasStock = await player.inventory.CheckHasStockForSneakerWithId(sneakerId);
//                 if (!hasStock) {
//                     throw new Exception(DID_NOT_HAVE_SNEAKER_STOCKED); 
//                 }

//                 // Get the sneaker data.
//                 int wholesalePrice = (await SneakerData.GetSneakerById(sneakerId)).price;
//                 int retailPrice = await player.inventory.GetSellingPriceForSneakerWithId(sneakerId);

//                 // Check if the mark up is considered reasonable.
//                 float markup = (float)(retailPrice - wholesalePrice) / (float)wholesalePrice;
//                 if (markup < markupAgreeableness) {
//                     await player.wallet.Credit(retailPrice);
//                     await player.inventory.RemoveSneakerByID(sneakerId);
//                 }

//             }
//             catch (Exception exception) {
//                 Debug.LogError(exception.Message);
//             }

//             LeaveInventory();
        
//         }

//         public void LeaveInventory() {
//             //
//         }

//     }

// }
