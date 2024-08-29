// System.
using System;
using System.Threading.Tasks;
// Unity.
using UnityEngine;
// Sirenix.
using Sirenix.OdinInspector;

namespace SneakerWorld.Main {

    /// <summary>
    /// Wraps the status data in a convenient class. 
    /// </summary>
    [System.Serializable]
    public class StatusData {
        public string username = "";
        public int level = 0;
        public int xp = 0;
        public bool online = false;
        public DateTime lastLoggedOut = DateTime.Now;
    }

}
