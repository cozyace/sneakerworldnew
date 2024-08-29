// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.UI;
// TMP
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    public class UserItemSlot : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public string userId;

        public Button button;

        void Awake() {
            button.onClick.AddListener(() => Player.instance.friends.SendFriendRequest(userId));
        }

        public void Draw(KeyValuePair<string, (string, RelationshipStatus)> userData) {

            nameText.text = userData.Value.Item1;
            // if (r == Relation)

            userId = userData.Key;

        }


    }

}
