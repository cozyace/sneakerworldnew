// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;

namespace SneakerWorld.Utils {

    public class LerpInt : MonoBehaviour {

        // lerp params.
        private int baseValue;
        private int deltaValue;
        private float ticks = 0f;
        public float duration = 1f;

        // target.
        private int _targetValue;
        public int targetValue {
            set {
                if (_targetValue != value) {
                    ticks = 0f;
                    baseValue = _currValue;
                    deltaValue = value - _currValue;
                    _targetValue = value;
                } 
            }
        }

        // current.
        private int _currValue;
        public int currValue {
            get {
                return _currValue;
            }
            set {
                ticks = duration;
                _currValue = value;
                _targetValue = value;
            }
        }

        void FixedUpdate() {
            if (ticks < duration) {
                ticks += Time.fixedDeltaTime;
                int currDelta = (int)(Mathf.Round((float)deltaValue * ticks / duration));
                _currValue = baseValue + currDelta;
            }
            else {
                _currValue = _targetValue;
            }

        }

    }

}
