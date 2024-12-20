// System.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    public static class ItemMaker {

        public static List<Item> Random(ItemType itemType, int count, bool repeats = false) {
            switch (itemType) {
                case (ItemType.Sneaker):
                    return GenerateRandomSneakers(count, repeats);
                case (ItemType.Crate):
                    return GenerateRandomCrates(count, repeats);
                default:
                    return new List<Item>(); 
            }
        }

        // Make a single crate.
        public static Item MakeCrate(Brand brand, Rarity rarity) {
            Item crate = new Item(ItemType.Crate, 1);
            crate.AddId<Brand>(brand);
            crate.AddId<Rarity>(rarity);
            return crate;
        }

        // Make a single random crate.
        public static Item RandomCrate() {
            Brand brand = RandomEnum<Brand>(Brand.Count);
            Rarity rarity = RandomEnum<Rarity>(Rarity.Count);
            return MakeCrate(brand, rarity);
        }

        public static Item OpenCrate(Item crate) {
            if (crate.itemType == ItemType.Crate && crate.ids.HasId<Brand>() && crate.ids.HasId<Rarity>()) {
                Item sneaker = RarityUtils.GetSneakerFromCrate(crate.FindId<Brand>(), crate.FindId<Rarity>());
                return sneaker;
            }
            return null;
        }

        // Make a single sneaker.
        public static Item MakeSneaker(Brand brand, Condition condition, Edition edition, ItemColor color) {
            Item sneaker = new Item(ItemType.Crate, 1);
            sneaker.AddId<Brand>(brand);
            sneaker.AddId<Condition>(condition);
            sneaker.AddId<Edition>(edition);
            sneaker.AddId<ItemColor>(color);
            sneaker.AddId<Rarity>(PriceCalculator.GetSneakerRarity(sneaker));
            return sneaker;
        }

        // Make a single random sneaker.
        public static Item RandomSneaker() {
            Brand brand = RandomEnum<Brand>(Brand.Count);
            Condition condition = RandomEnum<Condition>(Condition.Count);
            Edition edition = RandomEnum<Edition>(Edition.Count);
            ItemColor color = RandomEnum<ItemColor>(ItemColor.Count);
            return MakeSneaker(brand, condition, edition, color);
        }

        public static List<Item> GenerateAllCrates() {
            // Create an empty list.
            List<Item> crates = new List<Item>();

            // Itterate through the identifiers for a crate.
            int rarityCount = (int)Rarity.Count;
            int brandCount = (int)Brand.Count;
            for (int j = 0; j < brandCount; j++) {
                for (int i = 0; i < rarityCount; i++) {
                    Item crate = MakeCrate((Brand)j, (Rarity)i);
                    crates.Add(crate);
                }
            }

            // Sort them.
            crates.Sort((x, y) => x.price.CompareTo(y.price));
            return crates;
        }

        // Generate a random count of crates.
        public static List<Item> GenerateRandomCrates(int count, bool allowRepeats = false) {
            // Create an empty list.
            List<Item> crates = new List<Item>();

            // Itterate through and generate the random crates.
            for (int i = 0; i < count; i++) {
                Item crate = RandomCrate();
                while (!allowRepeats && crates.Find(x => x.IsEqual(crate)) != null) {
                    crate = RandomCrate();
                }
                crates.Add(crate);
            }

            crates.Sort((x, y) => x.price.CompareTo(y.price));
            return crates;
        }

        // Generate a random count of sneakers.
        public static List<Item> GenerateRandomSneakers(int count, bool allowRepeats = false) {
            // Create an empty list.
            List<Item> sneakers = new List<Item>();

            // Itterate through and generate the random crates.
            for (int i = 0; i < count; i++) {
                Item sneaker = RandomSneaker();
                while (!allowRepeats && sneakers.Find(x => x.IsEqual(sneaker)) != null) {
                    sneaker = RandomSneaker();
                }
                sneakers.Add(sneaker);
            }

            sneakers.Sort((x, y) => x.price.CompareTo(y.price));
            return sneakers;
        }
        
        
        // Generate a random count of sneakers by simulating crates.
        public static List<Item> GenerateSneakersFromRandomCrates(int count, bool allowRepeats = false) {

            // Initialize the lists.
            List<Item> crates = GenerateRandomCrates(count, true);
            List<Item> sneakers = new List<Item>();
            
            foreach (Item crate in crates) {
                Item sneaker = OpenCrate(crate);
                Item existingSneaker = sneakers.Find(s => s.IsEqual(sneaker));

                // Open until we get a unique sneaker.
                int depth = 0;
                while (!allowRepeats && existingSneaker != null && depth < 200) {
                    sneaker = OpenCrate(crate);
                    existingSneaker = sneakers.Find(s => s.IsEqual(sneaker));
                    depth += 1;
                }
                sneakers.Add(sneaker);
            }

            // Sort and return.
            sneakers.Sort((x, y) => x.level.CompareTo(y.level));
            return sneakers;

        }

        public static Item GenerateSneakerFromCrate(Brand brand, Rarity rarity) {

            // Get the allowed percentile range for this crate.
            Vector2 probDis = cratePercentileRange[rarity];

            // Get random probability values for the condition and edition of the sneaker based on this range.
            float conditionProb = UnityEngine.Random.Range(probDis.x, probDis.y);
            float editionProb = UnityEngine.Random.Range(probDis.x, probDis.y);

            // Get the edition and condition values based on the probability above.
            (Edition, float) ed = editionPercentiles.Sample(editionProb);
            (Condition, float) cond = conditionPercentiles.Sample(conditionProb);

            // Get the rarity based on the output sneaker.
            float aveProb = (ed.Item2 * cond.Item2);
            float brandAdjustedProb = brandRarityAdjustment[brand] * aveProb;
            (Rarity, float) outputRarity = rarityPercentiles.Sample(brandAdjustedProb);

            // Create the instance.
            Item sneaker = new Item(ItemType.Sneaker);

            // Set the params of the sneaker.
            sneaker.AddId(ed.Item1);
            sneaker.AddId(cond.Item1);
            sneaker.quantity = UnityEngine.Random.Range(5, 10); 

        }


        // Get a random enum from a generic enum.
        public static TEnum RandomEnum<TEnum>(TEnum maxValue) where TEnum : Enum {
            int x = UnityEngine.Random.Range(0, (int)maxValue);
            return (TEnum)x;
        }
    }

}
