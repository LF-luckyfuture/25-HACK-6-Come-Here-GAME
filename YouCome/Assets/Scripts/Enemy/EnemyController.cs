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

    private EnemyDrop enemyDrop;

    private Animator animator;
    public float attackRange = 1.5f;

    private const string IS_WALKING_PARAM = "IsWalking";
    private const string IS_ATTACKING_PARAM = "IsAttacking";

    private const string ATTACK_TRIGGER_PARAM = "Attack";

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Character>().transform;
        enemyHealth = 100f;
        enemyDrop = GetComponent<EnemyDrop>();

        // 获取Animator组件
        animator = GetComponent<Animator>();

        // 确保Animator存在
        if (animator == null)
        {
            Debug.LogError("Animator组件未找到！请确保敌人对象上有Animator组件。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = (target.position - transform.position).normalized * moveSpeed;
        // 计算与目标的距离
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // 根据距离决定行为
        if (distanceToTarget > attackRange)
        {
            // 在攻击范围外，走向目标
            MoveTowardsTarget();
            SetWalkingAnimation(true);
        }
        else
        {
            // 在攻击范围内，停止移动
            StopMoving();
            SetWalkingAnimation(false);

            // 处理攻击冷却
            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
            else
            {
                // 攻击玩家并触发攻击动画
                AttackTarget();
                hitCounter = hitWaitTime;
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            Character.instance.TakeDamage(damage);

            hitCounter = hitWaitTime;
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

    void MoveTowardsTarget()
    {
        theRB.velocity = (target.position - transform.position).normalized * moveSpeed;
    }

    // 停止移动
    void StopMoving()
    {
        theRB.velocity = Vector2.zero;
    }

    // 设置走路动画状态
    void SetWalkingAnimation(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool(IS_WALKING_PARAM, isWalking);
        }
    }

    // 攻击目标
    void AttackTarget()
    {
        // 这里可以添加攻击逻辑，比如播放攻击音效等
        Character.instance.TakeDamage(damage);
    }

    void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(ATTACK_TRIGGER_PARAM);
        }
    }

}
