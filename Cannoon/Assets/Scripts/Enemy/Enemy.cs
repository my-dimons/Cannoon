using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    public GameObject player;
    public Transform target;
    public AnimationClip deathAnimation;

    [Header("Can ___")]
    public bool canJump;
    public bool canMove;
    public bool onGround;
    public bool canDealDamage;

    [Header("Health")]
    [Tooltip("The base HP this enemy has")]
    public float baseHealth;
    [Tooltip("The current HP this enemy has")]
    public float health;

    [Header("Criticals")]
    [Tooltip("WIP | The chance of an enemy does a critical hit, which multiplies the base damage by the critical mutiplier")]
    public float baseCriticalChance;
    public float currentCriticalChance;
    [Tooltip("WIP | multiplier * damage, in effect when the enemy does a critical hit")]
    public float baseCriticalMultiplier;
    public float currentCriticalMultiplier;

    [Tooltip("The lowest possible wave this enemy will spawn in")]
    public float minWave;
    [Tooltip("The highest possible wave this enemy will spawn in")]
    public float maxWave;


    //OTHER: Referenced in start
    GameManager gameManager;
    EndlessMode endlessModeScript;
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;

        ApplyDifficultyRating(true);
        canMove = true;
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
        currentCriticalChance = Mathf.Clamp(baseCriticalChance * endlessModeScript.difficultyMultiplier, 0, 100);
        currentCriticalMultiplier = baseCriticalMultiplier * endlessModeScript.difficultyMultiplier;

        // apply only when the enemy spawns
        if (start)
        {
            health = baseHealth * endlessModeScript.difficultyMultiplier;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
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
