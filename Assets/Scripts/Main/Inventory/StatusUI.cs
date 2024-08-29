// System.
using System;
using System.Collections;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP.
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Listen to the wallet and draws it.
    /// </summary>
    public class StatusUI : MonoBehaviour {

        // The system that this UI tracks.
        public Status status;

        // The values.
        public StatusData state;

        // The ui components.
        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI levelText;
        public Image xpBar;


        // Runs once before instantiation.
        public void Awake() {
            status.onStatusChanged.AddListener(DrawState);
        }

        // Draw the cash balance.
        public void DrawState(StatusData state) {
            this.state = state;

            usernameText.text = state.username;
            levelText.text = state.level.ToString();
            xpBar.fillAmount = (float)state.xp / (float)Status.GetExperienceForLevel(state.level);

        }

    }

}
