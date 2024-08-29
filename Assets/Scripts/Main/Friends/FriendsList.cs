// System.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// Functionality to control the friends. 
    /// </summary>
    public class FriendsList : PlayerSystem {

        // An event to trigger when this system has been initialized.
        public UnityEvent<FriendsListData> onInitEvent = new UnityEvent<FriendsListData>();

        // An event to trigger when accepting a friend request.
        public UnityEvent<FriendsListData> onUpdateFriends = new UnityEvent<FriendsListData>();

        // An event to trigger when accepting a friend request.
        public UnityEvent<Dictionary<string, (string, RelationshipStatus)>> onSearchUsers = new UnityEvent<Dictionary<string, (string, RelationshipStatus)>>();

        // An event to trigger when sending a friend request.
        public UnityEvent<FriendData> onSendRequest = new UnityEvent<FriendData>();

        // An event to trigger when accepting a friend request.
        public UnityEvent<FriendData> onAcceptRequest = new UnityEvent<FriendData>();


        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            FriendsListData friends = await GetFriends();
            onInitEvent.Invoke(friends);
        }

        // Get the most updated friends list.
        public async Task<FriendsListData> GetFriends() {
            FriendsListData friendsList = await FirebaseManager.GetDatabaseValue<FriendsListData>(FirebasePath.FriendsList);
            if (friendsList == null) {
                friendsList = new FriendsListData();
            }
            onUpdateFriends.Invoke(friendsList);
            return friendsList;
        }

        // Get the a friends list from a friend.
        public async Task<FriendsListData> GetFriendsFrom(string friendId) {
            FriendsListData theirFriends = await FirebaseManager.GetDatabaseValue<FriendsListData>(FirebasePath.FriendsFriendListWithId(friendId));
            if (theirFriends == null) {
                theirFriends = new FriendsListData();
            }
            return theirFriends;
        }

        public async Task SetFriends(FriendsListData friends) {
            await FirebaseManager.SetDatabaseValue<FriendsListData>(FirebasePath.FriendsList, friends);
        }

        public async Task SetFriendsFor(string friendId, FriendsListData theirFriends) {
            await FirebaseManager.SetDatabaseValue<FriendsListData>(FirebasePath.FriendsFriendListWithId(friendId), theirFriends);
        }

        public void FilterFriendsByUsername(string inputUsername) {
            _FilterUsersByUsername(inputUsername);
        }

        [Button] // really don't like this.
        public async Task _FilterUsersByUsername(string inputUsername) {

            // Get a list of all the users.
            Firebase.Database.DataSnapshot users = await FirebaseManager.Instance.databaseRoot.Child("users").GetValueAsync();
            Dictionary<string, (string, RelationshipStatus)> usernames = new Dictionary<string, (string, RelationshipStatus)>();

            foreach (Firebase.Database.DataSnapshot child in users.Children) { 

                if (child.Key != Player.instance.id && child.HasChild("status") && child.Child("status").HasChild("username")) {
                    string childUserName = (string)child.Child("status").Child("username").GetValue(true);
                    Debug.Log(childUserName);

                    if (childUserName.Contains(inputUsername) || childUserName == inputUsername) {

                        Debug.Log(childUserName);
                        Debug.Log(child.Key);

                        RelationshipStatus r = RelationshipStatus.Count;

                        FriendsListData theirFriends = await GetFriendsFrom(child.Key);
                        FriendData us = theirFriends.all.Find(f => f.uid == player.id);
                        if (us != null) {
                            r = us.relation;
                        }

                        usernames.Add(child.Key, (childUserName, r));

                    }
                }
            }

            onSearchUsers.Invoke(usernames);

        }
        

        // Update a specific friends state.
        [Button]
        public async Task UpdateFriendState(string friendId) {
            FriendsListData friends = await GetFriends();
            FriendData friend = friends.all.Find(f => f.uid == friendId);
            if (friend != null && friend.accepted) {
                friend.userState = await FirebaseManager.GetDatabaseValue<StatusData>(FirebasePath.FriendStatusWithId(friendId));
            }
            await SetFriends(friends);

            // Trigger an event.
            onUpdateFriends.Invoke(friends);
        }

        // Update all yours friends state.
        [Button]
        public async Task UpdateAllFriendStates(string friendId) {
            FriendsListData friends = await GetFriends();

            List<FriendData> acceptedFriends = friends.all.FindAll(f => f != null && f.accepted);
            foreach (FriendData friend in acceptedFriends) {
                friend.userState = await FirebaseManager.GetDatabaseValue<StatusData>(FirebasePath.FriendStatusWithId(friendId));
            }
            await SetFriends(friends);

            // Trigger an event.
            onUpdateFriends.Invoke(friends);

        }

        // Checks whether a user with the given id exists.
        public async Task<bool> CheckUserExists(string userId) {
            StatusData userState = await FirebaseManager.GetDatabaseValue<StatusData>(FirebasePath.FriendStatusWithId(userId));
            if (userState == null) {
                Debug.Log($"User with id {userId} does not exist");
                return false;
            }
            return true;
        }

        public void SendFriendRequest(string friendId) {
            Debug.Log(friendId);
            _SendFriendRequest(friendId);
        }

        // Send a friend request.
        [Button]
        public async Task _SendFriendRequest(string friendId) {

            // Make sure the user exists.
            bool friendExists = await CheckUserExists(friendId);
            if (!friendExists) {
                Debug.Log("friend did not exist");
                return;
            }

            // Add an outgoing friend request to that friend.
            FriendsListData friends = await GetFriends();
            Debug.Log($"Sending friend request to : {friendId}");
            
            FriendData friend = friends.all.Find(f => f.uid == friendId);
            if (friend == null) {
                friend = new FriendData(friendId, RelationshipStatus.OutgoingRequested);
                friends.Add(friend);
            }


            // Add an incoming friend request.
            FriendsListData theirFriends = await GetFriendsFrom(friendId);

            FriendData us = theirFriends.all.Find(f => f.uid == player.id);
            if (us == null) {
                us = new FriendData(player.id, RelationshipStatus.IncomingRequested);
                theirFriends.Add(us);
            }

            //
            await SetFriends(friends);
            await SetFriendsFor(friendId, theirFriends);
            Debug.Log($"Successfully sent friend request to : {friendId}");

            // Trigger an event.
            onUpdateFriends.Invoke(friends);

        }

        // Accept a friend request.
        [Button]
        public async Task AcceptFriendRequest(string friendId) {

            // Check there is an incoming request.
            FriendsListData friends = await GetFriends();
            Debug.Log($"Checking for an incoming request from : {friendId}");

            List<FriendData> incomingRequests = friends.all.FindAll(f => f.relation == RelationshipStatus.IncomingRequested);
            FriendData friend = incomingRequests.Find(f => f.uid == friendId);

            // Check that there has been an outgoing request.
            FriendsListData theirFriends = await GetFriendsFrom(friendId);

            List<FriendData> outgoingRequests = theirFriends.all.FindAll(f => f.relation == RelationshipStatus.OutgoingRequested);
            FriendData me = incomingRequests.Find(f => f.uid == player.id);

            // If both.
            if (friend == null || me != null) {
                Debug.Log($"No valid request from : {friendId}");
                return;
            }

            me.relation = RelationshipStatus.IncomingAccepted;
            friend.relation = RelationshipStatus.OutgoingAccepted;
            
            await SetFriends(friends);
            await SetFriendsFor(friendId, theirFriends);
            Debug.Log($"Successfully accepted friend request from : {friendId}");

            // Trigger an event.
            onUpdateFriends.Invoke(friends);

        }

    }

}
