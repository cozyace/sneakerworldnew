// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// Stores the crate data retrieved from the database.
    /// </summary>
    [System.Serializable]
    public class CrateData : ItemData {

        // The prefix used to identify crates.
        public const string CRATE_ID_PREFIX = "crate";

        public CrateData(Brand brand, Rarity rarity) {
            this.brand = brand;
            this.rarity = rarity;
        }

        // Uses the cumulative distribution to get a random sneaker.
        public SneakerData GetRandomSneakerFromCrate() {
            (Edition, Condition, Rarity) sneaker = RarityUtils.GetSneakerParamsFromCrate(brand, rarity);
            return new SneakerData(brand, sneaker.Item1, sneaker.Item2, sneaker.Item3);
        }

        public override string GetName() {
            return GetCrateName(brand);
        }

        public override string GetId() {
            return GetCrateId(brand, rarity);
        }

        public override int GetPrice() {
            return RarityUtils.CratePriceCalculator(brand, rarity);
        }

        public override int GetLevel() {
            return RarityUtils.GetLevel(brand, rarity);
        }

        public override string GetIconPath() {
            return $"Art/Crates/{brand.ToString()}";
        }

        public static string GetCrateName(Brand brand) {
            return $"{CleanString(brand)} Box";
        }

        public static string GetCrateId(Brand brand, Rarity rarity) {
            return $"{CRATE_ID_PREFIX}{ID_BREAK}{UnderscoredString(brand)}{ID_BREAK}{UnderscoredString(rarity)}"; 
        }

        // Get the data corresponding to this crate Id.
        public static CrateData ParseId(string crateId) {
            string[] idElems = crateId.Split(ID_BREAK);
            string prefix = idElems[0];
            if (prefix == CRATE_ID_PREFIX && idElems.Length == 3) {
                Brand brand;
                Rarity rarity;
                Enum.TryParse(idElems[1], out brand);

                Debug.Log(idElems[1]);
                Debug.Log(brand);

                Enum.TryParse(idElems[2], out rarity);
                Debug.Log("parsed a crate Id");
                return new CrateData(brand, rarity);
            }
            return null;
        }

        // Get the data corresponding to this crate Id.
        public static async Task<CrateData> GetCrateById(string crateId) {
            string path = FirebasePath.ForCrateWithId(crateId);
            return await FirebaseManager.GetDatabaseValue<CrateData>(path);
        }

    }

}

