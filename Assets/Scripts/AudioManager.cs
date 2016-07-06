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
    public AudioClip[] humanGruntOKClips;
    public AudioClip[] positiveClips;
    public AudioClip[] wooshClips;
    public AudioClip[] timerClips;

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
        AudioSource.PlayClipAtPoint(GetClip(soundType), position, sfxVolume);
    }

    public void PlayOneShot(SoundType soundType)
    {
        sfxSource.pitch = RandomizePitch();
        sfxSource.PlayOneShot(GetClip(soundType), sfxVolume);
    }

    float RandomizePitch()
    {
        return Random.Range(sfxVolume - pitchVariation, sfxVolume + pitchVariation);
    }

    AudioClip GetClip(SoundType soundType)
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
            case SoundType.HumanGruntOk:
                clip = humanGruntOKClips[Random.Range(0, humanGruntOKClips.Length)];
                break;
            case SoundType.Positive:
                clip = positiveClips[Random.Range(0, positiveClips.Length)];
                break;
            case SoundType.Whoosh:
                clip = wooshClips[Random.Range(0, wooshClips.Length)];
                break;
            case SoundType.Timer:
                clip = timerClips[Random.Range(0, timerClips.Length)];
                break;
        }

        return clip;
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
    VolleySpike,
    HumanGruntOk,
    Whoosh,
    Positive,
    Timer
}
