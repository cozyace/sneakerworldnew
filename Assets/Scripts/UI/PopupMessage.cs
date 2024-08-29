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

    public class PopupMessage : MonoBehaviour {

        public TextMeshProUGUI messageText;
        private float ticks = 0f;
        public float messageDuration = 1f;

        public AnimationCurve scaleCurve;
        private Vector3 localScale;

        public Gradient colorGradient;

        void Awake() {
            Player.instance.purchaser.onPurchaseFailedEvent.AddListener(PlayMessage);
            localScale = transform.localScale;
            messageText.gameObject.SetActive(false);
        }

        void FixedUpdate() {
            if (!messageText.gameObject.activeSelf) { return; }

            ticks += Time.fixedDeltaTime;

            if (ticks > messageDuration) {
                messageText.gameObject.SetActive(false);
                return;
            }

            transform.localScale = localScale * scaleCurve.Evaluate(ticks / messageDuration);
            messageText.color = colorGradient.Evaluate(ticks / messageDuration);

        }

        void PlayMessage(string message) {
            ticks = 0f;
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }

    }

}
