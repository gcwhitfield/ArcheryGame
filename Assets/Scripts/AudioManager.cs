using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    public AudioSource effectsSource;
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null)
        {
            effectsSource.clip = clip;
            effectsSource.Play();
        }
    }

    public void StopSoundEffect()
    {
        effectsSource.Stop();
    }

}
