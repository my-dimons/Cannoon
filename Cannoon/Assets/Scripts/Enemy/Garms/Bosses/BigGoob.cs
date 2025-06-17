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

    [Header("Random Jumping")]
    public float minJumpTime;
    public float maxJumpTime;
    bool canRandomJump;

    [Header("Audio")]
    FollowEnemyAI enemyAi;
    Enemy enemy;
    GameObject gameManager;
    // Start is called before the first frame update
    void Start()
    {
        canRandomJump = true;
        enemy = GetComponent<Enemy>();
        enemyAi = GetComponent<FollowEnemyAI>();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.health <= 0)
        {
            gameManager.GetComponent<GameManager>().playingBossTracks = false;
            gameManager.GetComponent<MonoBehaviour>().StartCoroutine(gameManager.GetComponent<GameManager>().PlayMusicTrack());
        }

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
                    break;
                case 1:
                    StartCoroutine(IncreaseStats());
                    break;
                default:
                    StartCoroutine(GroundSpikes());
                    break;
            }

            StartCoroutine(Cooldown(5));
        }

        if (canRandomJump && canAttack && enemy.canJump && enemy.onGround)
        {
            Debug.Log("Big Goob Jumping");
            float time = Random.Range(minJumpTime, maxJumpTime);
            StartCoroutine(RandomJump(time));
        }
    }
    IEnumerator Cooldown(float additionalTime)
    {
        canAttack = false;
        float randomCooldown = Random.Range(attackCooldown - 2, attackCooldown + 2);
        yield return new WaitForSeconds(randomCooldown + additionalTime);
        canAttack = true;
    }

    IEnumerator RandomJump(float time)
    {
        canRandomJump = false;
        yield return new WaitForSeconds(time);

        Vector2 direction = ((Vector2)enemyAi.path.vectorPath[enemyAi.currentWaypoint] - GetComponent<Rigidbody2D>().position).normalized;
        float forceX = direction.x * enemy.speed * Time.deltaTime;
        enemyAi.Jump(forceX);
        canRandomJump = true;
    }

    IEnumerator Transform()
    {
        StartCoroutine(enemy.FreezeEnemy(transformAnim.length));
        enemy.animator.SetBool("isTransformed", true);
        transformed = true;
        ToggleDamage(false);

        yield return new WaitForSeconds(transformAnim.length);

        gameManager.GetComponent<GameManager>().playingBossTracks = true;
        StartCoroutine(gameManager.GetComponent<GameManager>().PlayBossMusicTrack());
        ToggleDamage(true);
        StartCoroutine(Cooldown(0));
    }

    IEnumerator GroundSpikes()
    {
        StartCoroutine(enemy.FreezeEnemy(groundSpikesAnim.length));

        enemy.animator.SetBool("isGroundPounding", true);
        ToggleDamage(false);

        yield return new WaitForSeconds(groundPoundAnimDelay);

        Instantiate(groundSpikes, new Vector3(transform.position.x, transform.position.y + groundSpikesYSpawnPos, 0), Quaternion.identity);

        yield return new WaitForSeconds(groundSpikesAnim.length - groundPoundAnimDelay);

        enemy.animator.SetBool("isGroundPounding", false);
        ToggleDamage(true);
    }

    IEnumerator IncreaseStats()
    {
        StartCoroutine(enemy.FreezeEnemy(statIncreaseAnim.length));

        ToggleDamage(false);
        enemy.animator.SetBool("isStatIncreasing", true);

        yield return new WaitForSeconds(statIncreaseAnim.length);

        // apply stats
        attackCooldown += attackCooldown / 100 * cooldownPercent;
        enemy.health += Mathf.Clamp(enemy.maxHealth / 100 * healthPercent, 200, enemy.baseHealth);

        enemy.baseSpeed += enemy.baseSpeed / 100 * healthPercent;
        enemy.speed = enemy.baseSpeed;

        enemy.animator.SetBool("isStatIncreasing", false);
        ToggleDamage(true);
    }

    void ToggleDamage(bool b)
    {
        enemy.canTakeDamage = b;
        enemy.canDealDamage = b;
    }
}
