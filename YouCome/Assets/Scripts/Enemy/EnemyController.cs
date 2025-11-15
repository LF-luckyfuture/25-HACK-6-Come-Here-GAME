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
    public Color hurtcolor= Color.red;
    public float flashDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 20f;
    public float hitCounter;

    [Header("动画组件")]
    public Animator animator;
    public float attackRange = 1.5f;

    // 动画参数名称
    private const string IS_WALKING_PARAM = "IsWalking";
    private const string ATTACK_TRIGGER_PARAM = "Attack";

    private EnemyDrop enemyDrop;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer; // 缓存SpriteRenderer
    private Color Color;
    private Vector3 position;
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
        else
        {
            Debug.Log("EnemyController初始化完成 - 敌人: " + gameObject.name);
        }

        // 检查Rigidbody2D约束
        if (theRB != null)
        {
            Debug.Log("Rigidbody2D约束 - FreezeRotation: " + theRB.freezeRotation +
                     ", FreezePositionX: " + theRB.constraints.HasFlag(RigidbodyConstraints2D.FreezePositionX) +
                     ", FreezePositionY: " + theRB.constraints.HasFlag(RigidbodyConstraints2D.FreezePositionY));
        }
    }

    void Update()
    {
        // 如果目标丢失，重新寻找
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return; // 如果还是找不到，直接返回
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
        spriteRenderer=GetComponent<SpriteRenderer>();
        Color = spriteRenderer.color;
        position = transform.localPosition;
    }

    void FixedUpdate()
    {
        // 在FixedUpdate中处理物理移动，确保平滑
        // 移动逻辑现在主要在Update中处理，这里只处理翻转
    }

    void FindPlayerTarget()
    {
        // 方法1：通过Tag寻找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("找到玩家目标: " + target.name + " 位置: " + target.position);
            return;
        }

        // 方法2：通过Character组件寻找
        Character character = FindObjectOfType<Character>();
        if (character != null)
        {
            target = character.transform;
            Debug.Log("通过Character组件找到玩家目标");
            return;
        }

        // 方法3：通过名称寻找
        player = GameObject.Find("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("通过名称找到玩家目标");
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

            // 调试移动方向
            Debug.Log("移动方向: " + moveDirection + " 目标位置: " + target.position + " 敌人位置: " + transform.position);

            // 应用速度
            theRB.velocity = moveDirection * moveSpeed;

            // 根据移动方向翻转敌人 sprite
            FlipSprite(moveDirection);
        }
    }

    void StopMoving()
    {
        if (theRB != null)
        {
            theRB.velocity = Vector2.zero;
        }
    }

    void FlipSprite(Vector2 direction)
    {
        // 使用缓存的SpriteRenderer
        if (spriteRenderer != null)
        {
            // 只有当X方向有显著移动时才翻转，避免抖动
            if (Mathf.Abs(direction.x) > 0.1f) // 添加阈值避免微小移动导致的抖动
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            // 如果没有SpriteRenderer，尝试获取一次
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            Debug.Log(gameObject.name + " 对玩家造成 " + damage + " 点伤害");
        }
    }

    // === 动画事件方法 ===

    public void OnAttackStart()
    {
        Debug.Log(gameObject.name + " 攻击开始");
        isAttacking = true;
        StopMoving();
        SetWalkingAnimation(false);
    }

    public void OnAttackEnd()
    {
        Debug.Log(gameObject.name + " 攻击结束");
        isAttacking = false;

        // 攻击结束后立即检查当前状态并恢复相应行为
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > attackRange)
            {
                // 玩家还在攻击范围外，立即开始追踪
                SetWalkingAnimation(true);
                // 不需要调用MoveTowardsTarget()，因为Update会处理
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

    // 调试可视化
    private void OnDrawGizmosSelected()
    {
        // 攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 移动方向指示
        if (Application.isPlaying && target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);

            // 显示当前速度方向
            if (theRB != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, theRB.velocity.normalized * 1f);
            }
        }
    }
    private System.Collections.IEnumerator HurtEffectCoroutine()
    {
        isFlashing = true;
        float elapsedTime = 0f;
        while (elapsedTime<flashDuration)
        {
            Color currentColor = Color.Lerp(hurtcolor, Color, elapsedTime / flashDuration);
            SetMonsterColor(currentColor);
            float shakeX=Mathf.Sin(elapsedTime*shakeFrequency)*shakeIntensity;
            float shakeY=Mathf.Cos(elapsedTime*shakeFrequency)*shakeIntensity;
            transform.localPosition = position + new Vector3(shakeX, shakeY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetMonsterColor(Color);
        transform.localPosition = position;
        isFlashing=false;
    }
    private void SetMonsterColor(Color targetColor)
    {
        if (spriteRenderer!=null)
         spriteRenderer.color = targetColor;
    }
}