using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;
    using Utils;

    public class TransactionPopup : MonoBehaviour {

        public TextMeshProUGUI changeText;
        public TextMeshProUGUI signText;
        public Image cashIcon;

        private float ticks = 0f;
        public float duration = 1f;

        public SizeAnimation anim;
        public Gradient colorGradient;

        void Awake() {
            Player.instance.wallet.onCashTransactionEvent.AddListener(PlayMessage);

            changeText.gameObject.SetActive(false);
            signText.gameObject.SetActive(false);
            cashIcon.gameObject.SetActive(false);

        }

        void FixedUpdate() {
            if (!changeText.gameObject.activeSelf) { return; }

            ticks += Time.fixedDeltaTime;

            if (ticks > duration) {
                changeText.gameObject.SetActive(false);
                signText.gameObject.SetActive(false);
                cashIcon.gameObject.SetActive(false);
                return;
            }

            changeText.color = colorGradient.Evaluate(ticks / duration);
            signText.color = colorGradient.Evaluate(ticks / duration);
            cashIcon.color = colorGradient.Evaluate(ticks / duration);

        }

        void PlayMessage(int oldValue, int change) {
            ticks = 0f;
            
            signText.text = change >= 0 ? "+" : "-";
            changeText.text = Mathf.Abs(change).ToString();
            
            changeText.gameObject.SetActive(true);
            signText.gameObject.SetActive(true);
            cashIcon.gameObject.SetActive(true);

            anim.Play();

        }

    }

}
