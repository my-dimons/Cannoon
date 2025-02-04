using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public GameObject parent;
    public bool playerBullet;

    public float distanceFromPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // gets destroyed if too far from player
        distanceFromPlayer = Vector2.Distance(this.transform.position, parent.transform.position);
        if (distanceFromPlayer >= 25)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void setStats(float newSpeed, GameObject newParent, bool isPlayerBullet, float newDamage)
    {
        speed = newSpeed;
        parent = newParent;
        playerBullet = isPlayerBullet;
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    { 
        // collides with ground
        if (other.gameObject.CompareTag("Ground"))
            Destroy(gameObject);

        // collides with enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.health -= damage;
            Destroy(gameObject);
        }
    }
}