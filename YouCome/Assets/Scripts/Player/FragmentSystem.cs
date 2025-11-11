using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FragmentSystem : MonoBehaviour
{
    // 碎片概率（分开设置）
    [Header("碎片概率")]
    [Range(0, 100)] public float planeFragmentProbability = 30; // 小飞机碎片概率
    [Range(0, 100)] public float knifeFragmentProbability = 5; // 朴刀碎片概率

    // 碎片获取间隔
    [Header("碎片获取")]
    [Range(0, 60)] public float fragmentInterval = 10;
    private float nextFragmentTime;

    // 拼图系统（分开统计两种碎片）
    [Header("拼图系统")]
    public int fragmentsPerPuzzle = 5; // 每种碎片需收集的数量
    private int currentPlaneFragments = 0; // 小飞机碎片当前数量
    private int currentKnifeFragments = 0; // 朴刀碎片当前数量

    // 效果参数（分开设置）
    [Header("效果参数")]
    [Range(0, 20)] public float planeSpeedIncrease = 3.5f; // 小飞机速度提升百分比
    [Range(0, 9)] public float knifeAttackIncrease = 2.5f; // 朴刀攻击力提升数值

    // 引用
    public GameObject player;
    private Character characterScript;

    // UI相关
    [Header("UI设置")]
    public GameObject puzzleUI;
    public TextMeshProUGUI fragmentTypeText; // 显示获得的碎片类型
    public TextMeshProUGUI fragmentCountText; // 显示当前碎片进度
    public TextMeshProUGUI effectText; // 显示效果提示
    public float uiDisplayTime = 5f;
    private bool isUIActive = false;
    private float uiCloseTime;

    void Start()
    {
        nextFragmentTime = Time.time + fragmentInterval;
        characterScript = player.GetComponent<Character>(); // 缓存Character脚本

        // 确保UI初始隐藏
        if (puzzleUI != null)
            puzzleUI.SetActive(false);
    }

    void Update()
    {
        // 检查是否可获取碎片（非UI激活状态）
        if (Time.time >= nextFragmentTime && !isUIActive)
        {
            TryGetFragment();
            nextFragmentTime = Time.time + fragmentInterval;
        }

        // 处理UI关闭（计时或按任意键）
        if (isUIActive)
        {
            if (Time.time >= uiCloseTime || Input.anyKeyDown)
            {
                ClosePuzzleUI();
            }
        }
    }

    // 尝试获取碎片（随机获取一种或不获取）
    void TryGetFragment()
    {
        float randomValue = Random.Range(0, 100);

        // 优先判断小飞机碎片
        if (randomValue <= planeFragmentProbability)
        {
            currentPlaneFragments++;
            ShowPuzzleUI("小飞机碎片", currentPlaneFragments, "获得小飞机碎片！");
            Debug.Log("获得小飞机碎片！当前进度: " + currentPlaneFragments + "/" + fragmentsPerPuzzle);
            CheckPlanePuzzleCompletion(); // 检查小飞机碎片是否集齐
        }
        // 再判断朴刀碎片
        else if (randomValue <= planeFragmentProbability + knifeFragmentProbability)
        {
            currentKnifeFragments++;
            ShowPuzzleUI("朴刀碎片", currentKnifeFragments, "获得朴刀碎片！");
            Debug.Log("获得朴刀碎片！当前进度: " + currentKnifeFragments + "/" + fragmentsPerPuzzle);
            CheckKnifePuzzleCompletion(); // 检查朴刀碎片是否集齐
        }
        else
        {
            // 未获得任何碎片（可选：不显示UI，或显示提示）
            Debug.Log("未获得碎片，再试试吧～");
        }
    }

    // 检查小飞机碎片是否集齐
    void CheckPlanePuzzleCompletion()
    {
        if (currentPlaneFragments >= fragmentsPerPuzzle)
        {
            CompletePlanePuzzle();
        }
    }

    // 检查朴刀碎片是否集齐
    void CheckKnifePuzzleCompletion()
    {
        if (currentKnifeFragments >= fragmentsPerPuzzle)
        {
            CompleteKnifePuzzle();
        }
    }

    // 小飞机拼图完成，触发速度效果
    void CompletePlanePuzzle()
    {
        currentPlaneFragments = 0; // 重置碎片计数
        characterScript.AddSpeed(planeSpeedIncrease); // 仅触发小飞机效果
        ShowPuzzleUI("小飞机拼图", fragmentsPerPuzzle, "拼图完成！速度大幅提升！");
        Debug.Log("小飞机拼图完成！已获得速度提升效果");
    }

    // 朴刀拼图完成，触发攻击力效果
    void CompleteKnifePuzzle()
    {
        currentKnifeFragments = 0; // 重置碎片计数
        characterScript.AddAttack(knifeAttackIncrease); // 仅触发朴刀效果
        ShowPuzzleUI("朴刀拼图", fragmentsPerPuzzle, "拼图完成！攻击力大幅提升！");
        Debug.Log("朴刀拼图完成！已获得攻击力提升效果");
    }

    // 显示拼图UI（带碎片类型、进度、提示）
    void ShowPuzzleUI(string fragmentType, int currentCount, string message)
    {
        if (puzzleUI != null)
        {
            Time.timeScale = 0; // 暂停游戏

            // 更新UI内容
            fragmentTypeText.text = "当前碎片：" + fragmentType;
            fragmentCountText.text = "收集进度：" + currentCount + "/" + fragmentsPerPuzzle;
            effectText.text = message;

            // 显示UI
            puzzleUI.SetActive(true);
            isUIActive = true;
            uiCloseTime = Time.realtimeSinceStartup + uiDisplayTime;
        }
    }

    // 关闭拼图UI，恢复游戏
    void ClosePuzzleUI()
    {
        if (puzzleUI != null)
        {
            Time.timeScale = 1; //恢复游戏
            puzzleUI.SetActive(false);
            isUIActive = false;
        }
    }
}