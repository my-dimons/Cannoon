using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float distanceFromShooter;
    public float bulletLife;
    [Tooltip("Leave 0 for no despawning")]
    public float NoSpeedBulletLife;
    public bool playerBullet;
    public GameObject sprite;
    public GameObject destroyingParticles;
    public bool lookWhereTraveling;

    // Start is called before the first frame update
    void Start()
    {
        if (NoSpeedBulletLife != 0)
            StartCoroutine(NoSpeedLife());
        StartCoroutine(BulletLife(bulletLife));
    }

    IEnumerator BulletLife(float life)
    {
        yield return new WaitForSeconds(life);
        DespawnBullet();
    }

    IEnumerator NoSpeedLife()
    {
        yield return new WaitForSeconds(NoSpeedBulletLife);
        if (speed == 0)
        {
            DespawnBullet();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // moves bullet
        transform.Translate(speed * Time.deltaTime * Vector3.right);
    }

    private void FixedUpdate()
    {
        if (lookWhereTraveling)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.MoveRotation(Quaternion.LookRotation(rb.velocity));
        }
    }

    private void DespawnBullet()
    {
        Destroy(this.gameObject);
    }
    public void SetStats(float newSpeed, float newDamage, float life, bool isPlayerBullet)
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
        {
            PlayParticles();
            Destroy(gameObject);
        }

        if (playerBullet)
        {
            // collides with enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                PlayParticles();
                GameObject enemy = other.gameObject;
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        
        else if (!playerBullet)
        {
            // collides with player
            if (other.gameObject.CompareTag("Player"))
            {
                PlayParticles();
                GameObject player = other.gameObject;
                PlayerHealth playerHealthScript = player.GetComponent<PlayerHealth>();
                playerHealthScript.TakeDamage(damage);
                Destroy(gameObject);
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
}