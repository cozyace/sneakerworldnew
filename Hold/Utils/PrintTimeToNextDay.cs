// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// TMP.
using TMPro;

namespace SneakerWorld.Utils {

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PrintTimeToNextDay : MonoBehaviour {

        private TextMeshProUGUI text;

        void Awake() {
            text = GetComponent<TextMeshProUGUI>();
        }

        void FixedUpdate() {
            int day = DateTime.Now.Day; int month = DateTime.Now.Month; int year = DateTime.Now.Year;
            DateTime tomorrow = new DateTime(year, month, day+1);
            TimeSpan timeSpan = tomorrow.Subtract(DateTime.UtcNow);
            text.text = timeSpan.ToString(@"hh\:mm\:ss") + " Left";
        }

    }

}
