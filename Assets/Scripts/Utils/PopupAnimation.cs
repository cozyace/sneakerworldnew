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

    public class PopupAnimation : MonoBehaviour {

        private float ticks = 0f;
        public float duration = 1f;

        public AnimationCurve scaleCurve;
        private Vector3 localScale;

        void Awake() {
            localScale = transform.localScale;
        }

        void OnEnable() {
            ticks = 0f;
        }

        void FixedUpdate() {
            ticks += Time.fixedDeltaTime;
            if (ticks > duration) {
                return;
            }
            transform.localScale = localScale * scaleCurve.Evaluate(ticks / duration);
        }

    }

}
