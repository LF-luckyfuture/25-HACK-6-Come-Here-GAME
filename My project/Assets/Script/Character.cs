using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    [Header(" Ù–‘")]
    [SerializeField]protected float maxHealth;
    [SerializeField]protected float currentHealth;
    [SerializeField]protected float ATK;
    [SerializeField]public int playerLevel;
    [SerializeField]public int currentExp;
    public float moveSpeed;
    public int maxPlayerExp;
    private int maxLevel = 5;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Awake()
    {
        currentHealth = 100f;
        playerLevel = 1;
        currentExp = 0;
    }
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth<=0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        currentHealth = 0;
    }
    private void FixedUpdate()
    {
        ExpTest();
        PlayerValueTest();
        Move();
    }
    public void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.velocity = moveDirection * moveSpeed;
        if (moveDirection.x<0)
        {
            sr.flipX = true;
        }
        else if (moveDirection.x>0)
        {
            sr.flipX = false;
        }
    }
    public void ExpTest()
    {
        maxPlayerExp =(int)(50f * (1f + (playerLevel - 1f) * 0.5f) * (playerLevel + 1f));
        if (currentExp>=maxPlayerExp)
        {
            if (currentExp>maxPlayerExp)
            {
                if (playerLevel < maxLevel)
                {
                    currentExp = currentExp - maxPlayerExp;
                    playerLevel++;
                    currentHealth = maxHealth;
                }
                else
                {
                    currentExp = maxPlayerExp;
                }
            }
            else
            {
                if (playerLevel<maxLevel)
                {
                    currentExp = 0;
                    playerLevel++;
                    currentHealth = maxHealth;
                }
                else
                {
                    currentExp = maxPlayerExp;
                }
            }
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
    
}
