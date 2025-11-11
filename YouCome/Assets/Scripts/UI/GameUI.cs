using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject settingUI;
    public GameObject musicUI;
    public GameObject gameUI;
    public SurvivalTime SurvivalTime;
    public Toggle autoAttack;
    public Character character;
    private void Start()
    {
        autoAttack.isOn = character.isAutoAttack;

    }
    public void ExitSetting()
    {
        settingUI.SetActive(false);
        musicUI.SetActive(false);
        gameUI.SetActive(false);
    }
    public void BackStart()
    {
        SceneManager.LoadScene(0);
    }
    public void Music()
    {
        musicUI.SetActive(true);
        gameUI.SetActive(false);
    }
    public void Game()
    {
        gameUI.SetActive(true);
        musicUI.SetActive(false);
    }
    public void OnToggleValueChnged(bool value)
    { 
        character.isAutoAttack = value;
    }
}
