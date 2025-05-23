using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour //FindObjectOfType<AudioManager>().Play("Audio");
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.sBlend;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " has no AudioSource assigned!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else if (s.source == null)
        {
            Debug.LogWarning("Sound: " + name + " has no AudioSource assigned!");
            return;
        }
        s.source.Stop();
    }
}
