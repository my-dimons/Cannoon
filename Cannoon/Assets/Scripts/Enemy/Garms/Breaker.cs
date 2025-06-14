using System.Collections;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    public float attackingCooldown;
    public bool canAttack;
    public GameObject sprite;
    [Header("Throwing")]
    public float spikeSpeed;
    public float minThrowingDistance;
    public AnimationClip throwingAnimation;
    public GameObject spike;
    public Vector2 spikeSpawningPosition;

    [Header("Attacking")]
    public float attackingDistance;
    public AnimationClip attackingAnimation;
    public AnimationClip groundSpikeAnimation;
    public GameObject groundSpikes;
    public float groundSpikeSpawningPositionX;

    FollowEnemyAI enemyAiScript;
    Enemy enemyScript;
    GameObject player;
    Animator animator;
    bool isAttacking;

    [Header("Audio")]
    public AudioClip attackingSound;
    public AudioClip throwingSound;

    [Header("DEBUGGING")]
    public bool visualizeGroundSpikeRaycast;
    public bool debugLogPlayerDistance;
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        enemyAiScript = GetComponent<FollowEnemyAI>();
        animator = enemyScript.animator;
        player = enemyScript.player;
     
        // a short cooldown when the enemy spawns so it cant attack instantly
        StartCoroutine(AttackCooldown(enemyScript.spawningAnimation.length + attackingCooldown));
    }

    IEnumerator AttackCooldown(float time)
    {
        canAttack = false;
        yield return new WaitForSeconds(time);
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < attackingDistance && canAttack && !isAttacking)
        {
            StartCoroutine(GroundSpikeAttack());
        }
        if (Vector2.Distance(player.transform.position, transform.position) > minThrowingDistance && canAttack && !isAttacking)
        {
            StartCoroutine(ThrowingSpikeAttack());
        }

        // DEBUGGING

        if (visualizeGroundSpikeRaycast)
        {
            Debug.DrawRay(new Vector2(transform.position.x + groundSpikeSpawningPositionX - groundSpikes.GetComponent<BoxCollider2D>().size.x * 1.5f, transform.position.y),
                Vector2.down * 2, Color.magenta);
            Debug.DrawRay(new Vector2(transform.position.x - groundSpikeSpawningPositionX + groundSpikes.GetComponent<BoxCollider2D>().size.x * 1.5f, transform.position.y),
                Vector2.down * 2, Color.blue);
        }
        if (debugLogPlayerDistance)
            Debug.Log(Vector2.Distance(player.transform.position, transform.position));
    }

    IEnumerator GroundSpikeAttack()
    {
        Vector3 spawningPosition;
        // Spawns the attack object a few seconds early
        float earlyTime = 0.25f;
        // adjusts the high of the spikes so they are level
        float spikeSpawningYPos = 4.5f;
        // how far the raycast is send downward
        float depth = 2f;


        if ((Physics2D.Raycast(new Vector2(transform.position.x + groundSpikeSpawningPositionX - groundSpikes.GetComponent<BoxCollider2D>().size.x * 1.5f, transform.position.y), 
            Vector2.down, depth, enemyAiScript.layerMask) && !enemyScript.facingRight) 
            || 
            (Physics2D.Raycast(new Vector2(transform.position.x - groundSpikeSpawningPositionX + groundSpikes.GetComponent<BoxCollider2D>().size.x * 1.5f, transform.position.y),
            Vector2.down, depth, enemyAiScript.layerMask) && enemyScript.facingRight))
        {
            canAttack = false;
            enemyScript.canTurn = false;
            isAttacking = true;
            StartCoroutine(enemyScript.FreezeEnemy(attackingAnimation.length + groundSpikeAnimation.length));
            animator.SetBool("isAttacking", true);

            yield return new WaitForSeconds(attackingAnimation.length - earlyTime);

            animator.SetBool("isAttacking", false);

            if (enemyScript.facingRight)
                spawningPosition = new Vector3(transform.position.x - groundSpikeSpawningPositionX, transform.position.y + spikeSpawningYPos, 0);
            else
                spawningPosition = new Vector3(transform.position.x + groundSpikeSpawningPositionX, transform.position.y + spikeSpawningYPos, 0);

            GameObject spawnedSpike = Instantiate(groundSpikes, spawningPosition, groundSpikes.transform.rotation);
            spawnedSpike.GetComponent<DestroyAfterTime>().destroyTime = groundSpikeAnimation.length - earlyTime;
            spawnedSpike.transform.GetChild(0).transform.localScale = enemyAiScript.enemySprite.localScale;

            // SFX
            enemyScript.enemyAudio.PlayOneShot(attackingSound, 1f * enemyScript.gameManager.soundVolume);

            isAttacking = false;
            enemyScript.canTurn = true;

            yield return new WaitForSeconds(groundSpikeAnimation.length - earlyTime);

            StartCoroutine(AttackCooldown(attackingCooldown));
        } else
        {
            StartCoroutine(ThrowingSpikeAttack());
        }
    }

    IEnumerator ThrowingSpikeAttack()
    {
        Vector3 spawningPosition;
        canAttack = false;
        isAttacking = true;
        enemyScript.canTurn = false;
        StartCoroutine(enemyScript.FreezeEnemy(attackingAnimation.length));
        animator.SetBool("isThrowing", true);
        if (enemyScript.facingRight)
        {
            spawningPosition = new Vector3(
                transform.position.x - spikeSpawningPosition.x,
                transform.position.y + spikeSpawningPosition.y,
                0);
        } else
        {
            spawningPosition = new Vector3(
                transform.position.x + spikeSpawningPosition.x,
                transform.position.y + spikeSpawningPosition.y,
                0);
        }

        GameObject spawnedSpike = Instantiate(spike, spawningPosition, spike.transform.rotation);
        enemyScript.enemyAudio.PlayOneShot(throwingSound, 1f * enemyScript.gameManager.soundVolume);
        spawnedSpike.GetComponent<Bullet>().sprite.transform.localScale = enemyAiScript.enemySprite.localScale;

        yield return new WaitForSeconds(throwingAnimation.length / 2);

        animator.SetBool("isThrowing", false);
        if (enemyScript.facingRight)
            spawnedSpike.GetComponent<Bullet>().speed = 25;
        else
            spawnedSpike.GetComponent<Bullet>().speed = -25;

        yield return new WaitForSeconds(throwingAnimation.length / 2);

        isAttacking = false;
        enemyScript.canTurn = true;

        StartCoroutine(AttackCooldown(attackingCooldown));
    }
}
