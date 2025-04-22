using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    public GameObject player;
    public Transform target;

    [Header("Stats")]

    [Tooltip("The base HP this enemy has")]
    public float baseHealth;
    [Tooltip("The current HP this enemy has")]
    public float currentHealth;

    [Tooltip("The base damage this enemy does")]
    public float baseDamage;
    [Tooltip("The current damage this enemy does")]
    public float currentDamage;

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

    [Header("Special Stats")]

    [Tooltip("Does this enemy fly?, if so it will have different spawn locations than usual (NOTE: FLYING ENEMIES ARE SOMETIMES REFERED TO AS 'SKY' ENEMIES)")]
    public bool flyingEnemy; // NOTE: sometimes flying enemies are also refered to as "sky" enemies


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
    }

    // Update is called once per frame
    void Update()
    {
        // if health is below 0: kill this enemy
        if (currentHealth <= 0)
        {
            KillEnemy();
        }

        ApplyDifficultyRating(false);
    }

    private void KillEnemy()
    {
        Destroy(gameObject);

        IncrementKills(1);
    }

    private void ApplyDifficultyRating(bool start)
    {
        currentDamage = baseDamage * endlessModeScript.difficultyMultiplier;
        currentCriticalChance = Mathf.Clamp(baseCriticalChance * endlessModeScript.difficultyMultiplier, 0, 100);
        currentCriticalMultiplier = baseCriticalMultiplier * endlessModeScript.difficultyMultiplier;

        // apply only when the enemy spawns
        if (start)
            currentHealth = baseHealth * endlessModeScript.difficultyMultiplier;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    
    void IncrementKills(int kills)
    {
        gameManager.currentKills += kills;
        gameManager.globalKills += kills;
    }
}
