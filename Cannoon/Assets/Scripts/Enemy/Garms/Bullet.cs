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
    public GameObject sprite;

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

    private void DespawnBullet()
    {
        Destroy(this.gameObject);
    }
    public void SetStats(float newSpeed, float newDamage, float life)
    {
        speed = newSpeed;
        damage = newDamage;
        bulletLife = life;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // collides with ground
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // collides with player
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerHealth playerHealthScript = player.GetComponent<PlayerHealth>();
            playerHealthScript.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}