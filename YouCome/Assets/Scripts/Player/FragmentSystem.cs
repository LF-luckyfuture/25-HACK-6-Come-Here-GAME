using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FragmentSystem : MonoBehaviour
{
    // [原有的变量声明保持不变]
    [Header("碎片概率")]
    [Range(0, 100)] public float planeFragmentProbability = 30;
    [Range(0, 100)] public float knifeFragmentProbability = 5;

    [Header("碎片获取")]
    [Range(0, 60)] public float fragmentInterval = 10;
    private float nextFragmentTime;

    [Header("拼图系统")]
    public int fragmentsPerPuzzle = 5;
    private int currentPlaneFragments = 0;
    private int currentKnifeFragments = 0;

    [Header("效果参数")]
    [Range(0, 20)] public float planeSpeedIncrease = 3.5f;
    [Range(0, 9)] public float knifeAttackIncrease = 2.5f;

    [Header("字体设置")]
    public TMP_FontAsset fallbackFont; // 备用字体

    // 引用
    public GameObject player;
    private Character characterScript;
    public GameObject deadUI;

    [Header("UI设置")]
    public GameObject puzzleUI;
    public GameObject puzzlePlaneUI;
    public TextMeshProUGUI fragmentCountText;
    public TextMeshProUGUI effectText;
    public TextMeshProUGUI planeFragmentCountText;
    public TextMeshProUGUI planeEffectText;
    public float uiDisplayTime = 5f;
    private bool isUIActive = false;
    private float uiCloseTime;

    // 预定义的文本映射，避免动态生成复杂字符串
    private Dictionary<string, string> textTemplates;

    void Start()
    {
        nextFragmentTime = Time.time + fragmentInterval;
        characterScript = player.GetComponent<Character>();

        // 初始化文本模板
        InitializeTextTemplates();

        // 确保UI初始隐藏
        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        // 检查并确保字体支持
        EnsureFontSupport();
    }

    void InitializeTextTemplates()
    {
        textTemplates = new Dictionary<string, string>
        {
            {"plane_fragment", "小飞机碎片"},
            {"knife_fragment", "朴刀碎片"},
            {"plane_puzzle", "小飞机拼图"},
            {"knife_puzzle", "朴刀拼图"},
            {"get_plane", "获得小飞机碎片！"},
            {"get_knife", "获得朴刀碎片！"},
            {"complete_plane", "拼图完成！速度大幅提升！"},
            {"complete_knife", "拼图完成！攻击力大幅提升！"},
            {"current_fragment", "当前碎片："},
            {"collect_progress", "收集进度："},
            {"no_fragment", "未获得碎片，再试试吧～"}
        };
    }

    void EnsureFontSupport()
    {
        // 设置备用字体
        if (fallbackFont != null)
        {
            // 获取所有TMP组件并设置备用字体
            TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var textComponent in textComponents)
            {
                // 如果当前字体不支持中文，使用备用字体
                if (textComponent.font != null && !IsFontSupportChinese(textComponent.font))
                {
                    textComponent.font = fallbackFont;
                }

                // 确保字体包含所需字符
                textComponent.ForceMeshUpdate();
            }
        }
    }

    bool IsFontSupportChinese(TMP_FontAsset font)
    {
        // 简单的检查：测试几个常用中文字符
        string testChars = "小飞机刀朴";
        foreach (char c in testChars)
        {
            if (!font.HasCharacter(c))
                return false;
        }
        return true;
    }

    void Update()
    {
        if (!deadUI.activeInHierarchy)
        {
            if (Time.time >= nextFragmentTime && !isUIActive)
            {
                TryGetFragment();
                nextFragmentTime = Time.time + fragmentInterval;
            }

            if (isUIActive)
            {
                if (Time.time >= uiCloseTime || Input.anyKeyDown)
                {
                    ClosePuzzleUI();
                    ClosePuzzlePlaneUI();
                }
            }
        }
    }

    void TryGetFragment()
    {
        float randomValue = Random.Range(0, 100);

        if (randomValue <= planeFragmentProbability)
        {
            currentPlaneFragments++;
            ShowPuzzlePlaneUI(textTemplates["plane_fragment"], currentPlaneFragments, textTemplates["get_plane"]);
            CheckPlanePuzzleCompletion();
        }
        else if (randomValue <= planeFragmentProbability + knifeFragmentProbability)
        {
            currentKnifeFragments++;
            ShowPuzzleUI(textTemplates["knife_fragment"], currentKnifeFragments, textTemplates["get_knife"]);
            CheckKnifePuzzleCompletion();
        }
    }

    void CheckPlanePuzzleCompletion()
    {
        if (currentPlaneFragments >= fragmentsPerPuzzle)
        {
            CompletePlanePuzzle();
        }
    }

    void CheckKnifePuzzleCompletion()
    {
        if (currentKnifeFragments >= fragmentsPerPuzzle)
        {
            CompleteKnifePuzzle();
        }
    }

    void CompletePlanePuzzle()
    {
        currentPlaneFragments = 0;
        characterScript.AddSpeed(planeSpeedIncrease);
        ShowPuzzlePlaneUI(textTemplates["plane_puzzle"], fragmentsPerPuzzle, textTemplates["complete_plane"]);
    }

    void CompleteKnifePuzzle()
    {
        currentKnifeFragments = 0;
        characterScript.AddAttack(knifeAttackIncrease);
        ShowPuzzleUI(textTemplates["knife_puzzle"], fragmentsPerPuzzle, textTemplates["complete_knife"]);
    }

    void ShowPuzzleUI(string fragmentType, int currentCount, string message)
    {
        if (puzzleUI != null)
        {
            Time.timeScale = 0;

            // 使用预定义的模板组合文本，避免动态字符串拼接
            fragmentCountText.text = textTemplates["collect_progress"] + currentCount + "/" + fragmentsPerPuzzle;
            effectText.text = message;

            // 强制更新文本网格
            fragmentCountText.ForceMeshUpdate();
            effectText.ForceMeshUpdate();

            puzzleUI.SetActive(true);
            isUIActive = true;
            uiCloseTime = Time.realtimeSinceStartup + uiDisplayTime;
        }
    }
    void ShowPuzzlePlaneUI(string fragmentType, int currentCount, string message)
    {
        if (puzzlePlaneUI != null)
        {
            Time.timeScale = 0;

            // 使用预定义的模板组合文本，避免动态字符串拼接
            planeFragmentCountText.text = textTemplates["collect_progress"] + currentCount + "/" + fragmentsPerPuzzle;
            planeEffectText.text = message;

            // 强制更新文本网格
            planeFragmentCountText.ForceMeshUpdate();
            planeEffectText.ForceMeshUpdate();

            puzzlePlaneUI.SetActive(true);
            isUIActive = true;
            uiCloseTime = Time.realtimeSinceStartup + uiDisplayTime;
        }
    }

    void ClosePuzzleUI()
    {
        if (puzzleUI != null)
        {
            Time.timeScale = 1;
            puzzleUI.SetActive(false);
            isUIActive = false;
        }
    }
    void ClosePuzzlePlaneUI()
    {
        if (puzzlePlaneUI != null)
        {
            Time.timeScale = 1;
            puzzlePlaneUI.SetActive(false);
            isUIActive = false;
        }
    }
}