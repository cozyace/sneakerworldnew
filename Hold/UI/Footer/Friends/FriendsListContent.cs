// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.UI;
//
using TMPro;

namespace SneakerWorld.UI {

    using Main;

    /// <summary>
    /// Functionality to control the friends. 
    /// </summary>
    public class FriendsListContent : MonoBehaviour {

        [System.Serializable]
        public class ContentSections {
            public GameObject itemSlotPrefab;
            public RectTransform contentSection;
        }

        // The system that this UI tracks.
        public FriendsList friendsList;
        public TMP_InputField inputField;

        // 
        public List<FriendData> incoming;
        public List<FriendData> outgoing;
        public List<FriendData> accepted;

        //
        public ContentSections friendsSection;
        public ContentSections requestsSection;
        public ContentSections usersSection;


        // Runs once before instantiation.
        public void Awake() {
            // Listen to the wallet events.
            friendsList.onInitEvent.AddListener(DrawFriends);
            friendsList.onUpdateFriends.AddListener(DrawFriends);
            friendsList.onSearchUsers.AddListener(DrawUsers);

            //
            inputField.onValueChanged.AddListener(friendsList.FilterFriendsByUsername);
            inputField.onEndEdit.AddListener(friendsList.FilterFriendsByUsername);
            inputField.onSelect.AddListener(friendsList.FilterFriendsByUsername);
            inputField.onDeselect.AddListener(friendsList.FilterFriendsByUsername);

        }

        void Start() {
            // friendsList.FilterFriendsByUsername(inputField.text);
            // friendsList.GetFriends();

            DrawUsers(new Dictionary<string, (string, RelationshipStatus)>());
        }

        // Draw the cash balance.
        public void DrawFriends(FriendsListData friends) {
            
            incoming = friends.all.FindAll(f => f != null && f.relation == RelationshipStatus.IncomingRequested);
            outgoing = friends.all.FindAll(f => f != null && f.relation == RelationshipStatus.OutgoingRequested);
            accepted = friends.all.FindAll(f => f != null && f.accepted);

            DrawContent(accepted, friendsSection.itemSlotPrefab, friendsSection.contentSection);

        }

        // Draw the cash balance.
        public void DrawUsers(Dictionary<string, (string, RelationshipStatus)> usernames) {
            foreach (Transform child in usersSection.contentSection.transform) {
                Destroy(child.gameObject);
            }
            
            foreach (var kv in usernames) {
                UserItem userItem = Instantiate(usersSection.itemSlotPrefab, usersSection.contentSection.transform).GetComponent<UserItem>();
                userItem.Draw(kv);
            }

        }

        public void DrawContent(List<FriendData> items, GameObject prefab, RectTransform contentSection) {
            if (!Application.isPlaying) { return; }

            foreach (Transform child in contentSection.transform) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < items.Count; i++) {
                DrawItemSlot(prefab, items[i], contentSection);
            }
        }

        public void DrawItemSlot(GameObject prefab, FriendData item, RectTransform contentSection) {
            FriendsItem friendsItem = Instantiate(prefab, contentSection.transform).GetComponent<FriendsItem>();
            friendsItem.Draw(item);
        }

    }

}
