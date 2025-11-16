using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class Character : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite frontImage;
    public Sprite backImage;
    public Sprite sideImage;
    public GameAudioTest audioTest;
    public GameObject fist;
    public GameObject machete;

    [Header("属性")]
    [SerializeField]public float maxHealth;
    [SerializeField]public float currentHealth;
    [SerializeField]public float ATK;
    [SerializeField]public int playerLevel;
    [SerializeField]public int currentExp;
    public float moveSpeed;
    public float macheteAttackCooldown=0.5f;
    public float fistAttackCooldown=0.2f;
    public float regenRate=2f;
    public float regenInterval=1f;
    public int maxPlayerExp;
    public static Character instance;
    public GameObject swordTrigger;
    public Transform swordTriPos;
    public GameObject fistSwordTri;
    public Transform fistSwordTriPos;
    public GameObject deadUI;
    public GameObject deadVFX;
    public SurvivalTime survivalTime;
    public bool isDead=false;
    public bool isAutoAttack=false;
    private int maxLevel = 5;
    private float lastMacheteAttackTime;
    private float lastFistAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("UI组件")]
    [SerializeField] public Slider HP;
    [SerializeField] public Slider EXP;
    [SerializeField] public TextMeshProUGUI levelText; 
    [SerializeField] public TextMeshProUGUI expText; 
    [SerializeField] public GameObject levelUpEffect; 
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        StartCoroutine(RegenerateHealth());
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && frontImage != null)
        {
            spriteRenderer.sprite = frontImage;
        }

        UpdateLevelUI();
        UpdateEXPUI();
    }
    private void Awake()
    {
        playerLevel = 1;
        currentExp = 0;
        instance = this;
    }
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHPUI();
        if (currentHealth <= 0)
        {
            Die();
            survivalTime.StopTime();
        }
    }
    public virtual void Die()
    {
        Instantiate(deadVFX, transform.position, transform.rotation);
        gameObject.SetActive(false);
        isDead = true;
        currentHealth = 0;
        deadUI.SetActive(true);
    }
    private void Update()
    {
        if (!isDead)
        {
            ExpTest();
            PlayerValueTest();
            Move();
            FistAttack();
            MacheteAttack();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

    }
    public void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.velocity = moveDirection * moveSpeed;
        if (Mathf.Abs(moveY) > Mathf.Abs(moveX))
        {
            spriteRenderer.sprite = moveY > 0 ? backImage : frontImage;
        }
        else if (moveX != 0)
        {
            spriteRenderer.sprite = sideImage;
            if (moveDirection.x > 0)
            {
                sr.flipX = true;
            }
            else if (moveDirection.x < 0)
            {
                sr.flipX = false;
            }
        }
    }
    public void ExpTest()
    {
        maxPlayerExp = (int)(50f * (1f + (playerLevel - 1f) * 0.5f) * (playerLevel + 1f));

        // 添加调试信息
        Debug.Log($"当前经验: {currentExp}/{maxPlayerExp}, 等级: {playerLevel}");

        // 检查是否升级
        if (currentExp >= maxPlayerExp && playerLevel < maxLevel)
        {
            // 升级逻辑
            if (currentExp > maxPlayerExp)
            {
                currentExp = currentExp - maxPlayerExp;
            }
            else
            {
                currentExp = 0;
            }

            playerLevel++;
            currentHealth = maxHealth;

            // 调用升级方法
            OnLevelUp();

            // 更新UI
            UpdateEXPUI();
            UpdateLevelUI();

            Debug.Log($"升级！新等级: {playerLevel}");
        }
        else if (playerLevel >= maxLevel)
        {
            currentExp = maxPlayerExp;
            UpdateEXPUI();
        }
    }
    public void PlayerValueTest()
    {
        if (playerLevel==1)
        {
            maxHealth = 100f;
            ATK = 50f;
        }
        else
        {
            maxHealth = 50f + playerLevel * 50f;
            ATK = 40f + playerLevel * 10f;
        }
    }
    public void MacheteAttack()
    {
        if (isAutoAttack==true && Time.time >= lastMacheteAttackTime + macheteAttackCooldown && machete.activeInHierarchy)
        {
            lastMacheteAttackTime = Time.time;
            Instantiate(swordTrigger, swordTriPos.position, swordTriPos.rotation, swordTriPos);
            audioTest.MacheteAttack();
        }
        if (Input.GetMouseButtonDown(0)&&Time.time>=lastMacheteAttackTime+macheteAttackCooldown&&machete.activeInHierarchy)
        {
            lastMacheteAttackTime = Time.time;
            Instantiate(swordTrigger, swordTriPos.position, swordTriPos.rotation, swordTriPos);
            audioTest.MacheteAttack();
        }
    }
    public void FistAttack()
    {
        if (isAutoAttack == true && Time.time >= lastFistAttackTime + fistAttackCooldown && fist.activeInHierarchy)
        {
            lastFistAttackTime = Time.time;
            Instantiate(fistSwordTri, fistSwordTriPos.position, fistSwordTriPos.rotation, fistSwordTriPos);
            audioTest.FistAttack();
        }
        if (Input.GetMouseButtonDown(0)&&Time.time>=lastFistAttackTime+fistAttackCooldown&&fist.activeInHierarchy)
        {
            lastFistAttackTime = Time.time;
            Instantiate(fistSwordTri, fistSwordTriPos.position, fistSwordTriPos.rotation, fistSwordTriPos);
            audioTest.FistAttack();
        }
    }
    IEnumerator RegenerateHealth()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += regenRate;
                currentHealth=Mathf.Min(currentHealth, maxHealth);
                UpdateHPUI();
            }
            yield return new WaitForSeconds(regenInterval);
        }
    }
    public void UpdateHPUI()
    {
        if (HP != null)
            HP.value = (float)currentHealth / maxHealth;
    }

    public void UpdateEXPUI()
    {
        if (EXP != null)
            EXP.value = (float)currentExp / maxPlayerExp;

        if (expText != null)
            expText.text = $"{currentExp}/{maxPlayerExp}";
    }

    // 新增：更新等级UI
    public void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Lv.{playerLevel}";
    }

    // 新增：升级时触发
    private void OnLevelUp()
    {
        Debug.Log($"升级了！当前等级: {playerLevel}");

        // 播放升级特效
        if (levelUpEffect != null)
        {
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        }

        // 播放升级音效（如果有）
        // audioTest.PlayLevelUpSound();

        // 更新属性
        PlayerValueTest();
        UpdateHPUI();

        // 可以在这里添加其他升级效果
        StartCoroutine(ShowLevelUpMessage());
    }

    // 显示升级消息（可选）
    private IEnumerator ShowLevelUpMessage()
    {
        // 可以创建一个临时的UI文本来显示升级信息
        Debug.Log($"恭喜！升级到 {playerLevel} 级！");
        yield return new WaitForSeconds(2f);
    }

    // 新增：添加经验值的方法（从敌人脚本调用）
    public void AddExp(int expAmount)
    {
        currentExp += expAmount;
        Debug.Log($"获得 {expAmount} 经验值，当前经验: {currentExp}/{maxPlayerExp}");
        UpdateEXPUI();

        // 立即检查是否可以升级
        ExpTest();
    }

    public void AddAttack(float percentage)
    {
        float maxAttack = 1.09f * (40f + playerLevel * 10f);
        ATK = Mathf.Min(ATK + percentage, maxAttack);
    }

    public void AddSpeed(float bonusPercentage)
    {
        float maxMoveSpeed = 1.2f * moveSpeed;
        float bonusVaule = moveSpeed * (bonusPercentage / 100);
        moveSpeed = Mathf.Min(moveSpeed + bonusVaule, maxMoveSpeed);
    }
}
