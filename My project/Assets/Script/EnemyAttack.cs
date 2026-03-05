using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damageToPlayer = 1;
    public float attackCooldown = 1f;

    private float lastAttackTime = -999f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToPlayer);
                    lastAttackTime = Time.time;
                    Debug.Log("Player kena damage dari musuh!");
                }
            }
        }
    }
}
