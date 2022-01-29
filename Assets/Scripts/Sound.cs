using UnityEngine.Audio;
using UnityEngine;


/// <summary>
///  <para> - string name </para>
///  <para> - AudioClip clip </para>
///  <para> - float volume </para>
///  <para> - float pitch </para>
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;
    
    // Clip audio
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

   
    // Vitesse du son
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    // AudioSource du son (caché dans l'inspector)
    [HideInInspector]
    public AudioSource source;


}