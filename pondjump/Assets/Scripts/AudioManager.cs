using UnityEngine.Audio;
using System;
using UnityEngine;


//do not change. you do not have to set up anything in the inspector anymore. 
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioSource playSound;
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }


    public void Play (string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Play();


    }

   void OnTriggerEnter(Collider other)
    {
        playSound.Play();
    }
}
