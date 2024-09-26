// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;

namespace SneakerWorld.Main {

    public enum Brand {
        Badidas,
        Bike,
        // Bunderarmour,
        // Bike_Max,
        // Bike_Force,
        Count
    }

    public static class BrandTables {
        
        // public int requiredLevel;
        // public int basePrice;
        // public int luxuryValue;

        public static Dictionary<Brand, int> brandLevelLock = new Dictionary<Brand, int>() {
            [Brand.Badidas] = 0,
            [Brand.Bike] = 3,
            // [Brand.Bike_Max] = 0.85f,
            // [Brand.Bike_Force] = 0.95f,
        };

        public static Dictionary<Brand, float> brandValue = new Dictionary<Brand, float>() {
            [Brand.Badidas] = 1.5f,
            [Brand.Bike] = 2f,
            // [Brand.Bike_Max] = 0.85f,
            // [Brand.Bike_Force] = 0.95f,
        };

        public static Dictionary<Brand, float> brandBasePrice = new Dictionary<Brand, float>() {
            [Brand.Badidas] = 200,
            [Brand.Bike] = 300,
            // [Brand.Bike_Max] = 0.85f,
            // [Brand.Bike_Force] = 0.95f,
        };


    }

}
