using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;
    public Transform target;
    public float enemyHealth = 100f;

    public float damage;
    public float hitWaitTime = 1f;
    public Color hurtcolor = Color.red;
    public float flashDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 20f;
    public float hitCounter;

    [Header("动画组件")]
    public Animator animator;
    [Header("攻击范围设置")]
    public float attackRange = 3f; // 从1.5f增加到3f，扩大攻击范围

    // 动画参数名称
    private const string IS_WALKING_PARAM = "IsWalking";
    private const string ATTACK_TRIGGER_PARAM = "Attack";

    private EnemyDrop enemyDrop;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalPosition;
    private bool isFlashing = false;

    void Start()
    {
        // 寻找玩家目标
        FindPlayerTarget();

        enemyHealth = 100f;
        enemyDrop = GetComponent<EnemyDrop>();

        // 缓存SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 获取Animator组件
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator未找到！敌人: " + gameObject.name);
        }
    }

    void Update()
    {
        // 如果目标丢失，重新寻找
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }

        if (animator == null) return;

        // 如果正在攻击，不处理移动逻辑
        if (isAttacking)
        {
            return;
        }

        // 计算与玩家的距离
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            // 在攻击范围外 - 走路并追踪玩家
            MoveTowardsTarget();
            SetWalkingAnimation(true);
        }
        else
        {
            // 在攻击范围内 - 停止走路，准备攻击
            StopMoving();
            SetWalkingAnimation(false);

            // 检查攻击冷却
            if (hitCounter <= 0f)
            {
                AttackTarget();
                hitCounter = hitWaitTime;
            }
        }

        // 更新攻击冷却
        if (hitCounter > 0f)
        {
            hitCounter -= Time.deltaTime;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        originalPosition = transform.localPosition;
    }

    void FindPlayerTarget()
    {
        // 方法1：通过Tag寻找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            return;
        }

        // 方法2：通过Character组件寻找
        Character character = FindObjectOfType<Character>();
        if (character != null)
        {
            target = character.transform;
            return;
        }

        // 方法3：通过名称寻找
        player = GameObject.Find("Player");
        if (player != null)
        {
            target = player.transform;
            return;
        }

        Debug.LogWarning("未找到玩家目标！敌人将无法移动。");
    }

    void MoveTowardsTarget()
    {
        if (!isAttacking && target != null)
        {
            // 计算移动方向
            Vector2 moveDirection = (target.position - transform.position).normalized;

            // 应用速度
            theRB.velocity = moveDirection * moveSpeed;

            // 改进的翻转逻辑：基于目标位置
            FlipSpriteBasedOnTarget();
        }
    }

    void StopMoving()
    {
        if (theRB != null)
        {
            theRB.velocity = Vector2.zero;
        }
    }

    // 改进的翻转方法：基于目标位置而不是移动方向
    void FlipSpriteBasedOnTarget()
    {
        if (spriteRenderer != null && target != null)
        {
            // 直接根据玩家在敌人的左边还是右边来翻转
            bool shouldFlip = target.position.x < transform.position.x;
            spriteRenderer.flipX = shouldFlip;
        }
    }

    void SetWalkingAnimation(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool(IS_WALKING_PARAM, isWalking);
        }
    }

    void AttackTarget()
    {
        // 设置攻击状态
        isAttacking = true;
        StopMoving();
        SetWalkingAnimation(false);

        // 触发攻击动画
        animator.SetTrigger(ATTACK_TRIGGER_PARAM);

        // 对玩家造成伤害
        if (Character.instance != null)
        {
            Character.instance.TakeDamage(damage);
        }
    }

    // === 动画事件方法 ===

    public void OnAttackStart()
    {
        isAttacking = true;
        StopMoving();
        SetWalkingAnimation(false);
    }

    public void OnAttackEnd()
    {
        isAttacking = false;

        // 攻击结束后立即检查当前状态并恢复相应行为
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > attackRange)
            {
                // 玩家还在攻击范围外，立即开始追踪
                SetWalkingAnimation(true);
            }
            else
            {
                // 玩家仍在攻击范围内，保持准备攻击状态
                SetWalkingAnimation(false);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isFlashing) return;
        StartCoroutine(HurtEffectCoroutine());
        enemyHealth -= damageAmount;
        if (enemyHealth <= 0f)
        {
            if (enemyDrop != null)
            {
                enemyDrop.DropLoot();
            }
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator HurtEffectCoroutine()
    {
        isFlashing = true;
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            Color currentColor = Color.Lerp(hurtcolor, originalColor, elapsedTime / flashDuration);
            SetMonsterColor(currentColor);
            float shakeX = Mathf.Sin(elapsedTime * shakeFrequency) * shakeIntensity;
            float shakeY = Mathf.Cos(elapsedTime * shakeFrequency) * shakeIntensity;
            transform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetMonsterColor(originalColor);
        transform.localPosition = originalPosition;
        isFlashing = false;
    }

    private void SetMonsterColor(Color targetColor)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = targetColor;
    }
}