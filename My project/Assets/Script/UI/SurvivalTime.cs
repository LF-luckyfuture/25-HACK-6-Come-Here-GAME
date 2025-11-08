using UnityEngine;
using UnityEngine.UI;
public class SurvivalTime : MonoBehaviour
{
    public Text survivalTimeText;
    private float startTime;
    private bool isTimeRunning=true;
    void Start()
    {
        startTime=Time.time;
    }
    void Update()
    {
        if (isTimeRunning)
        {
            UpdateTime();
        }
    }
    void UpdateTime()
    {
        float currentTime = Time.time-startTime;
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        survivalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void OnPlayerDeath()
    {
        isTimeRunning = false;
    }
}
