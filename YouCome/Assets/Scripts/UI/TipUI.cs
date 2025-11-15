using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipUI : MonoBehaviour
{
    [Header("ͼƬ")]
    public Image[] images=new Image[4];
    private int currentIndex = 0;
    private void Start()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(i == 0);
        }
        Time.timeScale = 0;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&&currentIndex<images.Length)
        {
            images[currentIndex].gameObject.SetActive(false);
            currentIndex++;
            if (currentIndex<images.Length)
            {
                images[currentIndex].gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}
