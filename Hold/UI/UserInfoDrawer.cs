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
    using Utils;

    /// <summary>
    /// Listen to the wallet and draws it.
    /// </summary>
    public class UserInfoDrawer : MonoBehaviour {

        // The values.
        public StatusData state;
        public LerpInt xpValue;

        // The ui components.
        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI levelText;
        public Image xpBar;

        // Runs once before instantiation.
        public void Awake() {
            Player.instance.status.onStatusChanged.AddListener(DrawState);
        }

        // Runs once every fixed interval.
        void FixedUpdate() {
            xpBar.fillAmount = (float)xpValue.currValue / (float)Status.GetExperienceForLevel(state.level);
        }

        // Draw the cash balance.
        public void DrawState(StatusData state) {
            this.state = state;

            usernameText.text = state.username;
            levelText.text = state.level.ToString();
            xpValue.targetValue = state.xp; 

        }

    }

}
