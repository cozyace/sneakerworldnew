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

    public class FriendsItemSlot : MonoBehaviour {

        // The elements of this UI component.
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI lastOnlineText;
        public Image onlineImage;
        public Image profileImage;

        public void Draw(FriendData friendData) {

            nameText.text = friendData.userState.username;
            lastOnlineText.text = friendData.userState.lastLoggedOut.ToString();

            SetOnlinePanel(onlineImage, friendData.userState.online);
            // SetProfileImage(profileImage, friendData.userState);

        }

        public static void SetOnlinePanel(Image image, bool online) {
            image.sprite = Resources.Load<Sprite>($"Graphics/Main/Profile/{online.ToString()}");
        }

    }

}
