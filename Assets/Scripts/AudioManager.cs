using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource bgmSource;
    public float bgmVolume;
    public AudioSource sfxSource;
    public float sfxVolume;

    public float pitchVariation = 0.05f;

    public AudioClip[] volleyHitClips;
    public AudioClip[] volleySpikeClips;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        if (bgmSource != null)
        {
            SetBGMVolume(bgmVolume);
            bgmSource.loop = bgmSource.playOnAwake = true;
        }

        if (sfxSource != null)
        {
            SetSFXVolume(sfxVolume);
            sfxSource.loop = sfxSource.playOnAwake = false;
        }
	}

    public void PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    public void PlayRandomClipAtPoint(SoundType soundType, Vector3 position)
    {
        AudioClip clip = null;

        switch (soundType)
        {
            case SoundType.VolleyHit:
                clip = volleyHitClips[Random.Range(0, volleyHitClips.Length)];
                break;
            case SoundType.VolleySpike:
                clip = volleySpikeClips[Random.Range(0, volleySpikeClips.Length)];
                break;
        }

        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}

public enum SoundType
{
    VolleyHit,
    VolleySpike
}
