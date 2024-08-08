// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// A convenient wrapper to put sneaker data into.
    /// </summary>
    [System.Serializable]
    public class SneakerData {

        // The prefix used to identify crates.
        public const string SNEAKER_ID_PREFIX = "sneaker";

        // The id of this crate.
        public string sneakerId => GetSneakerId();

        // The details of this sneaker.
        public string name;
        public Rarity rarity;
        public int price;
        // public string imagePath;
        public int level;

        // public bool signed;
        // public int version;
        // public Condition condition;

        public string GetSneakerId() {
            return GetSneakerId(name, rarity);
        }

        public static string GetSneakerId(string name, Rarity rarity) {
            return $"{SNEAKER_ID_PREFIX}-{name.ToLower().Replace(" ", "_")}-{rarity.ToString().ToLower().Replace(" ", "_")}"; 
        }

        // Get the data corresponding to this crate Id.
        public static async Task<SneakerData> GetSneakerById(string sneakerId) {
            string path = FirebasePath.ForSneakerWithId(sneakerId);
            return await FirebaseManager.GetDatabaseValue<SneakerData>(path);
        }

    }
    
}
