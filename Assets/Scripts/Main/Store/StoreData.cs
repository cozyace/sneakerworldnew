// System.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    // Stores data from the personal store.
    [System.Serializable]
    public class StoreData : InventoryData {
        
        public int crateRerolls = 0;
        public int sneakerRerolls = 0;

    }

    // Stores data from the featured store.
    [System.Serializable]
    public class FeaturedStoreData : InventoryData {
        
    }

}
