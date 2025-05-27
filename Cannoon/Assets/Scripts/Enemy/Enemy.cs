using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Health")]
    [Tooltip("The base HP this enemy has")]
    public float baseHealth;
    [Tooltip("The current HP this enemy has")]
    public float health;

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
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;

        ApplyDifficultyRating(true);
        canDealDamage = true;
        canMove = true;
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if health is below 0: kill this enemy
        if (health <= 0)
        {
            KillEnemy();
        }

        ApplyDifficultyRating(false);
    }

    private void KillEnemy()
    {
        GetComponent<FollowEnemyAI>().animator.SetBool("isDying", true);
        canDealDamage = false;

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
            health = baseHealth * endlessModeScript.difficultyMultiplier;
        }
    }

    public void TakeDamage(float damage)
    {
        // deal damage
        health -= damage;

        // flash white
        StartCoroutine(GetComponent<FollowEnemyAI>().enemySprite.GetComponent<DamageFlash>().FlashWhite());
        // SFX
        enemyAudio.PlayOneShot(hitSound, 1f * gameManager.audioVolume);

        // damage text
        Vector3 spawnPos = new Vector3(
            transform.position.x + Random.Range(-1f, 1f),
            transform.position.y + Random.Range(0.1f, 1f),
            0);
        Vector2 force = new Vector2(
            Random.Range(-100, 100),
            Random.Range(300, 500));
        GameObject text = Instantiate(damageText, spawnPos, Quaternion.identity);
        text.GetComponent<TextMeshPro>().text = Mathf.RoundToInt(damage).ToString();
        text.GetComponent<Rigidbody2D>().AddForce(force);
        // text color
        Color damageColor;
        switch (damage)
        {
            case >= 100:
                damageColor = new Color(0.31f, 0.72f, 0.93f);
                break;
            case >= 50:
                damageColor = new Color(0.93f, 0.38f, 0.31f);
                break;
            case >= 35:
                damageColor = new Color(0.81f, 0.84f, 0.4f);
                break;
            default:
                damageColor = Color.white;
                break;
        }
        text.GetComponent<TextMeshPro>().color = damageColor;
    }

    public void Heal(float heal)
    {
        health += heal;
    }
    
    void IncrementKills(int kills)
    {
        gameManager.currentKills += kills;
        gameManager.globalKills += kills;
    }
}
