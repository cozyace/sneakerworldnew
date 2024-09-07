// System.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    using ShopData = StoreStateData;

    public class Shop : PlayerSystem {

        // [System.Serializable]
        // public class ShopData {
        //     public int level = 1;
        //     // public float transactionInterval = 0f; 
        // }

        [System.Serializable]
        public class CustomerData {
            public string name = "Bob";
            public float transactionTicks = 1f;
            public float timeToQueue = 1f;
            public const float movespeedPerUnit = 5f;

            public CustomerData(float ticks, float dist) {
                transactionTicks = ticks;
                timeToQueue = dist / movespeedPerUnit;
            }
        }

        public List<CustomerData> customers = new List<CustomerData>();
        public float customerTicks = 0f; 

        public float markup = 1f; // the acceptable mark up based on shoplevel.
        public float transactionInterval = 1f;
        public float customerInterval = 1f;
        public int maxCustomers = 5;

        public UnityEvent<string> onSaleFailedEvent = new UnityEvent<string>();

        // Whether this is initialized or not.
        private bool initialized = false;

        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await GetShop();
            InventoryData inventory = await Player.instance.inventory.GetInventoryData();
            Player.instance.inventory.onInventoryChanged.Invoke(inventory);
            initialized = true;
        }

        public async Task<ShopData> GetShop() {
            ShopData shop = await Player.instance.inventory.state.GetState();
            if (shop == null) {
                shop = new ShopData();
            }
            
            markup = MarkupCalculator(shop.level);
            transactionInterval = TransactionIntervalCalculator(shop.level);
            customerInterval = CustomerIntervalCalculator(shop.level);
            maxCustomers = shop.level;

            // await SetShop(shop);
            return shop;
        }

        public async Task SetShop(ShopData shop) {
            // await FirebaseManager.SetDatabaseValue<ShopData>(FirebasePath.MyShop(), shop);
            // onShopUpdated.Invoke(shop);
        }

        void FixedUpdate() {
            if (!initialized) { return; }

            UpdateCustomers(Time.fixedDeltaTime);
        }

        async void UpdateCustomers(float dt) {
            if (customers == null) {
                customers = new List<CustomerData>();
            }
            customers = customers.FindAll(customer => customer != null && customer.transactionTicks > 0f );

            foreach (CustomerData customer in customers) {
                customer.timeToQueue -= CustomerData.movespeedPerUnit * dt;
                customer.timeToQueue = Mathf.Max(0f, customer.timeToQueue);
            }
            
            if (customers.Count > 0 && customers[0].timeToQueue <= 0f) {
                customers[0].transactionTicks -= dt;
                if (customers[0].transactionTicks <= 0f) {
                    for (int i = 1; i < customers.Count; i++) {
                        customers[i].timeToQueue += 1f;
                    }
                    FinalizeTransaction();
                    customers[0] = null;
                }
            }

            if (customers.Count <= maxCustomers) {
                customerTicks += dt;
                if (customerTicks > customerInterval) {
                    CustomerData customer = new CustomerData(transactionInterval, (float)(5 + maxCustomers-customers.Count));
                    customers.Add(customer);
                    customerTicks -= customerInterval;
                }
            }

        }

        public async Task<bool> FinalizeTransaction() {

            try {
                await GetShop();

                InventoryData inventory = await Player.instance.inventory.GetInventoryData();
                List<InventoryItem> saleItems = inventory.sneakers.FindAll(item => item.onSale);
                if (saleItems.Count <= 0) {
                    throw new Exception("No items on sale");
                }

                InventoryItem chosenItem = saleItems[UnityEngine.Random.Range(0, saleItems.Count)];
                SneakerData sneakerData = SneakerData.ParseId(chosenItem.itemId);

                if (chosenItem.quantity <= 0) {
                    throw new Exception($"Out of stock for one of {sneakerData.name}");
                }
                // if (chosenItem.markup > UnityEngine.Random.Range(markupCap / 3f, markupCap)) {
                //     throw new Exception($"Mark up for {sneakerData.name} was too high.");
                // }

                int credit = (int)Mathf.Ceil(sneakerData.price * (1f + markup));
                await Player.instance.wallet.Credit(credit);

                int xp = ((int)sneakerData.rarity+1) * 100;
                await Player.instance.status.AddExperience(xp);
                
                // chosenItem.quantity -= 1;
                await Player.instance.inventory.RemoveItemByID(chosenItem.itemId, 1);

                Debug.Log($"Successfully sold {sneakerData.name}");
                // onRerollEvent.Invoke(rerollPrice, rerollPrice+pricePerReroll);
                return true;
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onSaleFailedEvent.Invoke(exception.Message);
                return false;
            }
            
        }

        public float CustomerIntervalCalculator(int level) {
            float customerInterval = 12f - 0.5f * (float)(level-1);
            customerInterval = Mathf.Max(3f, customerInterval);
            return customerInterval;
        }

        public float TransactionIntervalCalculator(int level) {
            float transactionInterval = 2f - 0.01f * (float)(level-1);
            transactionInterval = Mathf.Max(0.2f, transactionInterval);
            return transactionInterval;
        }

        public float MarkupCalculator(int level) {
            float reputation = 0.1f + 0.05f * (float)(level-1);
            return reputation;
        }

    }

}