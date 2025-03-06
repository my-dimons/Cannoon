using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float distanceFromShooter;
    public float bulletLife;
    public bool playerBullet;

    public GameObject sprite;

    bool dying;
    // Start is called before the first frame update
    void Start()
    {
        dying = false;
    }

    // Update is called once per frame
    void Update()
    {
        // gets destroyed after a certain time
        DespawnBullet(bulletLife);

        // moves bullet
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void DespawnBullet(float time)
    {
        if (!dying)
            StartCoroutine(BulletLife(time));

        IEnumerator BulletLife(float life)
        {
            yield return new WaitForSeconds(life);
            Destroy(this.gameObject);
            dying = true;
        }
    }

    public void setStats(float newSpeed, float newDamage, float life, bool isPlayerBullet)
    {
        speed = newSpeed;
        damage = newDamage;
        bulletLife = life;
        playerBullet = isPlayerBullet;
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        // collides with ground
        if (other.gameObject.CompareTag("Ground"))
            Destroy(gameObject);

        if (playerBullet)
        {
            // collides with enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.health -= damage;
                Destroy(gameObject);
            }
        }
        
        else if (!playerBullet)
        {
            // collides with player
            if (other.gameObject.CompareTag("Player"))
            {
                GameObject player = other.gameObject;
                Player playerScript = player.GetComponent<Player>();
                playerScript.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}