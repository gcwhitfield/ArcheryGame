using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource effectsSource;
    public void PlaySoundEffect(AudioClip clip)
    {
        effectsSource.clip = clip;
        effectsSource.Play();
    }

}
