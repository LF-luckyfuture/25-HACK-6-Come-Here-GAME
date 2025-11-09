using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumUIController : MonoBehaviour
{
    [Header("“Ù¡øª¨∂ØÃı")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider bgmvolumeSlider;
    private void Start()
    {
        if (GlobalAudioManager.Instance!=null)
        {
            masterVolumeSlider.value = GlobalAudioManager.Instance.GetMasterVolume();
            sfxVolumeSlider.value=GlobalAudioManager.Instance.GetSfxVolume();
            bgmvolumeSlider.value=GlobalAudioManager.Instance.GetBgmVolume();
            masterVolumeSlider.onValueChanged.AddListener(GlobalAudioManager.Instance.SetMasterVolume);
            sfxVolumeSlider.onValueChanged.AddListener(GlobalAudioManager.Instance.SetSfxVolume);
            bgmvolumeSlider.onValueChanged.AddListener(GlobalAudioManager.Instance.SetBgmVolume);
        }
    }
}
