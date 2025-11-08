using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumController : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private Slider m_Slider;
    void Start()
    {
        m_AudioSource=GameObject.FindGameObjectWithTag("Volum").transform.GetComponent<AudioSource>();
        m_Slider=GetComponent<Slider>();
    }
    void Update()
    {
        VolumControl();
    }
    public void VolumControl()
    {
        m_AudioSource.volume=m_Slider.value;
    }
}
