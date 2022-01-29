using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // tableau de sons
    public Sound[] sounds;
    public static AudioManager Instance;
    public AudioMixer audioMixer;
    public AudioMixerGroup masterGroup;

    void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = masterGroup;
        }
        
   
        if(Instance == null)
        {
            Instance = this;
        } else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }
    

    private Sound GetSoundFromAudioManager(string name) 
    { 
        // Recherche d'un son dans le tableau sounds grâce au nom 
        Sound s = Array.Find(sounds, sound => sound.name == name); // Lambda expression ( ... => ...) 
        return s; 
    } 
 
    public void Play(string name) 
    { 
        Sound s = GetSoundFromAudioManager(name); 
         
        // Lancement du son 
        s.source.Play(); 
    } 
 
    public void Stop(string name) 
    { 
        Sound s = GetSoundFromAudioManager(name); 
        
        // Arrêt du son 
        s.source.Stop(); 
    }

    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
    }
}