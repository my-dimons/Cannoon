using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // The general script ALL enemies need, and an AI script
    public GameObject player;
    public Transform target;
    public float health;

    public float minDifficulty;
    public float maxDifficulty;

    [Header("Enemy Damage")]
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
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
        }
    }
}
