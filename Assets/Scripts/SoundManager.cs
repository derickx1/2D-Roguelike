using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private float lowPitchRange = .95f;
	[SerializeField]
    private float highPitchRange = 1.05f;

    public AudioSource efxSource;
    public AudioSource musicSource;

    public void PlaySingle (AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play ();
    }

    public void RandomizeSfx (params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range (lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
