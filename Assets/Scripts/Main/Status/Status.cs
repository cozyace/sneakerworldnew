// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the status data in a convenient class. 
    /// </summary>
    public class Status : PlayerSystem {

        // Triggers an event whenever the inventory changes.
        public UnityEvent<StatusData> onStatusChanged = new UnityEvent<StatusData>();


        // Implement the initialization from the player.
        protected override async Task TryInitialize() {
            StatusData state = await GetStatus();
            Debug.Log(state.username);
            onStatusChanged.Invoke(state);
        }

        // Get the status data.
        public async Task<StatusData> GetStatus() {
            StatusData status = await FirebaseManager.GetDatabaseValue<StatusData>(FirebasePath.Status);
            if (status == null) {
                status = new StatusData();
                await FirebaseManager.SetDatabaseValue<StatusData>(FirebasePath.Status, status);
            }
            return status;
        }

        // Set the username.
        [Button]
        public async Task SetUsername(string username) {
            StatusData status = await GetStatus();
            status.username = username;

            await FirebaseManager.SetDatabaseValue<StatusData>(FirebasePath.Status, status);
            onStatusChanged.Invoke(status);
        }

        // Set the username.
        [Button]
        public async Task AddExperience(int xp) {
            StatusData status = await GetStatus();

            status.xp += xp;
            int depth = 0;
            while (status.xp > GetExperienceForLevel(status.level) && depth < 50) {
                status.xp -= GetExperienceForLevel(status.level);
                status.level += 1;
                depth += 1;
            }

            await FirebaseManager.SetDatabaseValue<StatusData>(FirebasePath.Status, status);
            onStatusChanged.Invoke(status);
        }

        // Get experience for level.
        public static int GetExperienceForLevel(int level) {
            return 100 * level;
        }

        // public async void Deactivate() {
        //     await SetOnlineStatus(false);
        // }

        // public async Task SetLastLoggedOut() {
        //     DateTime currentTime = DateTime.Now;
        //     await FirebaseManager.SetDatabaseValue<DateTime>(FirebasePath.LastLoggedOut, currentTime);
        // }

        // public async Task SetOnlineStatus(bool isOnline) {
        //     await FirebaseManager.SetDatabaseValue<bool>(FirebasePath.IsOnline, isOnline);
        // }

    }

}
