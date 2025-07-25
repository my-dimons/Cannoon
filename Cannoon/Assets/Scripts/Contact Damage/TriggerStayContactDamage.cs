using UnityEngine;

public class TriggerStayContactDamage : MonoBehaviour
{
    [Header("Stats")]
    public float damage;
    [Header("Enemy")]
    public bool notEnemy;
    [Header("Player")]
    public bool isPlayer;

    Enemy enemyScript;

    private void Start()
    {
        enemyScript = transform.parent.GetComponent<Enemy>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isPlayer && collision.gameObject.CompareTag("Enemy") && collision.GetComponent<Enemy>().canTakeDamage)
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
        }
        // This object is not an enemy (Projectile, etc.)
        if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && notEnemy)
        {
            collision.transform.parent.parent.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
        // This object is an enemy
        else if (collision.gameObject.CompareTag("PlayerEnemyCollisions") && !notEnemy && !isPlayer)
        {
            Debug.Log("Damaging Player (Can Deal Damage: " + enemyScript.canDealDamage + ")");
            if (enemyScript.canDealDamage)
                enemyScript.player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
