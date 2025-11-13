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
    public float hitCounter;

    [Header("动画组件")]
    public Animator animator;
    public float attackRange = 1.5f;

    // 动画参数名称 - 确保所有敌人使用相同的参数名
    private const string IS_WALKING_PARAM = "IsWalking";
    private const string ATTACK_TRIGGER_PARAM = "Attack";

    private EnemyDrop enemyDrop;
    private bool isAttacking = false;

    void Start()
    {
        // 寻找玩家目标
        FindPlayerTarget();

        enemyHealth = 100f;
        enemyDrop = GetComponent<EnemyDrop>();

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
    }

    void Update()
    {
        // 如果目标丢失，重新寻找
        if (target == null)
        {
            FindPlayerTarget();
            return;
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

    void FindPlayerTarget()
    {
        // 方法1：通过Tag寻找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            Debug.Log("找到玩家目标: " + target.name);
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
            // 计算移动方向并应用速度
            Vector2 moveDirection = (target.position - transform.position).normalized;
            theRB.velocity = moveDirection * moveSpeed;

            // 可选：根据移动方向翻转敌人 sprite
            FlipSprite(moveDirection);
        }
    }

    void StopMoving()
    {
        theRB.velocity = Vector2.zero;
    }

    void FlipSprite(Vector2 direction)
    {
        // 如果敌人有SpriteRenderer，可以根据移动方向翻转
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                spriteRenderer.flipX = false; // 面向右
            }
            else if (direction.x < 0)
            {
                spriteRenderer.flipX = true; // 面向左
            }
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
                MoveTowardsTarget(); // 重要：立即开始移动！
            }
            else
            {
                // 玩家仍在攻击范围内，保持准备攻击状态
                SetWalkingAnimation(false);
                // 不调用StopMoving()，因为可能已经在移动
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
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
        }
    }
}