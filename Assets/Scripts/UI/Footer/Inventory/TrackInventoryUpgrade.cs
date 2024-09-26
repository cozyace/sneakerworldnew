// System.
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    public class TrackInventoryUpgrade : MonoBehaviour {

        public string type = "store";

        public float realignInterval = 8f;
        public float ticks = 0f;

        public bool isUpgrading = false;
        public float upgradeDuration = -1f;
        public DateTime startTime;

        public int cost;

        public TextMeshProUGUI levelValText;
        public TextMeshProUGUI costText;

        public LerpInt costLerp;

        public float percent;

        public Button upgradeButton;
        public Image progressBar;

        public string debugCurrTime;
        public string debugStartTime;
        public string debugSeconds;

        void Start() {
            if (type == "store") {
                Player.instance.store.state.onUpdateStoreState.AddListener(UpdateAlignment);
            }
            else if (type =="inventory") {
                Player.instance.inventory.state.onUpdateStoreState.AddListener(UpdateAlignment);
            }
            upgradeButton.onClick.AddListener(UpgradeStore);
        }

        public void UpgradeStore() {
            if (type == "store") {
                Player.instance.store.state.UpgradeStore();
            }
            else if (type =="inventory") {
                Player.instance.inventory.state.UpgradeStore();
            }
            upgradeButton.gameObject.SetActive(false);
        }

        void FixedUpdate() {
            if (Player.instance.percentLoaded <0.99f) {
                return;
            }

            ticks += Time.fixedDeltaTime;
            if (ticks > realignInterval) {
                AsyncUpdateAlignment();
                ticks -= realignInterval;
            }

            UpdateTracker();
            UpdateCost();

        }

        void UpdateCost() {
            costText.text = costLerp.currValue.ToString();
        }

        void UpdateAlignment(StoreStateData stateData) {
            AsyncUpdateAlignment(stateData);
        }

        public async Task AsyncUpdateAlignment() {
            StoreStateData stateData = null;
            if (type == "store") {
                stateData = await Player.instance.store.state.GetState();
            }
            else if (type =="inventory") {
                stateData = await Player.instance.inventory.state.GetState();
            }
            AsyncUpdateAlignment(stateData);
        }

        public async Task AsyncUpdateAlignment(StoreStateData stateData) {
            isUpgrading = stateData.isUpgrading;
            upgradeDuration = stateData.upgradeDuration;
            startTime = DateTime.FromBinary(stateData.startUpgradeTime);

            if (type == "store") {
                costLerp.targetValue = Player.instance.store.state.GetUpgradePrice(stateData.level); 
            }
            else if (type =="inventory") {
                costLerp.targetValue = Player.instance.inventory.state.GetUpgradePrice(stateData.level); 
            }


            levelValText.text = stateData.level.ToString();

            if (isUpgrading) {
                upgradeButton.gameObject.SetActive(false);
                progressBar.transform.parent.gameObject.SetActive(true);
            }
            else {
                upgradeButton.gameObject.SetActive(true);
                progressBar.transform.parent.gameObject.SetActive(false);
            }
        }

        async Task UpdateTracker() {
            if (!isUpgrading || upgradeDuration <= 0f || startTime == null) {
                percent = 0f;
                return;
            }

            TimeSpan timeSpan = DateTime.UtcNow.Subtract(startTime); 
            debugSeconds = timeSpan.TotalSeconds.ToString();

            if (timeSpan.TotalSeconds > upgradeDuration) {
                isUpgrading = false;
                if (type == "store") {
                    await Player.instance.store.state.CheckFinishedUpgrading();
                }
                else if (type =="inventory") {
                    await Player.instance.inventory.state.CheckFinishedUpgrading();
                }
            }

            percent = Mathf.Min(1f, (float)timeSpan.TotalSeconds / upgradeDuration);

            debugStartTime = startTime.ToLongDateString();
            debugCurrTime = DateTime.UtcNow.ToLongDateString();
            
            progressBar.fillAmount = percent;

        }
        

    }

}

