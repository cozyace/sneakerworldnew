// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class StoreContentPanel : MonoBehaviour {

        // A store content section.
        [System.Serializable]
        public class ContentSections {
            public GameObject storeItemPrefab;
            public RectTransform contentSection;
        }

        // The content sections.
        public ContentSections cratesContent;
        public ContentSections sneakersContent;
        public ContentSections featuredCratesContent;
        public ContentSections featuredSneakersContent;

        // The most upto date data.
        private List<CrateData> featuredCrates = new List<CrateData>();
        private List<CrateData> crates = new List<CrateData>();
        private List<SneakerData> featuredSneakers = new List<SneakerData>();
        private List<SneakerData> sneakers = new List<SneakerData>();


        // Runs once on instantiation.
        void Awake() {
            // Listen for changes in crates.
            Player.instance.store.onFeaturedCratesUpdated.AddListener(DrawFeaturedCrates);
            Player.instance.store.onCratesUpdated.AddListener(DrawRegularCrates);
            // Listen for changes in sneakers.
            Player.instance.store.onFeaturedSneakersUpdated.AddListener(DrawFeaturedSneakers);
            Player.instance.store.onSneakersUpdated.AddListener(DrawRegularSneakers);
        }

        // Draw the regular content.
        public void DrawRegularCrates(List<CrateData> crates) {
            if (!Application.isPlaying) { return; }
            this.crates = crates;
            DrawStoreContent<CrateData>(crates, cratesContent.storeItemPrefab, cratesContent.contentSection);
        }

        // Draw the regular content.
        public void DrawRegularSneakers(List<SneakerData> sneakers) {
            if (!Application.isPlaying) { return; }
            this.sneakers = sneakers;
            DrawStoreContent<SneakerData>(sneakers, sneakersContent.storeItemPrefab, sneakersContent.contentSection);
        }

        // Draw the featured content.
        public void DrawFeaturedCrates(List<CrateData> crates) {
            if (!Application.isPlaying) { return; }
            featuredCrates = crates;
            DrawStoreContent<CrateData>(crates, featuredCratesContent.storeItemPrefab, featuredCratesContent.contentSection);
        }

         // Draw the featured content.
        public void DrawFeaturedSneakers(List<SneakerData> sneakers) {
            if (!Application.isPlaying) { return; }
            featuredSneakers = sneakers;
            DrawStoreContent<SneakerData>(sneakers, featuredSneakersContent.storeItemPrefab, featuredSneakersContent.contentSection);
        }

        // Draw generic store content.
        public void DrawStoreContent<TItemData>(List<TItemData> items, GameObject prefab, RectTransform contentSection) 
            where TItemData : ItemData {
            if (!Application.isPlaying) { return; }

            bool hasChildren = contentSection.transform.childCount > 0;
            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < items.Count; i++) {
                DrawStoreItem<TItemData>(prefab, items[i], contentSection, hasChildren);
            }
        }

        // Draw a single store item.
        public void DrawStoreItem<TItemData>(GameObject prefab, TItemData item, RectTransform contentSection, bool hadChildren)
            where TItemData : ItemData {
            StoreItem newStoreItem = Instantiate(prefab, contentSection.transform).GetComponent<StoreItem>();
            newStoreItem.Draw<TItemData>(item, 1f);

            if (hadChildren && newStoreItem.GetComponent<RotationAnimation>()!=null) {
                newStoreItem.GetComponent<RotationAnimation>().Play();
            }
            if (hadChildren && newStoreItem.GetComponent<SizeAnimation>()!=null) {
                newStoreItem.GetComponent<SizeAnimation>().Play();
            }

        }

    }

}
