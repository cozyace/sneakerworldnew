// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the friend data in a convenient class. 
    /// </summary>
    public class Friend : MonoBehaviour {

        // The friends user ID.
        [SerializeField]
        private string friendId;

        public async Task<bool> GetOnlineStatus(string userID, bool isOnline) {
            return await FirebaseManager.GetDatabaseValue<bool>(userID, "isOnline");
        }

    }

}
