// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace GobbleFish.Audio {

    [System.Serializable]
    public class AudioSettings : Settings<AudioSettings> {
        
        // Master Volume.
        public float masterVolume = 1f;
        public bool masterMuted = false;
        
        // Music Volume.
        public float musicVolume = 0.7f;
        public bool musicMuted = false;
        
        // Ambience Volume.
        public float ambienceVolume = 0.7f;
        public bool ambienceMuted;
        
        // Sound Volume.
        public float soundVolume = 0.7f;
        public bool soundMuted = false;
        
    }

}