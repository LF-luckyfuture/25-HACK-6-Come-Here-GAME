using UnityEngine;
using System.Collections.Generic;
public enum AudioType
{
    SoundEffect,
    BackgroundMusic
}

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }
    [Header("音量默认值（0-1）")]
    [SerializeField] private float defaultMasterVolume = 1f;
    [SerializeField] private float defaultSfxVolume = 1f;
    [SerializeField] private float defaultBgmVolume = 1f;
    private float _masterVolume;
    private float _sfxVolume;
    private float _bgmVolume;
    private Dictionary<AudioType, List<AudioSource>> _audioSourceDict = new();
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";
    private const string BGM_VOLUME_KEY = "BgmVolume";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAudioDict();
            LoadSavedVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitAudioDict()
    {
        _audioSourceDict.Add(AudioType.SoundEffect, new List<AudioSource>());
        _audioSourceDict.Add(AudioType.BackgroundMusic, new List<AudioSource>());
    }
    private void LoadSavedVolumes()
    {
        _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, defaultMasterVolume);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSfxVolume);
        _bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, defaultBgmVolume);
    }
    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _masterVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _sfxVolume);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, _bgmVolume);
        PlayerPrefs.Save();
    }
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        UpdateAllAudioVolumes();
        SaveVolumes();
    }
    public void SetSfxVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumesByType(AudioType.SoundEffect);
        SaveVolumes();
    }
    public void SetBgmVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);
        UpdateAudioVolumesByType(AudioType.BackgroundMusic);
        SaveVolumes();
    }
    public float GetMasterVolume() => _masterVolume;
    public float GetSfxVolume() => _sfxVolume;
    public float GetBgmVolume() => _bgmVolume;
    public void PlayAudio(AudioClip clip, AudioType audioType, bool isLoop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("音频片段为空，无法播放！");
            return;
        }
        GameObject audioObj = new GameObject($"Temp_{audioType}_Audio");
        audioObj.transform.parent = transform;
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = isLoop;
        audioSource.volume = CalculateVolume(audioType);
        audioSource.Play();
        _audioSourceDict[audioType].Add(audioSource);
        if (!isLoop)
        {
            Destroy(audioObj, clip.length);
        }
    }
    public void StopAllAudioByType(AudioType audioType)
    {
        foreach (var audioSource in _audioSourceDict[audioType])
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                Destroy(audioSource.gameObject);
            }
        }
        _audioSourceDict[audioType].Clear();
    }
    private float CalculateVolume(AudioType audioType)
    {
        return audioType switch
        {
            AudioType.SoundEffect => _masterVolume * _sfxVolume,
            AudioType.BackgroundMusic => _masterVolume * _bgmVolume,
            _ => _masterVolume
        };
    }
    private void UpdateAudioVolumesByType(AudioType audioType)
    {
        foreach (var audioSource in _audioSourceDict[audioType])
        {
            if (audioSource != null)
            {
                audioSource.volume = CalculateVolume(audioType);
            }
        }
    }
    private void UpdateAllAudioVolumes()
    {
        foreach (var type in _audioSourceDict.Keys)
        {
            UpdateAudioVolumesByType(type);
        }
    }
    private void LateUpdate()
    {
        foreach (var type in _audioSourceDict.Keys)
        {
            _audioSourceDict[type].RemoveAll(source => source == null);
        }
    }
}