// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    public class PurchaseItemPopup : MonoBehaviour {

        [Header("pop up animation")]
        private float popupTicks = 0f;
        public float popupDuration = 1f;
        private Vector3 localScale;
        public Transform popupTransform;
        public AnimationCurve scaleCurve;

        [Header("pop up components")]

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI totalPriceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public TextMeshProUGUI maxQuantityText;
        public Image rarityPanel;
        public Image icon;

        public TextMeshProUGUI claimNameText;
        public TextMeshProUGUI claimRarityText;
        public TextMeshProUGUI claimQuantityText;
        public Image claimRarityPanel;
        public Image claimIcon;


        // 
        public GameObject purchaseScreen;
        public GameObject loadingScreen;
        public GameObject claimScreen;
        public GameObject openButton;

        public Image completePurchaseIcon;
        public Text completePurchaseText;


        // Runs once on instantiation.
        void Awake() {
            // Quantity change events.
            Player.instance.purchaser.onQuantityChanged.AddListener(DrawQuantity);
            // The start events.
            Player.instance.purchaser.onPurchaseCrateStartEvent.AddListener(DrawStartPurchase<CrateData>);
            Player.instance.purchaser.onPurchaseSneakerStartEvent.AddListener(DrawStartPurchase<SneakerData>);
            // Processing the purchase events.
            Player.instance.purchaser.onProcessingPurchase.AddListener(DrawProcessingPurchase);
            // The end events.
            Player.instance.purchaser.onPurchaseSuccessEvent.AddListener(DrawSuccessfulPurchase);
            Player.instance.purchaser.onPurchaseFailedEvent.AddListener(DrawFailedPurchase);

            // Cache variables.
            localScale = popupTransform.localScale;
            purchaseScreen.SetActive(false);
            popupTransform.gameObject.SetActive(false);
            claimScreen.SetActive(false);
            loadingScreen.SetActive(false);

        }

        void FixedUpdate() {
            if (!Application.isPlaying || !popupTransform.gameObject.activeSelf) { return; }

            popupTicks += Time.fixedDeltaTime;
            if (popupTicks > popupDuration) {
                return;
            }

            popupTransform.localScale = localScale * scaleCurve.Evaluate(popupTicks / popupDuration);

        }

        // Update the quantity.
        void DrawQuantity(int quantity, int maxQuanity, int price) {
            quantityText.text = quantity.ToString() + "/" + maxQuanity.ToString();
            maxQuantityText.text = " "; // maxQuanity.ToString() + " Left";
            priceText.text = price.ToString();
            totalPriceText.text = (quantity * price).ToString();
        }

        // Draw the start of a purchase.
        public void DrawStartPurchase<TItemData>(TItemData itemData)
            where TItemData : ItemData {
            // Set the ui components.
            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();
            SetRarityImage(rarityPanel, itemData.rarity);
            SetIconImage(icon, itemData);

            purchaseScreen.SetActive(true);
            popupTransform.gameObject.SetActive(true);

            popupTicks = 0f;
        }

        // While processing a purchase.
        void DrawProcessingPurchase() {
            loadingScreen.SetActive(true);
            popupTransform.gameObject.SetActive(false);
        }

        // Draw a successful purchase.
        void DrawSuccessfulPurchase(string message, string itemId, int quantity) {
            claimNameText.text = nameText.text;
            claimRarityText.text = rarityText.text;
            claimQuantityText.text = "x"+quantity.ToString();
            claimRarityPanel.sprite = rarityPanel.sprite;
            claimIcon.sprite = icon.sprite;

            loadingScreen.SetActive(false);
            claimScreen.SetActive(true);

            if (CrateData.ParseId(itemId) != null) {
                openButton.SetActive(true);
            }
            else {
                openButton.SetActive(false);
            }

        }

        // Draw a failed purchase.
        void DrawFailedPurchase(string message) {
            loadingScreen.SetActive(false);
            popupTransform.gameObject.SetActive(true);
        }

        public static void SetRarityImage(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIconImage(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }


    }

}
