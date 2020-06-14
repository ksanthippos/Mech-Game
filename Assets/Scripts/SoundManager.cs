using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
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

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    
    [SerializeField] 
    private Sound[] sounds;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            sounds[i].SetSource(go.AddComponent<AudioSource>());
        }
    }

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
