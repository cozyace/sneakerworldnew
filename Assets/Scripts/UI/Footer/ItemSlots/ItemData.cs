// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    [System.Serializable]
    public abstract class ItemData {

        public const string ID_BREAK = "-";

        // The id of this crate.
        public string id => GetId();
        public string name => GetName();
        public int price => GetPrice();
        public string iconPath => GetIconPath();
        public int level => GetLevel();

        // 
        public int quantity = 0;
        public bool featured = false;

        // The generic details of this item.
        public Brand brand;
        public Rarity rarity;

        public abstract string GetName();

        public abstract string GetId();

        public abstract int GetPrice();

        public abstract int GetLevel();

        public abstract int GetMaterial() {


        }

        public abstract string GetIconPath();

        public static string UnderscoredString(Enum e) {
            return e.ToString().Replace(" ", "_");
        }

        public static string CleanString(Enum e) {
            return e.ToString().Replace("_", " ");
        }

    }

}

