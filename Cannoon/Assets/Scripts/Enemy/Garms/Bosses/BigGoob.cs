using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class BigGoob : MonoBehaviour
{
    public bool transformed;
    public float attackCooldown;
    public bool canAttack;

    [Header("Spike Attack")]
    public GameObject groundSpikes;
    public float groundSpikesYSpawnPos;
    public float groundPoundAnimDelay;

    [Header("Stat Increase")]
    public float healthPercent;
    public float speedPercent;
    public float cooldownPercent;

    [Header("Animations")]
    public AnimationClip groundSpikesAnim;
    public AnimationClip statIncreaseAnim;
    public AnimationClip transformAnim;

    [Header("Audio")]
    FollowEnemyAI enemyAi;
    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<Enemy>();
        enemyAi = GetComponent<FollowEnemyAI>();

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.health <= enemy.maxHealth - 20 && !transformed)
        {
            StartCoroutine(Transform());
        }

        if (canAttack && enemy.onGround)
        {
            // select random attack
            int attack = Random.Range(0, 2);
            Debug.Log(attack);
            switch (attack)
            {
                case 0:
                    StartCoroutine(GroundSpikes());
                    Debug.Log("grd spk");
                    break;
                case 1:
                    Debug.Log("inc stat");
                    StartCoroutine(IncreaseStats());
                    break;
                default:
                    Debug.Log("def");
                    StartCoroutine(GroundSpikes());
                    break;
            }

            StartCoroutine(Cooldown(5));
        }
    }
    IEnumerator Cooldown(float additionalTime)
    {
        canAttack = false;
        float randomCooldown = Random.Range(attackCooldown - 2, attackCooldown + 2);
        yield return new WaitForSeconds(randomCooldown + additionalTime);
        canAttack = true;
    }

    IEnumerator Transform()
    {
        StartCoroutine(enemyAi.FreezeEnemy(transformAnim.length));
        enemyAi.animator.SetBool("isTransformed", true);
        transformed = true;
        ToggleDamage(false);

        yield return new WaitForSeconds(transformAnim.length);

        ToggleDamage(true);
        StartCoroutine(Cooldown(0));
    }

    IEnumerator GroundSpikes()
    {
        StartCoroutine(enemyAi.FreezeEnemy(groundSpikesAnim.length));
        
        enemyAi.animator.SetBool("isGroundPounding", true);
        ToggleDamage(false);

        yield return new WaitForSeconds(groundPoundAnimDelay);

        Instantiate(groundSpikes, new Vector3(transform.position.x, transform.position.y + groundSpikesYSpawnPos, 0), Quaternion.identity);

        yield return new WaitForSeconds(groundSpikesAnim.length - groundPoundAnimDelay);

        enemyAi.animator.SetBool("isGroundPounding", false);
        ToggleDamage(true);
    }

    IEnumerator IncreaseStats()
    {
        StartCoroutine(enemyAi.FreezeEnemy(statIncreaseAnim.length));

        ToggleDamage(false);
        enemyAi.animator.SetBool("isStatIncreasing", true);

        yield return new WaitForSeconds(statIncreaseAnim.length);

        // apply stats
        attackCooldown += attackCooldown / 100 * cooldownPercent;
        enemy.health += enemy.maxHealth / 100 * healthPercent;
        enemyAi.baseSpeed += enemyAi.baseSpeed / 100 * healthPercent;
        enemyAi.speed = enemyAi.baseSpeed;
        
        enemyAi.animator.SetBool("isStatIncreasing", false);
        ToggleDamage(true);
    }

    void ToggleDamage(bool b)
    {
        enemy.canTakeDamage = b;
        enemy.canDealDamage = b;
    }
}
