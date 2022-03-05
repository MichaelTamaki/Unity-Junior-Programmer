using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private AudioClip backgroundMenuClip;
    [SerializeField] private AudioClip backgroundPlayClip;
    [SerializeField] private AudioClip spawnBallClip;
    [SerializeField] private AudioClip effectKickClip;
    [SerializeField] private AudioClip missClip;
    [SerializeField] private AudioClip goalClip;

    private void Start()
    {
        SyncVolumeSliders();
    }

    public void PlayBackgroundMusic(bool isMenu)
    {
        backgroundSource.clip = isMenu ? backgroundMenuClip : backgroundPlayClip;
        backgroundSource.Play();
    }

    public void PauseBackgroundMusic()
    {
        backgroundSource.Pause();
    }

    public void UnPauseBackgroundMusic()
    {
        backgroundSource.UnPause();
    }

    private void PlayEffect(AudioClip clip)
    {
        effectSource.clip = clip;
        effectSource.Play();
    }
    public void PlaySpawnBallEffect()
    {
        PlayEffect(spawnBallClip);
    }

    public void PlayKickEffect()
    {
        PlayEffect(effectKickClip);
    }

    public void PlayMissEffect()
    {
        PlayEffect(missClip);
    }

    public void PlayGoalEffect()
    {
        PlayEffect(goalClip);
    }

    public void SyncVolumeSliders()
    {
        foreach (GameObject bgAudioObj in GameObject.FindGameObjectsWithTag("BGVolumeSliders"))
        {
            bgAudioObj.GetComponent<Slider>().value = backgroundSource.volume;
        }
        foreach (GameObject effectAudioObj in GameObject.FindGameObjectsWithTag("EffectVolumeSliders"))
        {
            effectAudioObj.GetComponent<Slider>().value = effectSource.volume;
        }
    }
}
