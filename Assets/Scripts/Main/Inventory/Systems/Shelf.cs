// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

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

    /// <summary>
    /// All the functionality for the inventory.
    /// </summary>
    public class Shelf : InventorySystem {

        public List<CustomerData> customers = new List<CustomerData>();
        public float customerTicks = 0f; 

        public float markup = 1f; // the acceptable mark up based on shoplevel.
        public float transactionInterval = 1f;
        public float customerInterval = 1f;
        public int maxCustomers = 5;

        public UnityEvent<string> onSaleFailedEvent = new UnityEvent<string>();


        // Triggers an event whenever the inventory changes.
        public UnityEvent<InventoryData> onShelfUpdated = new UnityEvent<InventoryData>();
        
        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            await state.Init();
            InventoryData inventory = await Get();
            onShelfUpdated.Invoke(inventory);
        }

        [Button]
        public async void Delete() {
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, new InventoryData());
            Debug.Log("Deleted inventory");
        }

        public override async Task<Inventory> Get() {
            InventoryData inventory = await FirebaseManager.GetDatabaseValue<InventoryData>(FirebasePath.Inventory);
            Debug.Log($"Managed to find inventory: {inventory!=null}");
            
            if (inventory == null) {
                inventory = new InventoryData();
                await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, inventory);
            }

            markup = MarkupCalculator(inventory.level);
            transactionInterval = TransactionIntervalCalculator(inventory.level);
            customerInterval = CustomerIntervalCalculator(inventory.level);
            maxCustomers = inventory.level;

            return inventory;
        }

        public override async Task Set(Inventory inventory) {
            await FirebaseManager.SetDatabaseValue<InventoryData>(FirebasePath.Inventory, inventory);
            onShelfUpdated.Invoke(inventory);
        }

        public async Task Add(Item item) {
            InventoryData inventory = await Get();
            inventory.Add(item);
            await Set(inventory);
        }

        public async Task Remove(Item item) {
            InventoryData inventory = await Get();
            inventory.Remove(item);
            await Set(inventory);
        }

        public async Task<Item> Get(Item item) {
            InventoryData inventory = await Get();
            Item item = inventory.Find(item);
            return item;
        }

        //
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
                Inventory inventory = await Get();
                List<Item> saleItems = inventory.items;

                if (saleItems.Count <= 0) {
                    throw new Exception("No items on sale");
                }

                Item chosenItem = saleItems[UnityEngine.Random.Range(0, saleItems.Count)];
                if (chosenItem.quantity <= 0) {
                    throw new Exception($"Out of stock for one of {sneakerData.name}");
                }

                // Credit the player cash.
                int credit = (int)Mathf.Ceil(chosenItem.price * (1f + markup));
                await Player.instance.wallet.Credit(credit);

                // Give the player xp.
                int xp = ((int)chosenItem.level+1) * 100;
                await Player.instance.status.AddExperience(xp);
                
                // Update the inventory.
                chosenItem.quantity -= 1;
                await Set();

                Debug.Log($"Successfully sold {chosenItem.name}");
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
