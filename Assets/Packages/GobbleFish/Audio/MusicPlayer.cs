// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;

namespace GobbleFish.Audio {

    ///<summary>
    /// 
    ///<summary>
    public class MusicPlayer : MonoBehaviour {

        // The music to play.
        [SerializeField]
        private AudioClip m_MusicClip;
        
        // The delay after which it will begin playing.
        [SerializeField]
        private float m_Delay;

        public void PlayMusic() {
            Invoke("PlayMusicImmediate", m_Delay);
        }

        private void PlayMusicImmediate() {
            if (AudioManager.Instance == null) { return; }
            AudioManager.Music.Play(m_MusicClip);
        }

    }

}