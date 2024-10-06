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

        // Track the upgrade timer.
        public bool isUpgrading = false;
        public float upgradeDuration = -1f;
        public DateTime startTime;
        public float percentCompleted;
        
        // Realign the upgrade timer.
        public float realignInterval = 8f;
        public float ticks = 0f;
        
        // Track the cost.
        public LerpInt costLerp;
        public int cost;
        public TextMeshProUGUI costText;

        // Track the level.
        public TextMeshProUGUI levelValText;

        // The components.
        public Button upgradeButton;
        public Image progressBar;

        // Console debugging.
        public string debugCurrTime;
        public string debugStartTime;
        public string debugSeconds;

        // Runs once before the first frame.
        void Start() {
            HookComponents();
        }

        // Hook the ui components together.
        void HookComponents() {
            upgradeButton.onClick.AddListener(StartUpgradeInventory);
        }

        void StartUpgradeInventory() {
            InventoryUpgrader.TryUpgrade(inventorySystem);
        }

        void FixedUpdate() {
            if (Player.instance.percentLoaded <0.99f) {
                return;
            }

            CheckAlignment();

            UpdateCost();
            UpdateDuration();

        }

        void CheckAlignment() {
            ticks += Time.fixedDeltaTime;
            if (ticks > realignInterval) {
                UpdateAlignment();
                ticks -= realignInterval;
            }
        }

        void UpdateCost() {
            costText.text = costLerp.currValue.ToString();
        }

        async Task UpdateAlignment() {
            Inventory inventory = await inventorySystem.Get();

            isUpgrading = inventory.isUpgrading;
            upgradeDuration = inventory.upgradeDuration;
            startTime = DateTime.FromBinary(inventory.startUpgradeTime);

            costLerp.targetValue = InventoryUpgrader.GetUpgradePrice(inventory.level); 
            levelValText.text = inventory.level.ToString();

            if (isUpgrading) {
                upgradeButton.gameObject.SetActive(false);
                progressBar.transform.parent.gameObject.SetActive(true);
            }
            else {
                upgradeButton.gameObject.SetActive(true);
                progressBar.transform.parent.gameObject.SetActive(false);
            }
        }

        async Task UpdateDuration() {
            if (!isUpgrading || upgradeDuration <= 0f || startTime == null) {
                percentCompleted = 0f;
                return;
            }

            TimeSpan timeSpan = DateTime.UtcNow.Subtract(startTime); 
            debugSeconds = timeSpan.TotalSeconds.ToString();

            if (timeSpan.TotalSeconds > upgradeDuration) {
                isUpgrading = false;
                await InventoryUpgrader.CheckFinishedUpgrading(inventorySytem);
            }

            percentCompleted = Mathf.Min(1f, (float)timeSpan.TotalSeconds / upgradeDuration);

            debugStartTime = startTime.ToLongDateString();
            debugCurrTime = DateTime.UtcNow.ToLongDateString();
            
            progressBar.fillAmount = percentCompleted;

        }
        

    }

}

