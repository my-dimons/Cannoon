using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemySpriteBase;
    public Transform enemySpriteCannon;
    public Transform enemyCannonSpriteRotationAnchor;
    public Transform enemyCannonShootingPoint;

    public GameObject bullet;
    bool cannonFacingRight;
    public bool canShoot;
    public float shootingCooldownTime;
    public float bulletSpeed;
    public float bulletLifetime;

    Rigidbody2D rb;
    Enemy enemyScript;

    IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldownTime);
        canShoot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        FlipCannonSprite();
        StartCoroutine(ShootingCooldown());

        target = this.gameObject.GetComponent<Enemy>().target;
        enemyScript = this.GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void RotateCannonTowardsTarget()
    {
        // Rotate cannon barrel towards player
        Vector2 diff = target.position - enemyCannonSpriteRotationAnchor.transform.position;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        enemyCannonSpriteRotationAnchor.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    private void Update()
    {
        RotateCannonTowardsTarget();
        FlipCannonSprite();

        if (canShoot)
        {
            ShootBullet();
        }
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    void ShootBullet()
    {
        GameObject prefab = Instantiate(bullet, enemyCannonShootingPoint.position, enemySpriteCannon.transform.rotation);

        prefab.GetComponent<Bullet>().setStats(bulletSpeed, GetComponent<Enemy>().damage, bulletLifetime, false);

        StartCoroutine(ShootingCooldown());
    }

    private void FlipCannonSprite()
    {
        if (enemyCannonSpriteRotationAnchor.transform.eulerAngles.z > 90 
            && 
            enemyCannonSpriteRotationAnchor.transform.eulerAngles.z < 270 
            && 
            cannonFacingRight)
        {
            enemySpriteCannon.GetComponent<SpriteRenderer>().flipY = true;
            cannonFacingRight = false;
            Debug.Log("FLIPING SPRITE");
        }

        else if (enemyCannonSpriteRotationAnchor.transform.eulerAngles.z < 90 && !cannonFacingRight 
            || 
            enemyCannonSpriteRotationAnchor.transform.eulerAngles.z > 270 
            &&
            !cannonFacingRight)
        {
            enemySpriteCannon.GetComponent<SpriteRenderer>().flipY = false;
            cannonFacingRight = true;
            Debug.Log("UNFLIPING SPRITE");
        }
    }
}
