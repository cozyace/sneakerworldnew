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

    /// <summary>
    /// Stores the inventory data retrieved from the database.
    /// </summary>
    [System.Serializable]
    public class InventoryData {
        public List<InventoryItem> items = new List<InventoryItem>(); 
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class InventoryItem {
        public string itemId;
        public int quantity;
        public InventoryItem(string itemId) {
            this.itemId = itemId;
            this.quantity = 0;
        }
    }

}
