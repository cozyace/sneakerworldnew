// System.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// A place for customers to come and buy items in the store.
    /// </summary>
    public class Shelf : MonoBehaviour {

        public enum ShelfSize {
            Small = 3,
            Medium = 5,
            Large = 7
        }

        // The number of items that this shelf can display.
        public ShelfSize shelfSize;

        // The items on display on this shelf, in order.
        public List<string> itemIds = new List<string>();


        public void ValidateItems() {
            List<string> tmp = new List<string>();
            for (int i = 0; i < itemIds.Count; i++) {
                if (itemIds[i] != null && itemIds[i] != "") {
                    tmp.Add(itemIds[i]);
                }
            }
            itemIds = tmp;
        }

        public bool AddItemToShelf(string itemId) {
            ValidateItems();
            int maxSize = (int)shelfSize;
            if (itemIds.Count >= maxSize) {
                return false;
            }
            itemIds.Add(itemId);
            return true;
        }

        public bool RemoveItemFromShelf(string itemId) {
            bool foundItem = false;
            for (int i = 0; i < itemIds.Count; i++) {
                if (itemIds[i] == itemId) {
                    itemIds[i] = null;
                }
            }
            ValidateItems();
            return foundItem;
        }

    }

}
