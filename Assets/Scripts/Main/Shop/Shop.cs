// // System.
// using System;
// using System.Linq;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;
// using UnityEngine.Events;
// // Sirenix.
// using Sirenix.OdinInspector;

// namespace SneakerWorld.Main {

//     public class Shop : PlayerSystem {

//         // [System.Serializable]
//         // public class ShopData {
//         //     public int level = 1;
//         //     // public float transactionInterval = 0f; 
//         // }

        

//         public List<CustomerData> customers = new List<CustomerData>();
//         public float customerTicks = 0f; 

//         public float markup = 1f; // the acceptable mark up based on shoplevel.
//         public float transactionInterval = 1f;
//         public float customerInterval = 1f;
//         public int maxCustomers = 5;

//         public UnityEvent<string> onSaleFailedEvent = new UnityEvent<string>();

//         // Whether this is initialized or not.
//         private bool initialized = false;

//         // Implement the initialization from the player.
//         protected override async Task TryInitialize() {
//             await GetShop();
//             InventoryData inventory = await Player.instance.inventory.GetInventoryData();
//             Player.instance.inventory.onInventoryChanged.Invoke(inventory);
//             initialized = true;
//         }

//         public async Task GetShop(InventorySytem) {
//             ShopData shop = await Player.instance.inventory.state.GetState();
//             if (shop == null) {
//                 shop = new ShopData();
//             }
            
            

//             // await SetShop(shop);
//             return shop;
//         }

//         public async Task SetShop(ShopData shop) {
//             // await FirebaseManager.SetDatabaseValue<ShopData>(FirebasePath.MyShop(), shop);
//             // onShopUpdated.Invoke(shop);
//         }

        
//     }

// }