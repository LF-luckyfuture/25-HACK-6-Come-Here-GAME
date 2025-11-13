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

    private const string IS_WALKING_PARAM = "IsWalking";
    private const string ATTACK_TRIGGER_PARAM = "Attack";

    private EnemyDrop enemyDrop;
    private bool isAttacking = false;

    void Start()
    {
        // 确保能找到目标
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        enemyHealth = 100f;
        enemyDrop = GetComponent<EnemyDrop>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator未找到！");
            }
        }

        Debug.Log("EnemyController初始化完成，可以接收动画事件");
    }

    void Update()
    {
        if (target == null || animator == null) return;

        // 如果正在攻击，不处理移动逻辑
        if (isAttacking)
        {
            // 攻击时确保停止移动
            StopMoving();
            SetWalkingAnimation(false);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            // 在攻击范围外 - 走路
            MoveTowardsTarget();
            SetWalkingAnimation(true);
        }
        else
        {
            // 在攻击范围内 - 停止走路，准备攻击
            StopMoving();
            SetWalkingAnimation(false);

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

    void MoveTowardsTarget()
    {
        if (!isAttacking) // 只有不在攻击时才能移动
        {
            theRB.velocity = (target.position - transform.position).normalized * moveSpeed;
        }
    }

    void StopMoving()
    {
        theRB.velocity = Vector2.zero;
    }

    void SetWalkingAnimation(bool isWalking)
    {
        if (animator != null && !isAttacking) // 攻击时不设置走路状态
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

        Debug.Log("攻击触发！");
    }

    // === 动画事件方法 ===
    // 这些方法必须public才能被动画事件调用

    public void OnAttackStart()
    {
        Debug.Log("OnAttackStart事件被调用");
        isAttacking = true;
        StopMoving();
        SetWalkingAnimation(false);
    }

    public void OnAttackEnd()
    {
        Debug.Log("OnAttackEnd事件被调用");
        isAttacking = false;

        // 立即检查当前状态
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget > attackRange)
            {
                SetWalkingAnimation(true);
            }
        }
    }

    // 可选：添加一个更具体命名的方法
    public void OnAttackAnimationStart()
    {
        OnAttackStart();
    }

    public void OnAttackAnimationEnd()
    {
        OnAttackEnd();
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
}