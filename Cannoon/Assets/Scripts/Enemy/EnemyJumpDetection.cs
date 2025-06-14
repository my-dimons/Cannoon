using System.Collections;
using UnityEngine;

public class EnemyJumpDetection : MonoBehaviour
{
    public GameObject enemy;
    Enemy enemyScript;
    FollowEnemyAI enemyFollowAi;

    [Header("Landing")]
    [Tooltip("How much the enemy is slowed upon landing (speed/var)")]
    public float landingSpeedDiviser;
    [Tooltip("How long the enemy is slowed upon landing (in seconds)")]
    public float landingTime;
    IEnumerator SlowEnemyOnLanding()
    {
        enemyScript.speed = enemyScript.baseSpeed / landingSpeedDiviser;
        yield return new WaitForSeconds(landingTime);
        enemyScript.speed = enemyScript.baseSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = enemy.GetComponent<Enemy>();
        enemyFollowAi = enemy.GetComponent<FollowEnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // player falls on the ground
            if (!enemyScript.onGround)
            {
                StartCoroutine(SlowEnemyOnLanding());
                enemyScript.onGround = true;
                enemyScript.animator.SetBool("isJumping", !enemyScript.onGround);
            }
            // reset players jump when hitting the ground
            if (!enemyScript.canJump)
            {
                enemyScript.canJump = true;
            }

            enemyScript.onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            enemyScript.onGround = false;
            enemyScript.canJump = false;
            enemyScript.animator.SetBool("isJumping", !enemyScript.onGround);
        }
    }
}
