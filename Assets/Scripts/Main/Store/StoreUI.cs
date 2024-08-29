// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Listens to the store and updates the UI accordingly. 
    /// </summary>
    public class StoreUI : MonoBehaviour {

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
        public List<CrateData> featuredCrates = new List<CrateData>();
        public List<CrateData> crates = new List<CrateData>();
        public List<SneakerData> featuredSneakers = new List<SneakerData>();
        public List<SneakerData> sneakers = new List<SneakerData>();

        // Timers.
        public TextMeshProUGUI featuredCrateTimerText;
        public TextMeshProUGUI featuredSneakerTimerText;


        // Runs once on instantiation.
        void Awake() {
            // Listen for changes in crates.
            Player.instance.store.onFeaturedCratesUpdated.AddListener(DrawFeaturedCrates);
            Player.instance.store.onCratesUpdated.AddListener(DrawRegularCrates);
            // Listen for changes in sneakers.
            Player.instance.store.onFeaturedSneakersUpdated.AddListener(DrawFeaturedSneakers);
            Player.instance.store.onSneakersUpdated.AddListener(DrawRegularSneakers);
        }

        // Runs once before the first frame.
        void Start() {
            // Draw the first pass.
            // DrawRegularContent<CrateData>(vendor.crates);
            // DrawRegularContent<SneakerData>(vendor.sneakers);
            // DrawFeaturedContent<CrateData>(vendor.featuredCrates);
            // DrawFeaturedContent<SneakerData>(vendor.featuredSneakers);
        }

        void FixedUpdate() {
            UpdateTimeLeft();
        }

        void UpdateTimeLeft() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            DateTime tomorrow = new DateTime(year, month, day+1);
            TimeSpan timeSpan = tomorrow.Subtract(DateTime.UtcNow);
            // Debug.Log(timeSpan.ToString(@"hh\:mm\:ss"));
            featuredCrateTimerText.text = timeSpan.ToString(@"hh\:mm\:ss") + " Left";
            featuredSneakerTimerText.text = timeSpan.ToString(@"hh\:mm\:ss") + " Left";
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

            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < items.Count; i++) {
                DrawStoreItem<TItemData>(prefab, items[i], contentSection);
            }
        }

        // Draw a single store item.
        public void DrawStoreItem<TItemData>(GameObject prefab, TItemData item, RectTransform contentSection)
            where TItemData : ItemData {
            StoreItem newStoreItem = Instantiate(prefab, contentSection.transform).GetComponent<StoreItem>();
            newStoreItem.Draw<TItemData>(item);
        }

    }

}
