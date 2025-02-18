using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float distanceFromPlayer;
    public float bulletLife;

    public GameObject sprite;

    bool dying;
    // Start is called before the first frame update
    void Start()
    {
        dying = false;
        sprite.transform.rotation = Quaternion.Euler(0 + transform.rotation.x, 0 + transform.rotation.y, 0);
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

    public void setStats(float newSpeed, float newDamage, float life)
    {
        speed = newSpeed;
        damage = newDamage;
        bulletLife = life;
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