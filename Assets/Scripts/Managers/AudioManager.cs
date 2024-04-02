using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;
    
    [Header("Tracks")]
    [SerializeField] private AudioClip mainTrack;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private AudioClip notification;

    private void Start()
    {
        musicSource.clip = mainTrack;
        musicSource.Play();
    }

    public void ButtonClick()
    {
        soundSource.PlayOneShot(buttonClick);
    }
}
