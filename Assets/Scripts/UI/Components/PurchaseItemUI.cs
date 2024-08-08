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

    using ItemData = SneakerWorld.Main.ItemData;
    using Rarity = SneakerWorld.Main.Rarity;
    // using RarityUtils = SneakerWorld.Main.RarityUtils;

    public class PurchaseItemUI : MonoBehaviour {

        public PurchaseHandler purchaseHandler;

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rarityText;
        public TextMeshProUGUI quantityText;
        public Image rarityPanel;
        public Image icon;

        public GameObject loadingScreen;
        public Image completePurchaseIcon;
        public Text completePurchaseText;

        void Start() {
            purchaseHandler.onQuantityChanged.AddListener(UpdateQuantity);

            purchaseHandler.onPurchaseCrateStartEvent.AddListener(Draw<CrateData>);
            purchaseHandler.onPurchaseSneakerStartEvent.AddListener(Draw<SneakerData>);

            purchaseHandler.onProcessingPurchase.AddListener(ProcessingPurchase);

            purchaseHandler.onPurchaseSuccessEvent.AddListener(CompletePurchase);
            purchaseHandler.onPurchaseFailedEvent.AddListener(FailPurchase);

        }

        void UpdateQuantity(int quantity, int price) {
            quantityText.text = quantity.ToString();
            priceText.text = $"${int.Parse(quantityText.text) * price}";
        }

        public void Draw<TItemData>(TItemData itemData)
            where TItemData : ItemData {

            Debug.Log(itemData.name);

            nameText.text = itemData.name;
            rarityText.text = itemData.rarity.ToString();

            SetRarityPanel(rarityPanel, itemData.rarity);
            SetIcon(icon, itemData);

            // mainPanel.sprite = Resources.Load<Sprite>(crateData.imagePath);

        }

        void ProcessingPurchase() {
            // loadingScreen.gameObject.SetActive(true);
        }

        void CompletePurchase(string message, string itemId, int quantity) {
            // loadingScreen.gameObject.SetActive(false);

            // completePurchaseIcon.sprite = icon.sprite;
            // completePurchaseText.text = message + $"x{quantity}";
        }

        void FailPurchase(string message) {
            
        }

        public static void SetRarityPanel(Image image, Rarity rarity) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Rarities/{rarity.ToString()}");
        }

        public static void SetIcon(Image image, ItemData item) {
            image.sprite = Resources.Load<Sprite>(item.iconPath);
        }


    }

}
