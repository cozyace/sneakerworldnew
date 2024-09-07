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

    [System.Serializable]
    public class StoreRoller {

        // The message to throw up if the Player.instance does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to reroll!";

        // The message to throw up if the Player.instance does not have enough funds.
        public const string SUCCESSFUL_REROLL_MESSAGE = "Successfully rerolled!";

        // Reroll events.
        public UnityEvent<int> onRerollSneakersEvent = new UnityEvent<int>();
        public UnityEvent<int> onRerollCratesEvent = new UnityEvent<int>();

        public UnityEvent<int, int> onRerollInit = new UnityEvent<int, int>();

        // The price for rerolling
        public int pricePerReroll = 100;

        public void Init(StoreData store) {
            int nextRollPrice = GetRollPrice(store.sneakerRerolls);
            int nextCratesRollPrice = GetRollPrice(store.crateRerolls);

            onRerollInit.Invoke(nextRollPrice, nextCratesRollPrice);
        }

        public List<CrateData> AllCrates() {
            List<CrateData> crateData = new List<CrateData>();

            int rarityCount = (int)Rarity.Count;
            int brandCount = (int)Brand.Count;
            for (int j = 0; j < brandCount; j++) {
                for (int i = 0; i < rarityCount; i++) {
                    crateData.Add(new CrateData((Brand)j, (Rarity)i));
                }
            }

            crateData.Sort((x, y) => x.price.CompareTo(y.price));
            return crateData;
        }

        public List<SneakerData> RandomSneakerData(int count) {
            List<CrateData> crateData = RandomCrateData(count, true);
            List<SneakerData> sneakers = new List<SneakerData>();
            
            foreach (CrateData crate in crateData) {
                SneakerData sneaker = crate.GetRandomSneakerFromCrate();
                SneakerData existingSneaker = sneakers.Find(s => s.id == sneaker.id);

                int depth = 0;
                while (existingSneaker != null && depth < 200) {
                    sneaker = crate.GetRandomSneakerFromCrate();
                    existingSneaker = sneakers.Find(s => s.id == sneaker.id);
                    depth += 1;
                }

                sneaker.quantity = UnityEngine.Random.Range(5, 10); 
                sneakers.Add(sneaker);
            }

            sneakers.Sort((x, y) => x.level.CompareTo(y.level));
            return sneakers;

        }

        public List<InventoryItem> RandomSneakers(int count) {
            List<SneakerData> sneakers = RandomSneakerData(count);
            List<InventoryItem> items = new List<InventoryItem>();
            int quantity = 0;
            foreach (SneakerData sneaker in sneakers) {
                quantity = (int)Mathf.Ceil(UnityEngine.Random.Range(100, 50) / (sneaker.level + 1));
                items.Add(new InventoryItem(sneaker.id, quantity));
            }
            return items;
        }

        public List<CrateData> RandomCrateData(int count, bool allowRepeats = false) {

            List<CrateData> allCrates = AllCrates();
            List<CrateData> crates = new List<CrateData>();
            List<int> cachedIndices = new List<int>();
            int crateCount = allCrates.Count;

            int i = 0;
            int depth = 0; 
            while (i < count && depth < 200) {

                int index = UnityEngine.Random.Range(0, crateCount);
                if (allowRepeats || !cachedIndices.Contains(index)) {
                    crates.Add(allCrates[index]);
                    cachedIndices.Add(index);
                    i += 1;
                }

                depth += 1;

            }
            crates.Sort((x, y) => x.level.CompareTo(y.level));
            return crates;
        }

        public List<InventoryItem> RandomCrates(int count, bool allowRepeats = false) {
            List<CrateData> crates = RandomCrateData(count, allowRepeats);
            List<InventoryItem> items = new List<InventoryItem>();
            int quantity = 0;
            foreach (CrateData crate in crates) {
                quantity = (int)Mathf.Ceil(UnityEngine.Random.Range(100, 50) / (crate.level + 1));
                items.Add(new InventoryItem(crate.id, quantity));
            }
            return items;
        }

        public void RerollSneakers() {
            AsyncRerollSneakers();
        }

        public void RerollCrates() {
            AsyncRerollCrates();
        }

        public async Task AsyncRerollSneakers() {

            try {
                StoreData store = await Player.instance.store.GetStore();
                StoreStateData storeState = await Player.instance.store.state.GetState();
                int sneakerCount = storeState.sneakerCount;
                int rerollPrice = GetRollPrice(store.sneakerRerolls);

                // Check the Player.instance can afford the crate.
                bool hasFunds = await Player.instance.wallet.Debit(rerollPrice);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                store.sneakers = RandomSneakers(sneakerCount);
                store.sneakerRerolls += 1;
                int nextRollPrice = GetRollPrice(store.sneakerRerolls);

                await Player.instance.store.SetStore(store);

                Debug.Log(SUCCESSFUL_REROLL_MESSAGE);
                // onRerollEvent.Invoke(rerollPrice, rerollPrice+pricePerReroll);
                onRerollSneakersEvent.Invoke(nextRollPrice);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                // onPurchaseFailedEvent.Invoke(exception.Message);
            }
            
        }

        public async Task AsyncRerollCrates() {

            try {
                StoreData store = await Player.instance.store.GetStore();
                StoreStateData storeState = await Player.instance.store.state.GetState();
                int crateCount = storeState.crateCount;
                int rerollPrice = GetRollPrice(store.crateRerolls);

                // Check the Player.instance can afford the crate.
                bool hasFunds = await Player.instance.wallet.Debit(rerollPrice);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                // Add the sneaker to inventory.
                store.crates = RandomCrates(crateCount, false);
                store.crateRerolls += 1;
                int nextRollPrice = GetRollPrice(store.crateRerolls);

                await Player.instance.store.SetStore(store);

                Debug.Log(SUCCESSFUL_REROLL_MESSAGE);
                onRerollCratesEvent.Invoke(nextRollPrice);
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                Player.instance.purchaser.onPurchaseFailedEvent.Invoke(exception.Message);
            }
        }

        public int GetRollPrice(int currRolls) {
            return pricePerReroll * (currRolls + 1);
        }

    }

}
