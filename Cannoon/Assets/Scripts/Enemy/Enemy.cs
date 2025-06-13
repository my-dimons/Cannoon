using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    public GameObject player;
    public Transform target;
    public AnimationClip deathAnimation;

    [Header("Bools")]
    public bool frozen;
    public bool canJump;
    public bool canMove;
    public bool onGround;
    public bool canDealDamage;
    public bool canTakeDamage;
    public bool stunned; // used for stun upgrade

    public bool destroyBullet;
    [Header("Health")]
    public float maxHealth;
    [Tooltip("The base HP this enemy has")]
    public float baseHealth;
    [Tooltip("The current HP this enemy has")]
    public float health;
    public GameObject healingParticles;

    [Tooltip("The lowest possible wave this enemy will spawn in")]
    public float minWave;
    [Tooltip("The highest possible wave this enemy will spawn in")]
    public float maxWave;
    [Tooltip("This enemy can be spawned no matter the wave")]
    public bool waveOverride;

    [Header("Damage")]
    public GameObject damageText;

    [Header("Sounds")]
    public AudioSource enemyAudio;
    public AudioClip hitSound;

    //OTHER: Referenced in start
    [HideInInspector] public GameManager gameManager;
    EndlessMode endlessModeScript;
    FollowEnemyAI enemyAi;
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyAi = GetComponent<FollowEnemyAI>();
        target = player.transform;

        ApplyDifficultyRating(true);
        canJump = true;
        StartCoroutine(Spawning());
    }

    IEnumerator Spawning()
    {
        bool doDamage = true;
        if (!canDealDamage)
            doDamage = false;

        canTakeDamage = false;
        canDealDamage = false;
        yield return new WaitForSeconds(GetComponent<FollowEnemyAI>().spawningAnimation.length);
        if (doDamage)
            canDealDamage = true;
        canTakeDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if health is below 0: kill this enemy
        if (health <= 0)
        {
            Death();
        }

        ApplyDifficultyRating(false);
    }

    private void Death()
    {
        GetComponent<FollowEnemyAI>().animator.SetBool("isDying", true);
        canDealDamage = false;
        canTakeDamage = false;

        StartCoroutine(DestroyEnemy(deathAnimation.length));
        StartCoroutine(GetComponent<FollowEnemyAI>().FreezeEnemy(deathAnimation.length));

        IEnumerator DestroyEnemy(float time)
        {
            yield return new WaitForSeconds(time);

            Destroy(gameObject);
            IncrementKills(1);
        }
    }

    private void ApplyDifficultyRating(bool start)
    {
        // apply only when the enemy spawns
        if (start)
        {
            health = (float)(baseHealth * endlessModeScript.difficultyMultiplier) * gameManager.difficulty;
            maxHealth = health;
            enemyAi.baseSpeed *= Mathf.Clamp((float)(endlessModeScript.difficultyMultiplier / 1.75f), 1, enemyAi.baseSpeed * 1.4f);
            enemyAi.baseSpeed *= gameManager.difficulty;
        }
    }

    public void TakeDamage(float damage)
    {
        // deal damage
        health -= damage;

        // flash white
        StartCoroutine(GetComponent<FollowEnemyAI>().enemySprite.GetComponent<DamageFlash>().FlashWhite());
        // SFX
        enemyAudio.PlayOneShot(hitSound, 1f * gameManager.soundVolume);

        // damage text
        Vector3 spawnPos = new (
            transform.position.x + Random.Range(-1f, 1f),
            transform.position.y + Random.Range(0.1f, 1f),
            0);
        Vector2 force = new (
            Random.Range(-100, 100),
            Random.Range(300, 500));
        GameObject text = Instantiate(damageText, spawnPos, Quaternion.identity);
        text.GetComponent<TextMeshPro>().text = Mathf.RoundToInt(damage).ToString();
        text.GetComponent<Rigidbody2D>().AddForce(force);

        // text color
        var damageColor = damage switch
        {
            >= 140 => new Color(0.84f, 0.44f, 0.9f),
            >= 80 => new Color(0.31f, 0.72f, 0.93f),
            >= 45 => new Color(0.93f, 0.38f, 0.31f),
            >= 30 => new Color(0.81f, 0.84f, 0.4f),
            _ => Color.white,
        };
        text.GetComponent<TextMeshPro>().color = damageColor;
    }

    public void Heal(float heal)
    {
        health += heal;
        Instantiate(healingParticles, transform);
    }
    
    void IncrementKills(int kills)
    {
        gameManager.currentKills += kills;
        gameManager.globalKills += kills;
    }
}
