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

        public void Draw(List<Item> item) {
            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < item.Count; i++) {
                ItemSlot slot = Instantiate(itemPrefab, contentSection).GetComponent<ItemSlot>();
                slot.Draw(item[i].itemId, item[i].quantity, item[i].onSale, item[i].markup);
            }
        }

    }

}
