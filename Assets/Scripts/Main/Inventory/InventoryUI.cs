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
    public class InventoryUI : MonoBehaviour {

        // Sneakers.
        public GameObject sneakerItemPrefab;
        public RectTransform sneakerContentSection;

        // Crates.
        public GameObject crateItemPrefab;
        public RectTransform cratesContentSection;

        // Regularly updating stock within the inventory.
        public InventoryData stock;

        public TextMeshProUGUI cratesAmountText;
        public TextMeshProUGUI sneakersAmountText;

        // Runs once on instantiation.
        void Awake() {
            Player.instance.inventory.onInventoryChanged.AddListener(DrawInventory);
        }
        
        // Draw the inventory ui.
        public void DrawInventory(InventoryData inventoryData) {
            // Cache the inventory data.
            stock = inventoryData;

            List<InventoryItem> sneakers = inventoryData.items.FindAll(s => s.itemId.Contains("sneaker"));
            DrawSneakers(sneakers);

            List<InventoryItem> crates = inventoryData.items.FindAll(s => s.itemId.Contains("crate"));
            DrawCrates(crates);

        }

        public void DrawSneakers(List<InventoryItem> sneakers) {
            foreach (Transform child in sneakerContentSection.transform) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < sneakers.Count; i++) {
                SneakerInventoryItem sneakerItem = Instantiate(sneakerItemPrefab, sneakerContentSection).GetComponent<SneakerInventoryItem>();
                sneakerItem.Draw(sneakers[i].itemId, sneakers[i].quantity);
            }
            sneakersAmountText.text = $"{sneakers.Count}/45 Sneakers";
        }

        public void DrawCrates(List<InventoryItem> crates) {
            foreach (Transform child in cratesContentSection.transform) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < crates.Count; i++) {
                CrateInventoryItem crateItem = Instantiate(crateItemPrefab, cratesContentSection).GetComponent<CrateInventoryItem>();
                crateItem.Draw(crates[i].itemId, crates[i].quantity);
            }
            cratesAmountText.text = $"{crates.Count}/45 Crates";
        }

    }

}
