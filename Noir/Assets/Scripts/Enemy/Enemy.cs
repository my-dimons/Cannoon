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

    // Damage types (how this enemy deals damage)
    public bool damageOnContact;

    public bool Ranged;
    public GameObject bullet;
    public float bulletLifetime;
    public float bulletSpeed;
    public float fireRate;
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("ENEMY COLLIDED");
        if (damageOnContact)
        {
            Debug.Log("DAMAGE ON CONTACT");
            if (collision.gameObject.CompareTag("PlayerEnemyCollisions"))
            {
                Debug.Log("Dealing Damage");
                player.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
