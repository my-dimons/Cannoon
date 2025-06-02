using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Cannonball : MonoBehaviour
{
    public float baseDamage;
    public float damage;
    public float distanceFromShooter;
    public float bulletLife;
    public GameObject destroyingParticles;
    public Color particleColor;

    [Header("Special")]
    public int bounces;
    public int pierces;
    public float pierceDamageDecrease;
    public bool explode;
    public GameObject bulletExplosion;

    [Header("Audio")]
    public AudioSource sound;
    public AudioClip bounceSfx;
    public AudioClip explosionSfx;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BulletLife(bulletLife));
        damage = baseDamage;
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
        baseDamage = newDamage;
        bulletLife = life;
        bounces = bounce;
        pierces = pierce;
        explode = explosion;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // collides with enemy
        if (other.gameObject.CompareTag("Enemy") && other.GetComponent<Enemy>().canTakeDamage)
        {
            PlayParticles();
            if (pierces > 0)
            {
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                damage /= pierceDamageDecrease;

                enemyScript.TakeDamage(damage);
                pierces--;
            }
            else
            {
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.TakeDamage(baseDamage);
                if (explode)
                    SpawnExplosion();
                Destroy(transform.parent.gameObject);
            }
        }

        // collides with ground
        if (other.gameObject.CompareTag("Ground"))
        {
            PlayParticles();
            if (bounces <= 0)
            {
                if (explode)
                    SpawnExplosion();
                Destroy(transform.parent.gameObject);
            }
            else
            {
                bounces--;
                sound.PlayOneShot(bounceSfx, 0.3f * GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume);
            }
        }
    }

    void PlayParticles()
    {
        if (destroyingParticles != null)
        {
            GameObject particles = Instantiate(destroyingParticles, transform.position, destroyingParticles.transform.rotation);

            MainModule main = particles.GetComponent<ParticleSystem>().main;
            main.startColor = particleColor;
            particles.GetComponent<ParticleSystem>().Play();
        }
    }

    void SpawnExplosion()
    {
        if (explode)
        {
            GameObject explosion = Instantiate(bulletExplosion, transform.position, bulletExplosion.transform.rotation);

            explosion.GetComponent<AudioSource>().PlayOneShot(explosionSfx, 0.5f * GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume);
            explosion.transform.GetChild(0).GetComponent<ContactDamage>().damage = baseDamage / 2;
            explosion.transform.localScale = new(bulletExplosion.transform.localScale.x * transform.parent.localScale.x, bulletExplosion.transform.localScale.y * transform.parent.localScale.y, 1);
        }
    }
}
