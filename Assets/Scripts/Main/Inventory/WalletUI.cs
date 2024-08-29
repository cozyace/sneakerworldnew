// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Listen to the wallet and draws it.
    /// </summary>
    public class WalletUI : MonoBehaviour {

        // The system that this UI tracks.
        public Wallet wallet;

        // The values.
        public int cash;
        public int gems;

        // The ui components.
        public TextMeshProUGUI cashText;
        public TextMeshProUGUI gemsText;

        // animation.
        public Image icon;
        Vector3 localScale;
        public AnimationCurve scaleCurve;
        private float ticks = 0f;
        public float duration = 2f;
        private int originalVal;
        private int totalChange;


        // Runs once before instantiation.
        public void Awake() {
            // Listen to the wallet events.
            wallet.onInitEvent.AddListener(DrawBalance);
            wallet.onTransactionEvent.AddListener(DrawTransaction);
            wallet.onGemTransactionEvent.AddListener(DrawGemTransaction);

            //
            localScale = icon.transform.localScale;
        }

        void FixedUpdate() {
            ticks += Time.fixedDeltaTime;

            if (ticks < duration) {
                
                icon.transform.localScale = localScale * scaleCurve.Evaluate(ticks / duration);
                
                float halfDur = 2f * ticks / duration;
                halfDur = Mathf.Clamp(halfDur, 0f, 1f);

                int currChange = (int)(Mathf.Floor(halfDur * totalChange));
                cashText.text = (originalVal + currChange).ToString();

            }

        }

        // Draw the cash balance.
        public void DrawBalance(int cash, int gems) {
            this.cash = cash;
            this.gems = gems;
            cashText.text = $"{this.cash}";
            gemsText.text = $"{this.gems}";            
        }

        // Draw the transaction.
        public void DrawTransaction(int prevValue, int deltaValue) {
            // cash = prevValue + deltaValue;
            // cashText.text = $"{cash}";
            ticks = 0f;
            originalVal = prevValue;
            totalChange = deltaValue;
        }

        // Draw the gems transaction.
        public void DrawGemTransaction(int prevValue, int deltaValue) {
            gems = prevValue + deltaValue;
            gemsText.text = $"{gems}";
        }

    }

}
