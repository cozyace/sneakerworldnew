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
    public class StoreStateData {
        public int level = 1;
        public int sneakerCount => 2 * level;
        public int crateCount => 2 * level;

        public int inventorySneakerMax => 5 * level;
        public int inventoryCratesMax => 5 * level;
        public int maxSneakersOnSale => 3 * level;

        public bool isUpgrading = false;
        public long startUpgradeTime = 0;
        public float upgradeDuration = -1f;
    }

    /// <summary>
    /// Wraps the user data in a convenient class. 
    /// </summary>
    public class StoreUpgrader : MonoBehaviour {

        public string type = "store";


        // The message to throw up if the player does not have enough funds.
        public const string NOT_ENOUGH_FUNDS_MESSAGE = "Not enough funds to upgrade!";

        // The message to throw up if the player does not have enough funds.
        public const string SUCCESSFUL_REROLL_MESSAGE = "Successfully upgraded!";

        public UnityEvent<StoreStateData> onUpdateStoreState = new UnityEvent<StoreStateData>();
        public UnityEvent<string> onUpgradeFailed = new UnityEvent<string>();

        public bool initialized = false;

        public async Task Init() {
            StoreStateData stateData = await GetState();
            CheckFinishedUpgrading();
            onUpdateStoreState.Invoke(stateData);
            initialized = true;
        }

        public async Task<StoreStateData> GetState() {
            StoreStateData stateData =await FirebaseManager.GetDatabaseValue<StoreStateData>(FirebasePath.StatePath(type));
            if (stateData == null) {
                stateData = new StoreStateData();
                await SetState(stateData);
            }
            return stateData;
        }

        public async Task SetState(StoreStateData stateData) {
            await FirebaseManager.SetDatabaseValue<StoreStateData>(FirebasePath.StatePath(type), stateData);
            onUpdateStoreState.Invoke(stateData);
        }

        // Set the username.
        public async Task UpgradeStore() {

            try {
                StoreStateData stateData = await GetState();

                if (stateData.isUpgrading) {
                    throw new Exception("Already upgrading");
                }

                int price = GetUpgradePrice(stateData.level);
                
                // Check the Player.instance can afford the crate.
                bool hasFunds = await Player.instance.wallet.Debit(price);
                if (!hasFunds) {
                    throw new Exception(NOT_ENOUGH_FUNDS_MESSAGE);
                }
                Debug.Log("Managed to process debit.");

                stateData.isUpgrading = true;
                stateData.startUpgradeTime = DateTime.UtcNow.ToBinary();
                stateData.upgradeDuration = GetDuration(stateData.level);

                await SetState(stateData);               
            
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
                onUpgradeFailed.Invoke(exception.Message);

                StoreStateData stateData = await GetState();
                await SetState(stateData);               

            }
            
        }

        private bool checking = false;
        public async Task CheckFinishedUpgrading() {
            if (checking) {
                return;
            }
            checking = true;

            StoreStateData stateData = await GetState();

            TimeSpan timeSpan = DateTime.UtcNow.Subtract(DateTime.FromBinary(stateData.startUpgradeTime)); 
            if (timeSpan.TotalSeconds > stateData.upgradeDuration) {
                if (stateData.isUpgrading) {
                    stateData.level += 1;
                }
                stateData.isUpgrading = false;
                stateData.startUpgradeTime = DateTime.UtcNow.ToBinary();
                stateData.upgradeDuration = -1f;
                await SetState(stateData);       
            }

            checking = false;

        }

        public int GetUpgradePrice(int level) {
            return (int)(200f * Mathf.Pow(2f, (float)level));
        }

        public float GetDuration(int level) {
            return Mathf.Min(60f*60f*24f*7f, 2f * Mathf.Pow(2f, (float)level));
        }

    }


}
