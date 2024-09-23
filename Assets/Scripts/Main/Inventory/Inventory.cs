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

    public class Counter {
        public ItemType itemType;
        public int value;
        public Counter(ItemType itemType, int value) {
            this.itemType = itemType;
            this.value = value;
        }
    }

    /// <summary>
    /// Stores the inventory data retrieved from the database.
    /// </summary>
    [System.Serializable]
    public class Inventory {

        // The complete list of all items in the inventory.
        public List<Item> items = new List<Item>();

        // The upgrading parameters.
        public bool isUpgrading = false;
        public long startUpgradeTime = 0;
        public float upgradeDuration = -1f;

        // The inventory capacity.
        public int level = 1;
        public int maxSneakersOnSale => 3 * level;

        // The number of rerolls.
        public List<Counter> capacity = new List<Counter>();
        public List<Counter> rerolls = new List<Counter>();

        // The seperated list of items by item type.
        public List<Item> sneakers => items.Get(ItemType.Sneaker);
        public List<Item> crates => items.Get(ItemType.Crates);

        // Gets the item by item type.
        public List<Item> Get(ItemType itemType) {
            return items.FindAll(itemType);
        }

        // Clears the full list of items.
        public void Clear() {
            items = new List<Item>();
        }

        // Clears a specific list of item based on item type.
        public void Clear(ItemType itemType) {
            List<Item> specificItems = Get(itemType);
            foreach (Item item in specificItems) {
                items.Remove(item);
            }
        }

        // Adds an item into the full item list.
        public void Add(List<Item> addItems) {
            foreach (Item item in addItems) {
                Add(item);
            }
        }

        // Adds an item into the full item list.
        public void Add(Item addItem) {
            Item item = items.Find(item => item.isEqual(addItem));
            if (item != null) {
                item.quantity += addItem.quantity;
            }
            else if (HasCapacity(addItem.itemType)) {
                items.Add(addItem);
            }
        }

        // Removes an item from the full item list.
        public void Remove(Item removeItem) {
            Item item = items.Find(item => item.isEqual(removeItem));
            if (item != null && item.quantity > removeItem.quantity) {
                item.quantity -= removeItem.quantity;
                if (item.quantity <= 0) {
                    items.Remove(item);
                }
            }
        }

        // Finds the item from the full item list.
        public Item Find(Item findItem) {
            return items.Find(item => item.isEqual(findItem));
        }

        // Check whether there is space to add this item to the inventory.
        public bool HasCapacity(ItemType itemType) {
            return Get(itemType).Count < GetMaxCapacity(itemType);
        }

        // Get the max capacity of this item type.
        public int GetMaxCapacity(ItemType itemType) {
            GetCounterValue(capacity, itemType, 2 * level);
        }

        // Get the number of rerolls on this inventory.
        public int GetRerolls(ItemType itemType) {
            GetCounterValue(rerolls, itemType, 0);
        }

        // Get an value from a list of counters based on item type.
        public int GetCounterValue(List<Counter> counters, ItemType itemType, int defaultValue) {
            Counter counter = counters.Find(x => x.itemType == itemType);
            if (counter != null) {
                return counter.value;
            }
            counter = new Counter(itemType, defaultValue);
            rerolls.Add(counter);
            return counter.value;
        }

        // Start upgrading this inventory.
        public bool StartUpgrade(DateTime startTime, float duration) {
            isUpgrading = true;
            startUpgradeTime = startTime.ToBinary();
            upgradeDuration = duration;
        }

        // Check the upgrade on this inventory.
        public bool CheckUpgrade(DateTime now) {
            TimeSpan timeSpan = now.Subtract(DateTime.FromBinary(startUpgradeTime)); 
            if (timeSpan.TotalSeconds > upgradeDuration) {
                if (isUpgrading) {
                    level += 1;
                    foreach (Counter counter in capacity) {
                        counter.value = 2 * level;
                    }
                }
                isUpgrading = false;
                startUpgradeTime = DateTime.UtcNow.ToBinary();
                upgradeDuration = -1f;
                return true;
            }
            return false;
        }

    }

}
