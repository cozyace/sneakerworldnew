// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public class RotationAnimation : CurveAnimation {

        public float factor = 360f;
        private Quaternion localRotation;

        void Awake() {
            localRotation = transform.localRotation;
        }

        protected override void Animate() {
            transform.localRotation = Quaternion.Euler(0f, 0f, factor * curve.Evaluate(ticks / duration)) * localRotation;
        }

    }

}
