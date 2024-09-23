// System.
using System;
using System.Collections;
using System.Collections.Generic;
using SneakerWorld.Main;

// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    public enum ItemType {
        Sneaker,
        Crate,
        Manager,
        Decoration
    }

    public class ItemIdentifier {
		public string identifierType;
		public string identifierValue;

        public static ItemIdentifier New<TEnum>(TEnum tenum) where TEnum : Enum {
            identifierValue = tenum.ToString();
            identifierType = tenum.Type.Name;
        }

        public bool IsSameType<TEnum>() where TEnum : Enum {
            string inputType = TEnum.Name;
            return (inputType == identifierType);
        } 
	}

    [System.Serializable]
    public class Item {

        // The id of this crate.
        public ItemType itemType;
        public List<ItemIdentifier> ids = new List<ItemIdentifier>();
        public int quantity = 0;

        // The other stuff.
        public string name => NameGenerator.GenerateName(this);
        public int price => PriceCalculator.CalculatePrice(this);

        // public string imagePath => GetImagePath();
        // public int level => GetLevel();

        // How much of this item there is.
        // public bool featured = false;

        // The generic details of this item.
        public bool onSale = false;
        public float markup = 0f;

        public Item(ItemType itemType, int quantity = 1, List<ItemIdentifier> ids = null) {
            this.itemType = itemType;
            this.quantity = quantity;
            this.ids = ids;
            if (this.ids = null) {
                this.ids = new List<ItemIdentifier>();
            }
        } 

        public bool IsEqual(Item item) {
            // Check they are the same item type.
            if (itemType != item.itemType) { 
                return false;
            }

            // Check they have the same number of identifiers.
            if (ids.Count != item.ids.Count) {
                return false;
            }

            // Check all their identifiers are the same.
            for (int i = 0; i < ids.Count; i++) {
                string correspondingId = item.ids.Find(id => id.identifierType == ids[i].identifierType);
                if (correspondingId == null || correspondingId.identifierValue != ids[i].identifierValue) {
                    return false;
                }
            }

            // If all validation has been passed.
            return true;

        }

        public void AddId<TEnum>(TEnum tenum) where TEnum : Enum {
            ItemIdentifier id = ids.Find(x => x.IsSameType<TEnum>());
            if (id != null) {
                id = new ItemIdentifier.New<TEnum>(tenum);
                return;
            }
            ids.Add(new ItemIdentifier.New<TEnum>(tenum));
        }

        public bool HasId<TEnum>() where TEnum : Enum {
            ItemIdentifier id = ids.Find(x => x.IsSameType<TEnum>());
            if (id != null) {
                return true;
            }
            return false;
        }

        public TEnum FindId<TEnum>() where TEnum : Enum {
            ItemIdentifier id = ids.Find(x => x.IsSameType<TEnum>());
            if (id != null) {
                return TEnum.Parse(id.identifierValue);
            }
            return (TEnum)0;
        }

        public Item Duplicate(int quantity = -1) {
            quantity = quantity < 0 ? this.quantity : quantity;
            return new Item(itemType, quantity, ids);
        }

    }

}

