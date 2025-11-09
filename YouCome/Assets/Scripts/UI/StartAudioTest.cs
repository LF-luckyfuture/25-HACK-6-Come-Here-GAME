using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAudioTest : MonoBehaviour
{
    [Header("ÒôÆµËØ²Ä")]
    public AudioClip startBackgroundMusic;
    public AudioClip clickSFX;
    private void Start()
    {
        PlayBackgroundMusic();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClickSound();
        }
    }
    private void PlayBackgroundMusic()
    {
        if (GlobalAudioManager.Instance!=null&&startBackgroundMusic!=null)
        {
            GlobalAudioManager.Instance.PlayAudio(startBackgroundMusic, AudioType.BackgroundMusic, true);
        }
    }
    private void PlayClickSound()
    {
        if (GlobalAudioManager.Instance!=null&&clickSFX!=null)
        {
            GlobalAudioManager.Instance.PlayAudio(clickSFX, AudioType.SoundEffect);
        }
    }
}
