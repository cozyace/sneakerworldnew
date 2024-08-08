// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.UI {

    using Vendor = Tests.TestVendor;
    using ItemData = SneakerWorld.Main.ItemData;

    using CrateData = Main.CrateData;
    using SneakerData = Main.SneakerData;


    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class StoreUI : MonoBehaviour {

        [System.Serializable]
        public class ContentSections {
            public GameObject itemSlotPrefab;
            public GameObject featuredItemSlotPrefab;
            public RectTransform regularContentSection;
            public RectTransform featuredContentSection;
        }

        // A reference to the inventory.
        public Vendor vendor;

        public ContentSections cratesContent;
        public ContentSections sneakersContent;

        // Runs once before the first frame.
        void Awake() {
            
            // Listen for changes in crates.
            vendor.onFeaturedCratesUpdated.AddListener(DrawFeaturedContent<CrateData>);
            vendor.onCratesUpdated.AddListener(DrawRegularContent<CrateData>);

            // Listen for changes in sneakers.
            vendor.onFeaturedSneakersUpdated.AddListener(DrawFeaturedContent<SneakerData>);
            vendor.onSneakersUpdated.AddListener(DrawRegularContent<SneakerData>);
            
        }


        // Draw the regular content.
        public void DrawRegularContent<TItemData>(List<TItemData> items) 
            where TItemData : ItemData {
            if (!Application.isPlaying) { return; }

            ContentSections section = GetContentSection<TItemData>();
            DrawContent(items, section.itemSlotPrefab, section.regularContentSection);
        }

        // Draw the featured content.
        public void DrawFeaturedContent<TItemData>(List<TItemData> items) 
            where TItemData : ItemData {
            if (!Application.isPlaying) { return; }
            Debug.Log($"drawing featured items : {items.Count}");

            ContentSections section = GetContentSection<TItemData>();
            DrawContent(items, section.featuredItemSlotPrefab, section.featuredContentSection);
        }

        public void DrawContent<TItemData>(List<TItemData> items, GameObject prefab, RectTransform contentSection) 
            where TItemData : ItemData {
            if (!Application.isPlaying) { return; }

            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < items.Count; i++) {
                DrawItemSlot<TItemData>(prefab, items[i], contentSection);
            }
        }

        public void DrawItemSlot<TItemData>(GameObject prefab, TItemData item, RectTransform contentSection)
            where TItemData : ItemData {
            ItemSlotUI newItemSlot = Instantiate(prefab, contentSection.transform).GetComponent<ItemSlotUI>();
            newItemSlot.Draw<TItemData>(item);
        }

        // Get the appropriate content section for the data being given.
        public ContentSections GetContentSection<TItemData>() 
            where TItemData : ItemData {
            
            if (typeof(TItemData) == typeof(CrateData)) {
                return cratesContent;
            }
            else if (typeof(TItemData) == typeof(SneakerData)) {
                return sneakersContent;
            } 
            return null;

        }

    }

}
