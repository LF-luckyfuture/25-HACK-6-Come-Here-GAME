using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingController : MonoBehaviour
{
    public GameObject settingPanel;
    public GameUI gameUI;
    public bool isGamePaused = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingPanel();
        }
    }
    void ToggleSettingPanel()
    {
        isGamePaused = !isGamePaused;
        settingPanel.SetActive(true);
        Time.timeScale = isGamePaused ? 0f : 1f;
    }
    public void CloseSettingPanel()
    {
        isGamePaused=false;
        gameUI.ExitSetting();
        Time.timeScale = 1f;
    }
}
