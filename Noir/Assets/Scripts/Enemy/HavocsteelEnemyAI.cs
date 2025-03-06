using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HavocsteelEnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySprite;

    [Header("Stats")]
    public float speed;

    [Header("Shooting")]
    public bool canShoot;
    public GameObject bullet;
    public GameObject shootingPoint;
    public float bulletSpeed;
    public float bulletLifetime;
    public float shootingCooldown;


    [Header("Pathfinding")]

    public LayerMask layerMask;

    Path path;
    int currentWaypoint = 0;
    public float nextWaypointDistance = 3f;
    bool reachedEndOfPath = false;
    bool isFacingRight = true;

    Seeker seeker;
    Rigidbody2D rb;
    Enemy enemyScript;

    IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(GetTarget());
        IEnumerator GetTarget()
        {
            yield return new WaitForSeconds(.01f);

            target = this.gameObject.GetComponent<Enemy>().target;
            InvokeRepeating("UpdatePath", 0f, .5f);
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float forceX = direction.x * speed * Time.deltaTime;

        // Move enemy along the X axis, not Y
        rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (forceX >= 0.01f)
        {
            enemySprite.localScale = new Vector3(1f, 1f, 1f);
            isFacingRight = true;
        }

        else if (forceX <= -0.01f)
        {
            enemySprite.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = false;
        }

    }

    private void Update()
    {
        ShootBullet();
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    void ShootBullet()
    {
        if (canShoot)
        {
            // shooting velocity/direction depends on which way the enemy is facing
            float spawnBulletRotation;
            if (isFacingRight)
                spawnBulletRotation = 0f;
            else
                spawnBulletRotation = 180f;
            
            GameObject newBullet = Instantiate(bullet, shootingPoint.transform.position, Quaternion.Euler(0, 0, spawnBulletRotation));
            newBullet.GetComponent<Bullet>().setStats(bulletSpeed, enemyScript.damage, bulletLifetime, false);
            StartCoroutine(ShootingCooldown());
        }
    }
}
