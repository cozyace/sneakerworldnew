// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public abstract class CurveAnimation : MonoBehaviour {

        // time controls.
        protected float ticks = 0f;
        public float duration = 1f;

        // animation settings.
        public AnimationCurve curve;
        public bool loop;
        public bool playing;
        public bool playOnEnable;

        void OnEnable() {
            if (playOnEnable) {
                Play();
            }
        }

        public void Play() {
            ticks = 0f;
            playing = true;
        }

        public void Stop() {
            ticks = 0f;
            playing = false;
        }

        void FixedUpdate() {
            if (!playing) { return; }

            ticks += Time.fixedDeltaTime;
            if (ticks > duration && !loop) {
                playing = false;
                return;
            }
            else if (ticks > duration && loop) {
                ticks -= duration;
            }
            Animate();
        }

        protected abstract void Animate();


    }

}
