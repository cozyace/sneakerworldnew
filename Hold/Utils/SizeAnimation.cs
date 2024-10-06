// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public class SizeAnimation : CurveAnimation {

        public float factor = 1f;
        private Vector3 localScale;

        void Awake() {
            localScale = transform.localScale;
        }

        protected override void Animate() {
            // transform.localScale = localScale * (1f + factor * curve.Evaluate(ticks / duration));
            transform.localScale = localScale * curve.Evaluate(ticks / duration);
        }

    }

}
