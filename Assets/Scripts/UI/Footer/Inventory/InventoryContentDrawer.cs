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
    public class InventoryContentDrawer : MonoBehaviour {

        // Sneakers.
        public GameObject itemPrefab;
        public RectTransform contentSection;
        public TextMeshProUGUI amountText;

        // text.
        public string amountStringTrail = "/45 Sneakers";

        public void Draw(List<InventoryItem> item) {
            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < item.Count; i++) {
                InventorySlot slot = Instantiate(itemPrefab, contentSection).GetComponent<InventorySlot>();
                slot.Draw(item[i].itemId, item[i].quantity);
            }
            amountText.text = $"{item.Count}{amountStringTrail}";
        }
        
    }

}
