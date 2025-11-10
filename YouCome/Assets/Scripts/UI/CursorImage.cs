using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorImage : MonoBehaviour
{
    public Image cursorImage;
    void Update()
    {
        transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            cursorImage.color = Color.red;
            transform.localScale = new Vector3(2f, 2f, 2f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            cursorImage.color = Color.white;
            transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
        }
    }
}
