using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParallaxEffect : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform rectTransform;
        public float moveSpeed = 1f;
        public Vector2 maxOffset = new Vector2(100, 50);
        [HideInInspector] public Vector2 startPosition;
    }

    public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
    public float inputSensitivity = 0.1f;

    private Vector2 mouseReferencePos;

    void Start()
    {
        // 记录各层初始位置
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer.rectTransform != null)
            {
                layer.startPosition = layer.rectTransform.anchoredPosition;
            }
        }

        // 初始化鼠标位置
        mouseReferencePos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    void Update()
    {
        // 获取鼠标输入（支持新旧输入系统）
#if ENABLE_INPUT_SYSTEM
        Vector2 currentMousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#else
        Vector2 currentMousePos = Input.mousePosition;
#endif

        // 计算鼠标偏移（归一化到[-1,1]）
        Vector2 mouseOffset = new Vector2(
            (currentMousePos.x - mouseReferencePos.x) / Screen.width * 2f,
            (currentMousePos.y - mouseReferencePos.y) / Screen.height * 2f
        );

        mouseOffset *= inputSensitivity;

        // 应用视差移动
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            if (layer.rectTransform != null)
            {
                Vector2 targetOffset = new Vector2(
                    mouseOffset.x * layer.maxOffset.x * layer.moveSpeed,
                    mouseOffset.y * layer.maxOffset.y * layer.moveSpeed
                );

                layer.rectTransform.anchoredPosition = layer.startPosition + targetOffset;
            }
        }
    }

    // 供外部调用来切换交互模式
    public void SetInputSensitivity(float sensitivity)
    {
        inputSensitivity = Mathf.Clamp(sensitivity, 0.1f, 1f);
    }
}