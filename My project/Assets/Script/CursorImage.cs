using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorImage : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
