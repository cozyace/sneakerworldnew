// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public class VerticalSlideAnimation : MonoBehaviour {

        // Animating the opening and closing of the panel.
        private bool open;

        // Position animation.
        public AnimationCurve openPosCurve;
        public float posYFactor = 1600f;
        Vector3 localPosition;

        // Scale animation.
        public AnimationCurve openScaleCurve;
        public float scaleYFactor = 0.01f;
        Vector3 localScale;

        // Time controls.
        float ticks = 0f;
        public float duration = 0.5f;

        void Awake() {
            localPosition = transform.localPosition;
            localScale = transform.localScale;
        }

        void Update() {
            float factor = open ? 1f : -1f;
            
            ticks += factor * Time.deltaTime;
            if (ticks >= duration || ticks <= 0f) {
                ticks = Mathf.Clamp(ticks, 0f, duration);
                return;
            }

            transform.localPosition = localPosition + posYFactor * Vector3.up * openPosCurve.Evaluate(ticks/duration);
            transform.localScale = localScale + scaleYFactor * Vector3.up * openScaleCurve.Evaluate(ticks/duration);

        }

        public void Open(bool open) {
            this.open = open;
            if (open && !gameObject.activeSelf) { 
                gameObject.SetActive(true);
            }
        }


    }

}
