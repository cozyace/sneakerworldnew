// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public static class EnumExtensions {
        public static int ToInt(this Enum enumValue) {
            return Convert.ToInt32(enumValue);
        }
    }

    public enum Condition {
        Tattered,
        Worn,
        Decent,
        New,
        Mint,
        Count
    }

    public enum Edition {
        RipOff,
        Second,
        First,
        Original,
        Count,
    }

    public enum Recolor {
        Blue,
        Brown,
        Green,
        Purple,
        Red,
        Teal,
        Yellow,
        Count,
    }

    public enum Rarity {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Count,
    }

}
