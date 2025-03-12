using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    [Header("Stats")]
    public GameObject player;
    public Transform target;
    public float health;

    public float minDifficulty;
    public float maxDifficulty;

    [Header("Enemy Damage")]
    public float damage;

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
        if (health <= 0)
        {
            Destroy(gameObject);
            player.GetComponent<Player>().kills += 1;
            gameManager.globalKills += 1;
        }
    }
}
