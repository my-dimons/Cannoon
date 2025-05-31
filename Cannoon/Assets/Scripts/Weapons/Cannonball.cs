using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    public float damage;
    public float distanceFromShooter;
    public float bulletLife;
    public GameObject destroyingParticles;

    [Header("Special")]
    public int bounces;
    public int pierces;
    public bool explode;
    public GameObject bulletExplosion;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BulletLife(bulletLife));
    }

    IEnumerator BulletLife(float life)
    {
        yield return new WaitForSeconds(life);
        DespawnBullet();
    }

    private void FixedUpdate()
    {
        RotateCannonball();
    }

    // rotates the cannonball depending on where its traveling
    private void RotateCannonball()
    {
        Vector2 velocity = transform.parent.GetComponent<Rigidbody2D>().velocity;

        if (velocity.sqrMagnitude > 0.01f) // Prevent jittering when nearly stopped
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.parent.Find("sprite").transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    private void DespawnBullet()
    {
        Destroy(this.gameObject);
    }
    public void SetStats(float newDamage, float life, int bounce, int pierce, bool explosion)
    {
        damage = newDamage;
        bulletLife = life;
        bounces = bounce;
        pierces = pierce;
        explode = explosion;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // collides with ground
        if (other.gameObject.CompareTag("Ground"))
        {
            PlayParticles();
            if (bounces > 0)
            {
                bounces -= 1;
            }
            else
            {
                if (explode)
                    SpawnExplosion();
                Destroy(transform.parent.gameObject);
            }
        }
        // collides with enemy
        if (other.gameObject.CompareTag("Enemy") && other.GetComponent<Enemy>().canTakeDamage)
        {
            PlayParticles();

            if (pierces > 0)
            {
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.TakeDamage(damage);
                pierces -= 1;
            }
            else
            {
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.TakeDamage(damage);
                Destroy(transform.parent.gameObject);

                if (explode)
                    SpawnExplosion();
            }
        }
    }

    void PlayParticles()
    {
        if (destroyingParticles != null)
        {
            GameObject particles = Instantiate(destroyingParticles, transform.position, destroyingParticles.transform.rotation);
            particles.GetComponent<ParticleSystem>().Play();
        }
    }

    void SpawnExplosion()
    {
        if (explode)
        {
            GameObject explosion = Instantiate(bulletExplosion, transform.position, bulletExplosion.transform.rotation);

            explosion.transform.GetChild(0).GetComponent<ContactDamage>().damage = damage / 2;
            explosion.transform.localScale = new(bulletExplosion.transform.localScale.x * transform.parent.localScale.x, bulletExplosion.transform.localScale.y * transform.parent.localScale.y, 1);
        }
    }
}
