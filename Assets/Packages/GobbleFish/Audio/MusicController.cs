// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
// GobbleFish.
using GobbleFish.Audio;

namespace GobbleFish.Audio {

    [System.Serializable]
    public class MusicController {

        // The audio source that plays the music.
        private AudioSource m_Source;

        // The name of this thing.
        private string name;

        // The transform to parent things to.
        private Transform transform;

        // Runs once before the first frame.
        public MusicController(string name, float volume, Transform transform) {
            this.name = name;
            this.transform = transform;
            m_Source = GenerateSource();
            SetVolume(volume);
        }

        // Generates the audio source to play the music from.
        private AudioSource GenerateSource() {
            AudioSource audioSource = new GameObject(name + " AudioSource", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.transform.SetParent(transform);
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.loop = true;
            audioSource.pitch = 1f;
            return audioSource;
        }

        public void Play(AudioClip audioClip) {
            m_Source.clip = audioClip;
            m_Source.PlayScheduled(UnityEngine.AudioSettings.dspTime);
        }

        public void Pause() {
            m_Source.Stop();
        }

        public void Stop() {
            m_Source.Stop();
        }

        public void SetVolume(float volume) {
            m_Source.volume = volume;
        }

    }
}