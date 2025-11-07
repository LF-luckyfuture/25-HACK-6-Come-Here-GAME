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
    void Start()
    {
        target = FindObjectOfType<Character>().transform;
        enemyHealth = 100f;
        enemyDrop = GetComponent<EnemyDrop>();
    }
    void Update()
    {
        theRB.velocity = (target.position - transform.position).normalized * moveSpeed;

        if (hitCounter > 0f)
        {
            hitCounter -= Time.deltaTime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && hitCounter <= 0f)
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
}