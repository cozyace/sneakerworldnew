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

    public class ConnectionLoad : MonoBehaviour {

        public GameObject connectionError;

        public TextMeshProUGUI loadingText;
        public Image loadingBarFill;

        private int dotCount = 0;
        private float dotAddInterval = 0.5f;
        private float dotAddTicks = 0;

        void Awake() {
            Player.instance.onSystemLoaded.AddListener(DrawLoadingBar);
            Player.instance.onFailedToLoad.AddListener(DrawFailedToLoad);
            DrawLoadingBar(0f);
            connectionError.gameObject.SetActive(false);
        }

        void FixedUpdate() {
            dotAddTicks += Time.fixedDeltaTime;
            if (dotAddTicks > dotAddInterval) {
                dotCount = (dotCount + 1) % 4;
                loadingText.text = "LOADING";
                for (int i = 0; i < dotCount; i++) {
                    loadingText.text += ".";
                } 
                dotAddTicks -= dotAddInterval;
            }
        }

        void DrawLoadingBar(float percentLoaded) {
            loadingBarFill.fillAmount = percentLoaded;
            if (percentLoaded == 1f) {
                StartCoroutine("IEFinishLoading");
            }
        }

        void DrawFailedToLoad() {
            connectionError.gameObject.SetActive(true);
        }

        private IEnumerator IEFinishLoading() {
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }

    }

}
