using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : UnitySingleton<AudioManager>
{
    public AudioSource effectsSource1;
    public AudioSource effectsSource2;
    public AudioSource effectsSource3;
    public AudioSource loopingSource;
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null)
        {
            List<AudioSource> sourceList = new List<AudioSource>();
            sourceList.Add(effectsSource1);
            sourceList.Add(effectsSource2);
            sourceList.Add(effectsSource3);
            foreach (AudioSource source in sourceList)
            {
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.Play();
                    break;
                }
            }
        }
    }

    public void StopSoundEffect()
    {
        effectsSource1.Stop();
        effectsSource2.Stop();
        effectsSource3.Stop();
    }

}
