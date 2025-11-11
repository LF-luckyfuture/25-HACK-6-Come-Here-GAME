using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public GameObject settingUI;
    public GameObject producerUI;
    public GameObject musicUI;
    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
    public void SettingButton()
    {
        settingUI.SetActive(true);
    }
    public void ProducerButton()
    {
        producerUI.SetActive(true);
    }
    public void ExitSetting()
    {
        settingUI.SetActive(false);
        musicUI.SetActive(false);
    }
    public void ExitProducer()
    {
        producerUI.SetActive(false);
    }
    public void Music()
    {
        musicUI.SetActive(true);
    }
}
