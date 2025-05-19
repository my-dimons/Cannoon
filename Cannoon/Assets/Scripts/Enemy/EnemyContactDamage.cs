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
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && notEnemy)
        {
            Debug.Log("Dealing damage to player");
            collision.transform.parent.parent.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        else if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && enemyScript.canDealDamage && !notEnemy)
        {
            Debug.Log("Dealing damage to player");
            enemyScript.player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
