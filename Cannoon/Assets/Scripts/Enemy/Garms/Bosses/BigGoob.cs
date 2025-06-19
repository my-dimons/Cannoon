using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class BigGoob : MonoBehaviour
{
    public bool bigGoob;
    public bool transformed;
    bool attacking;
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
    public AudioSource audioSource;
    public AudioClip increaseStatsSfx;
    public AudioClip groundPoundSfx;
    public AudioClip transformSfx;
    FollowEnemyAI enemyAi;
    Enemy enemy;
    GameObject gameManager;
    GameObject bossBar;
    GameObject bossBarFill;
    bool bossBarEnabled;
    // Start is called before the first frame update
    void Start()
    {
        canRandomJump = true;
        bigGoob = false;
        enemy = GetComponent<Enemy>();
        enemyAi = GetComponent<FollowEnemyAI>();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        bossBar = GameObject.Find("Boss Bar");
        bossBarFill = bossBar.transform.Find("Bar").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (bossBarEnabled)
            FillBossBar();
        if (enemy.health <= 0)
        {
            DisableBossBar();
            gameManager.GetComponent<GameManager>().playingBossTracks = false;
            gameManager.GetComponent<MonoBehaviour>().StartCoroutine(gameManager.GetComponent<GameManager>().PlayMusicTrack());
            StopCoroutine(gameManager.GetComponent<GameManager>().PlayBossMusicTrack());
            audioSource.Stop();
        }

        if (enemy.health <= enemy.maxHealth - 20 && !transformed)
        {
            StartCoroutine(Transform());
        }

        if (canAttack && enemy.onGround && bigGoob)
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

        if (canRandomJump && enemy.onGround && bigGoob && canAttack)
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
        Vector2 direction = ((Vector2)enemyAi.path.vectorPath[enemyAi.currentWaypoint] - GetComponent<Rigidbody2D>().position).normalized;
        float forceX = direction.x * enemy.speed * Time.deltaTime;
        enemyAi.Jump(forceX);

        yield return new WaitForSeconds(time);

        canRandomJump = true;
    }

    IEnumerator Transform()
    {
        StartCoroutine(enemy.FreezeEnemy(transformAnim.length));
        enemy.animator.SetBool("isTransformed", true);
        transformed = true;
        audioSource.PlayOneShot(transformSfx, 0.85f * gameManager.GetComponent<GameManager>().soundVolume);
        ToggleDamage(false);

        yield return new WaitForSeconds(transformAnim.length);

        gameManager.GetComponent<GameManager>().playingBossTracks = true;
        gameManager.GetComponent<GameManager>().canPlayMusic = true;
        StopCoroutine(gameManager.GetComponent<GameManager>().PlayMusicTrack());
        StartCoroutine(gameManager.GetComponent<GameManager>().PlayBossMusicTrack());
        bigGoob = true;
        ToggleDamage(true);
        EnableBossBar();
        StartCoroutine(Cooldown(0));
    }

    IEnumerator GroundSpikes()
    {
        StartCoroutine(enemy.FreezeEnemy(groundSpikesAnim.length));

        enemy.animator.SetBool("isGroundPounding", true);
        attacking = true;
        ToggleDamage(false);
        audioSource.PlayOneShot(groundPoundSfx, 0.85f * gameManager.GetComponent<GameManager>().soundVolume);
        yield return new WaitForSeconds(groundPoundAnimDelay);

        GameObject spikes = Instantiate(groundSpikes, new Vector3(transform.position.x, transform.position.y + groundSpikesYSpawnPos, 0), Quaternion.identity);
        spikes.GetComponent<AudioSource>().volume = gameManager.GetComponent<GameManager>().soundVolume;

        yield return new WaitForSeconds(groundSpikesAnim.length - groundPoundAnimDelay);

        attacking = false;
        enemy.animator.SetBool("isGroundPounding", false);
        ToggleDamage(true);
    }

    IEnumerator IncreaseStats()
    {
        StartCoroutine(enemy.FreezeEnemy(statIncreaseAnim.length));
        attacking = true;
        ToggleDamage(false);
        enemy.animator.SetBool("isStatIncreasing", true);
        audioSource.PlayOneShot(increaseStatsSfx, 0.85f * gameManager.GetComponent<GameManager>().soundVolume);
        yield return new WaitForSeconds(statIncreaseAnim.length);

        attacking = false;
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

    void EnableBossBar()
    {
        bossBar.GetComponent<RectTransform>().localPosition = Vector2.zero;
        bossBarEnabled = true;
    }

    void FillBossBar()
    {
        float healthPercent = Mathf.Clamp01(enemy.health / enemy.maxHealth);
        bossBarFill.GetComponent<Image>().fillAmount = healthPercent;
    }

    void DisableBossBar()
    {
        bossBar.GetComponent<RectTransform>().localPosition = new(0, 2000);
        bossBarEnabled = false;
    }
}
