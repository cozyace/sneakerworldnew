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

    public class PopupCash : MonoBehaviour {

        public TextMeshProUGUI changeText;
        public TextMeshProUGUI signText;
        public TextMeshProUGUI valueText;
        public Image cashIcon;

        private float ticks = 0f;
        public float duration = 1f;

        public float radius = 50f;

        public AnimationCurve scaleCurve;
        private Vector3 localScale;
        Vector3 origin;

        public Gradient colorGradient;

        private int originalVal;
        private int totalChange;

        void Awake() {
            Player.instance.wallet.onTransactionEvent.AddListener(PlayMessage);
            localScale = transform.localScale;
            origin = transform.localPosition;

            changeText.gameObject.SetActive(false);
            signText.gameObject.SetActive(false);
            valueText.gameObject.SetActive(false);
            cashIcon.gameObject.SetActive(false);

        }

        void FixedUpdate() {
            if (!changeText.gameObject.activeSelf) { return; }

            ticks += Time.fixedDeltaTime;

            if (ticks > duration) {
                valueText.gameObject.SetActive(false);
                changeText.gameObject.SetActive(false);
                signText.gameObject.SetActive(false);
                cashIcon.gameObject.SetActive(false);
                return;
            }

            // float halfDur = 2f * ticks / duration;
            // halfDur = Mathf.Clamp(halfDur, 0f, 1f);

            // int currChange = (int)(Mathf.Floor(halfDur * totalChange));
            // valueText.text = (originalVal + currChange).ToString();
            // changeText.text = (Mathf.Abs(totalChange - currChange)).ToString();

            transform.localScale = localScale * scaleCurve.Evaluate(ticks / duration);
            changeText.color = colorGradient.Evaluate(ticks / duration);
            valueText.color = colorGradient.Evaluate(ticks / duration);
            signText.color = colorGradient.Evaluate(ticks / duration);
            cashIcon.color = colorGradient.Evaluate(ticks / duration);

        }

        void PlayMessage(int oldValue, int change) {
            ticks = 0f;
            signText.text = change >= 0 ? "+" : "-";
            changeText.text = Mathf.Abs(change).ToString();
            valueText.text = oldValue.ToString();
            
            valueText.gameObject.SetActive(false);
            changeText.gameObject.SetActive(true);
            signText.gameObject.SetActive(true);
            cashIcon.gameObject.SetActive(true);

            originalVal = oldValue;
            totalChange = change;

            transform.localPosition = origin + (Vector3)UnityEngine.Random.insideUnitCircle * radius;
        }

    }

}
