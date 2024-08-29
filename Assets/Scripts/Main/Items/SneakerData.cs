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
    public class SneakerData : ItemData {

        // The prefix used to identify crates.
        public const string SNEAKER_ID_PREFIX = "sneaker";

        // The details of this sneaker.
        public Condition condition;
        public Edition edition;
        // public string imagePath;
        // public bool signed;
        // public int version;

        public SneakerData(Brand brand, Edition edition, Condition condition, Rarity rarity) {
            this.brand = brand;
            this.edition = edition;
            this.condition = condition;
            this.rarity = rarity;
        }

        public SneakerData(Brand brand, Edition edition, Condition condition) {
            this.brand = brand;
            this.edition = edition;
            this.condition = condition;
            this.rarity = RarityUtils.RarityFromCondAndEd(brand, condition, edition);
        }

        public override string GetId() {
            return GetSneakerId(brand, edition, condition, rarity);
        }

        public override string GetName() {
            return GetSneakerName(brand, edition, condition);
        }

        public override int GetPrice() {
            return RarityUtils.ShoePriceCalculator(brand, rarity);
        }

        public override int GetLevel() {
            return RarityUtils.GetLevel(brand, rarity);
        }

        public override string GetIconPath() {
            return $"Art/Sneakers/{brand.ToString()}";
        }

        public static string GetSneakerName(Brand brand, Edition edition, Condition condition) {
            return $"{CleanString(brand)} {AbbreviateEdition(edition)} [{CleanString(condition)}]"; 
        }

        public static string AbbreviateEdition(Edition edition) {
            return edition switch {
                Edition.Original => "",
                Edition.First => "1st",
                Edition.Second => "2nd",
                Edition.RipOff => "Fake",
            };
        }

        public static string GetSneakerId(Brand brand, Edition edition, Condition condition, Rarity rarity) {
            return $"{SNEAKER_ID_PREFIX}{ID_BREAK}{UnderscoredString(brand)}{ID_BREAK}{CleanString(edition)}{ID_BREAK}{UnderscoredString(condition)}{ID_BREAK}{UnderscoredString(rarity)}"; 
        }

        // Get the data corresponding to this crate Id.
        public static SneakerData ParseId(string crateId) {
            string[] idElems = crateId.Split(ID_BREAK);
            string prefix = idElems[0];
            if (prefix == SNEAKER_ID_PREFIX && idElems.Length == 5) {
                Brand brand;
                Edition ed;
                Condition cond;
                Rarity rarity;
                Enum.TryParse(idElems[1], out brand);
                Enum.TryParse(idElems[2], out ed);
                Enum.TryParse(idElems[3], out cond);
                Enum.TryParse(idElems[4], out rarity);
                
                Debug.Log("parsed a crate Id");
                return new SneakerData(brand, ed, cond);
            }
            return null;
        }

        // Get the data corresponding to this crate Id.
        public static async Task<SneakerData> GetSneakerById(string sneakerId) {
            string path = FirebasePath.ForSneakerWithId(sneakerId);
            return await FirebaseManager.GetDatabaseValue<SneakerData>(path);
        }

    }
    
}
