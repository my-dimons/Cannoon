using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    public float attackingCooldown;
    public GameObject sprite;
    [Header("Throwing")]
    public float spikeSpeed;
    public float minThrowingDistance;
    public bool canThrow;
    public AnimationClip throwingAnimation;
    public GameObject spike;
    public Vector2 spikeSpawningPosition;

    [Header("Attacking")]
    public float attackingDistance;
    public bool canAttack;
    public AnimationClip attackingAnimation;
    public AnimationClip groundSpikeAnimation;
    public GameObject groundSpikes;
    public float groundSpikeSpawningPositionX;

    FollowEnemyAI enemyAiScript;
    Enemy enemyScript;
    GameObject player;
    Animator animator;
    bool isAttacking;
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        enemyAiScript = GetComponent<FollowEnemyAI>();
        animator = enemyAiScript.animator;
        player = enemyScript.player;
     
        // a short cooldown when the enemy spawns so it cant attack instantly
        StartCoroutine(AttackCooldown(enemyAiScript.spawningAnimation.length));
    }

    IEnumerator AttackCooldown(float time)
    {
        canThrow = false;
        canAttack = false;
        yield return new WaitForSeconds(time);
        canThrow = true;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < attackingDistance && canAttack && !isAttacking)
        {
            StartCoroutine(GroundSpikeAttack());
        }
        if (Vector2.Distance(player.transform.position, transform.position) > minThrowingDistance && canThrow && !isAttacking)
        {
            StartCoroutine(ThrowingSpikeAttack());
        }
    }

    IEnumerator GroundSpikeAttack()
    {
        Vector3 spawningPosition;
        // Spawns the attack object a few seconds early
        float earlyTime = 0.25f;
        // adjusts the high of the spikes so they are level
        float spikeSpawningYPos = 4.5f;
        canAttack = false;
        canThrow = false;
        isAttacking = true;
        StartCoroutine(enemyAiScript.FreezeEnemy(attackingAnimation.length + groundSpikeAnimation.length));
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackingAnimation.length - earlyTime);

        animator.SetBool("isAttacking", false);

        if (enemyAiScript.facingRight)
            spawningPosition = new Vector3(transform.position.x - groundSpikeSpawningPositionX, transform.position.y + spikeSpawningYPos, 0);
        else
            spawningPosition = new Vector3(transform.position.x + groundSpikeSpawningPositionX, transform.position.y + spikeSpawningYPos, 0);

        GameObject spawnedSpike = Instantiate(groundSpikes, spawningPosition, groundSpikes.transform.rotation);
        spawnedSpike.GetComponent<DestroyAfterTime>().destroyTime = groundSpikeAnimation.length - earlyTime;
        spawnedSpike.transform.GetChild(0).transform.localScale = enemyAiScript.enemySprite.localScale;

        isAttacking = false;

        yield return new WaitForSeconds(groundSpikeAnimation.length - earlyTime);

        StartCoroutine(AttackCooldown(attackingCooldown));
    }

    IEnumerator ThrowingSpikeAttack()
    {
        Vector3 spawningPosition;
        canThrow = false;
        canAttack = false;
        isAttacking = true;
        StartCoroutine(enemyAiScript.FreezeEnemy(attackingAnimation.length));
        animator.SetBool("isThrowing", true);
        if (enemyAiScript.facingRight)
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
        spawnedSpike.GetComponent<Bullet>().sprite.transform.localScale = enemyAiScript.enemySprite.localScale;

        yield return new WaitForSeconds(throwingAnimation.length / 2);

        animator.SetBool("isThrowing", false);
        if (enemyAiScript.facingRight)
            spawnedSpike.GetComponent<Bullet>().speed = 25;
        else
            spawnedSpike.GetComponent<Bullet>().speed = -25;


        yield return new WaitForSeconds(throwingAnimation.length / 2);

        isAttacking = false;

        StartCoroutine(AttackCooldown(attackingCooldown));
    }
}
