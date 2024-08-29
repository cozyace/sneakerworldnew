// System.
using System;
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    ///
    public class FriendsListData {
        public List<FriendData> all = new List<FriendData>();

        public void Add(FriendData f) {
            all.Add(f);
        }

        public void Update() {
            incoming = all.FindAll(f => f.relation == RelationshipStatus.IncomingRequested);
            outgoing = all.FindAll(f => f.relation == RelationshipStatus.OutgoingRequested);
            accepted = all.FindAll(f => f.accepted);
        }

        List<FriendData> incoming = new List<FriendData>();
        List<FriendData> outgoing = new List<FriendData>();
        List<FriendData> accepted = new List<FriendData>();

    }

    /// <summary>
    /// Wraps the friend data in a convenient class. 
    /// </summary>
    [System.Serializable]
    public class FriendData {

        public string uid;
        public RelationshipStatus relation;
        public StatusData userState;

        public bool accepted => 
            relation == RelationshipStatus.IncomingAccepted 
            || relation == RelationshipStatus.OutgoingAccepted;

        // public Status userState;
        public FriendData(string uid, RelationshipStatus relation) {
            this.uid = uid;
            this.relation = relation;
            this.userState = userState;
        }
    }

}
