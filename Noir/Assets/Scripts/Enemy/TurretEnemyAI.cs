using System.Collections;
using UnityEngine;

public class TurretEnemyAI : MonoBehaviour
{
    [Header("Required References")]
    public Transform target;

    [Tooltip("The enemys body/base sprite")]
    public Transform enemySpriteBase;
    [Tooltip("This enemys cannon sprite")]
    public Transform enemySpriteCannon;
    [Tooltip("The rotation anchor point that this enemys cannon sprite rotates on")]
    public Transform enemyCannonSpriteRotationAnchor;
    [Tooltip("Where the bullets are shot from")]
    public Transform enemyCannonShootingPoint;

    public GameObject bullet;
    bool cannonFacingRight;
    bool canShoot;

    [Header("Stats")]
    public float baseShootingCooldown;
    public float currentShootingCooldown;
    public float baseBulletSpeed;
    public float currentBulletSpeed;

    public float bulletLifetime;

    // OTHER: Referenced in Start()
    Rigidbody2D rb;
    Enemy enemyScript;
    EndlessMode endlessModeScript;

    IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(currentShootingCooldown);
        canShoot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        FlipCannonSprite();
        StartCoroutine(ShootingCooldown());

        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
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
        ApplyDifficultyRating();
        RotateCannonTowardsTarget();
        FlipCannonSprite();

        if (canShoot)
        {
            ShootBullet();
        }
    }

    private void ApplyDifficultyRating()
    {
        currentShootingCooldown = baseShootingCooldown / endlessModeScript.difficultyMultiplier;
        currentBulletSpeed = baseBulletSpeed * Mathf.Clamp(endlessModeScript.difficultyMultiplier/1.5f, 1, Mathf.Infinity);
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    void ShootBullet()
    {
        GameObject prefab = Instantiate(bullet, enemyCannonShootingPoint.position, enemySpriteCannon.transform.rotation);

        prefab.GetComponent<Bullet>().SetStats(currentBulletSpeed, GetComponent<Enemy>().currentDamage, bulletLifetime, false);

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
