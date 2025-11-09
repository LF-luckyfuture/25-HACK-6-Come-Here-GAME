using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject settingUI;
    public GameObject musicUI;
    public SurvivalTime SurvivalTime;
    public void ExitSetting()
    {
        settingUI.SetActive(false);
        musicUI.SetActive(false);
    }
    public void BackStart()
    {
        SceneManager.LoadScene(0);
    }
    public void Music()
    {
        musicUI.SetActive(true);
    }
}
