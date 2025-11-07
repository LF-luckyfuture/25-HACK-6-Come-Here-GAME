using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistAttackTrigger : MonoBehaviour
{
    public Character character;
    private void Start()
    {
        character = FindObjectOfType<Character>();
        Destroy(gameObject, 0.1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            if (enemyController != null && character != null)
            {
                enemyController.TakeDamage(character.ATK * 0.8f);
            }
        }
    }
}
