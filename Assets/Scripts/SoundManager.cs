using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this "helper" class collects individual audioclips and adds source for them
// soundmanager class also access play funcion via this class
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]    // min max values to slider
    public float volume;
    
    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        source.volume = volume;
        source.Play();
    }
}

// manages all Sound-class instances and plays them by name (string parameter)
public class SoundManager : MonoBehaviour
{

    // is accessible from all scripts
    public static SoundManager instance;
    
    [SerializeField] 
    private Sound[] sounds;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // collect all sounds to array
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

    // find a sound by name 
    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        
        // no sound found
        Debug.LogWarning("No sound found: " + _name);
    } 
}
