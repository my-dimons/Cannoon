using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public bool notEnemy;

    Enemy enemyScript;

    private void Start()
    {
        enemyScript = transform.parent.GetComponent<Enemy>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // This object is an enemy
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && notEnemy)
        {
            collision.transform.parent.parent.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        // This object is not an enemy (Projectile, etc.)
        else if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && enemyScript.canDealDamage && !notEnemy)
        {
            enemyScript.player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
