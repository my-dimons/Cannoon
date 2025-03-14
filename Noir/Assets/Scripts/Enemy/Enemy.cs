using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    [Header("Stats")]
    public GameObject player;
    public Transform target;

    [Tooltip("How much HP this enemy has")]
    public float health;
    [Tooltip("How much damage this enemy does")]
    public float damage;
    [Tooltip("WIP | The chance of an enemy does a critical hit, which multiplies the base damage by the critical mutiplier")]
    public float criticalChance;
    [Tooltip("WIP | multiplier * damage, in effect when the enemy does a critical hit")]
    public float criticalMultiplier;

    [Tooltip("The lowest possible wave difficulty this enemy will spawn in")]
    public float minDifficulty;
    [Tooltip("The highest possible wave difficulty this enemy will spawn in")]
    public float maxDifficulty;

    [Header("Special Stats")]
    [Tooltip("Does this enemy fly?, if so it will have different spawn locations than usual (NOTE: FLYING ENEMIES ARE SOMETIMES REFERED TO 'SKY' ENEMIES)")]
    public bool flyingEnemy; // NOTE: sometimes flying enemies are also refered to "sky" enemies

    [Header("Other")]
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // if health is below 0: kill this enemy
        if (health <= 0)
        {
            Destroy(gameObject);
            player.GetComponent<Player>().kills += 1;
            gameManager.globalKills += 1;
        }
    }
}
