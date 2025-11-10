using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioTest : MonoBehaviour
{
    public GameObject deadUI;
    [Header("“Ù∆µ")]
    public AudioClip clickSFX;
    public AudioClip switchMachete;
    public AudioClip fistAttack;
    public AudioClip macheteAttack;
    public AudioClip getEXP;
    public AudioClip getFragment;
    public AudioClip switchFist;
    public AudioClip move;
    public AudioClip gameBackgroundMusic;
    public AudioClip deadBackgroundMusic;
    private bool isDeadUIShow=false;
    private bool hasSwitchedToBgm=false;
    private void Start()
    {
        if (GlobalAudioManager.Instance != null && gameBackgroundMusic != null)
        {
            GlobalAudioManager.Instance.StopAllAudioByType(AudioType.BackgroundMusic);
            GlobalAudioManager.Instance.PlayAudio(gameBackgroundMusic, AudioType.BackgroundMusic, true);
        }
    }
    private void Update()
    {
        CheckDeadUIShow();
        if (isDeadUIShow&&Input.GetMouseButtonDown(0)&& GlobalAudioManager.Instance != null && clickSFX != null)
        {
            GlobalAudioManager.Instance.PlayAudio(clickSFX, AudioType.SoundEffect);
        }
    }
    private void CheckDeadUIShow()
    {
        if (deadUI!=null&&deadUI.activeInHierarchy&&!isDeadUIShow)
        {
            isDeadUIShow = true;
            SwitchToDeadBgm();
        }
        else if (deadUI!=null&&!deadUI.activeInHierarchy&&isDeadUIShow)
        {
            isDeadUIShow = false;
            hasSwitchedToBgm = false;
            GlobalAudioManager.Instance.StopAllAudioByType(AudioType.BackgroundMusic);
            GlobalAudioManager.Instance.PlayAudio(gameBackgroundMusic, AudioType.BackgroundMusic, true);
        }
    }
    private void SwitchToDeadBgm()
    {
        if (!hasSwitchedToBgm && GlobalAudioManager.Instance != null && deadBackgroundMusic != null)
        {
            GlobalAudioManager.Instance.StopAllAudioByType(AudioType.BackgroundMusic);
            GlobalAudioManager.Instance.PlayAudio(deadBackgroundMusic, AudioType.BackgroundMusic, true);
            hasSwitchedToBgm=true;
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
