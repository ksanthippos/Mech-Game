using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] audioSources;

    private AudioSource weaponChange;
    private AudioSource shieldsUp;
    private AudioSource shieldsDown;
    private AudioSource pauseSound;
    
    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
        weaponChange = audioSources[0];
        shieldsUp = audioSources[1];
        shieldsDown = audioSources[2];
        pauseSound = audioSources[3];
        
    }

    public void PlayWeaponsChange()
    {
        weaponChange.Play();
    }

    public void PlayShieldsUp()
    {
        shieldsUp.Play();
    }

    public void PlayShieldsDown()
    {
        shieldsDown.Play();
    }

    public void PlayPauseSound()
    {
        pauseSound.Play();
    }
}
