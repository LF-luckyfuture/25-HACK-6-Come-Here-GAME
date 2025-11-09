using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [Header("ÒôÆµËØ²Ä")]
    public AudioClip startBackgroundMusic;
    public AudioClip clickSFX;
    public AudioClip switchMachete;
    public AudioClip fistAttack;
    public AudioClip macheteAttack;
    public AudioClip getEXP;
    public AudioClip switchFist;
    public AudioClip move;
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
    public void FistAttack()
    {
        if (GlobalAudioManager.Instance != null && fistAttack != null)
        {
            GlobalAudioManager.Instance.PlayAudio(fistAttack, AudioType.SoundEffect);
        }
    }
    public void MacheteAttack()
    {
        if (GlobalAudioManager.Instance != null && macheteAttack != null)
        {
            GlobalAudioManager.Instance.PlayAudio(macheteAttack, AudioType.SoundEffect);
        }
    }
    public void MacheteSwitch()
    {
        if (GlobalAudioManager.Instance != null && switchMachete != null)
        {
            GlobalAudioManager.Instance.PlayAudio(switchMachete, AudioType.SoundEffect);
        }
    }
    public void FistSwitch()
    {
        if (GlobalAudioManager.Instance != null && switchFist != null)
        {
            GlobalAudioManager.Instance.PlayAudio(switchFist, AudioType.SoundEffect);
        }
    }
    public void GetEXP()
    {
        if (GlobalAudioManager.Instance != null && getEXP != null)
        {
            GlobalAudioManager.Instance.PlayAudio(getEXP, AudioType.SoundEffect);
        }
    }
    public void MoveSound()
    {
        if (GlobalAudioManager.Instance != null && move != null)
        {
            GlobalAudioManager.Instance.PlayAudio(move, AudioType.SoundEffect);
        }
    }
}
