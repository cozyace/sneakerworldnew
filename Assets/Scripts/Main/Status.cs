// // System.
// using System;
// using System.Threading.Tasks;
// // Unity.
// using UnityEngine;

// namespace SneakerWorld.Main {

//     /// <summary>
//     /// Wraps the status data in a convenient class. 
//     /// </summary>
//     public class Status : MonoBehaviour {

//         [System.Serializable]
//         public class StatusData {
//             public int level;
//             public int xp;
//             public bool online;
//             public DateTime lastLoggedOut;
//         }

//         // The cached reference to the player.
//         private Player player;

//         // Initializes the user.
//         public async Task<bool> Initialize() {
//             this.player = player;
//             try {
//                 await SetOnlineStatus(true);
//                 return true;
//             }
//             catch (Exception exception) {
//                 Debug.Log(exception.Message);
//             }
//             return false;
//         }

//         public async void Deactivate() {
//             await SetOnlineStatus(false);
//         }

//         public async Task SetLastLoggedOut() {
//             DateTime currentTime = DateTime.Now;
//             await FirebaseManager.SetDatabaseValue<DateTime>(FirebasePath.LastLoggedOut, currentTime);
//         }

//         public async Task SetOnlineStatus(bool isOnline) {
//             await FirebaseManager.SetDatabaseValue<bool>(FirebasePath.IsOnline, isOnline);
//         }

//     }

// }
