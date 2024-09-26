// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Listens to the inventory and updates the UI accordingly. 
    /// </summary>
    public class StoreContentListener : MonoBehaviour {

        // Sneakers.
        public InventoryContentDrawer sneakerContent;
        public InventoryContentDrawer cratesContent;

        public InventoryContentDrawer featuredSneakersContent;
        public InventoryContentDrawer featuredCratesContent;

        public Inventory stock;
        public Inventory featuredStock;

        // Runs once on instantiation.
        void Awake() {
            Player.instance.store.onStoreUpdated.AddListener(DrawStore);
            Player.instance.store.onFeaturedStoreUpdated.AddListener(DrawFeaturedStore);
        }
        
        // Draw the inventory ui.
        public void DrawStore(StoreData storeData) {
            // Cache the inventory data.
            stock = storeData;
            sneakerContent.Draw(storeData.sneakers);
            cratesContent.Draw(storeData.crates);
        }

        // Draw the inventory ui.
        public void DrawFeaturedStore(FeaturedStoreData featuredStoreData) {
            // Cache the inventory data.
            featuredStock = featuredStoreData;
            featuredSneakersContent.Draw(featuredStoreData.sneakers);
            featuredCratesContent.Draw(featuredStoreData.crates);
        }

    }

}
