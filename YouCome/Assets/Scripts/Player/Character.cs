using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite frontImage;
    public Sprite backImage;
    public Sprite sideImage;
    public GameAudioTest audioTest;
    public GameObject fist;
    public GameObject machete;
    [Header(" Ù–‘")]
    [SerializeField]public float maxHealth;
    [SerializeField]public float currentHealth;
    [SerializeField]public float ATK;
    [SerializeField]public int playerLevel;
    [SerializeField]public int currentExp;
    public float moveSpeed;
    public float macheteAttackCooldown=1.25f;
    public float fistAttackCooldown=0.5f;
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
    private int maxLevel = 5;
    private float lastMacheteAttackTime;
    private float lastFistAttackTime;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    [SerializeField] public Slider HP;
    [SerializeField] public Slider EXP;
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
    public void MacheteAttack()
    {
        if (Input.GetMouseButtonDown(0)&&Time.time>=lastMacheteAttackTime+macheteAttackCooldown&&machete.activeInHierarchy)
        {
            lastMacheteAttackTime = Time.time;
            Instantiate(swordTrigger, swordTriPos.position, swordTriPos.rotation, swordTriPos);
            audioTest.MacheteAttack();
        }
    }
    public void FistAttack()
    {
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
        HP.value = (float)currentHealth / maxHealth;
    }
    public void UpdateEXPUI()
    {
        EXP.value = (float)currentExp / maxPlayerExp;
    }
}
