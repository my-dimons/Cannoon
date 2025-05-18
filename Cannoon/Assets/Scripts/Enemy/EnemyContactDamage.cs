using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Stats")]
    public int damage;

    Enemy enemyScript;

    private void Start()
    {
        enemyScript = transform.parent.GetComponent<Enemy>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && enemyScript.canDealDamage)
        {
            Debug.Log("Dealing damage to player");
            enemyScript.player.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
}
