using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    public GameObject player;
    public GameObject bullet;

    [Header("Stats")]
    [Tooltip("How much damage shot bullets do")]
    public float bulletDamage;
    [Tooltip("How fast bullets travel")]
    public float bulletSpeed;
    [Tooltip("Time inbetween shooting (In Seconds)")]
    public float bulletCooldown;
    [Tooltip("Time untill the shot bullet gets deleted (in Seconds)")]
    public float bulletLifetime;

    [Tooltip("Minimun hold time for shooting (In Seconds)")]
    public float minCharge;
    [Tooltip("Maximum hold time for shooting (In Seconds)")]
    public float maxCharge;
    [Tooltip("Minimum shooting power")]
    public float minPower;
    [Tooltip("Maximum shooting power")]
    public float maxPower;


    /*
    [Header("Special Stats")]
    public bool canPierce;
    public int pierceAmount;
    public bool canBounce;
    public int bounceAmount;
    */

    [Header("Rotation & Shooting")]
    public GameObject cannonRotationObj;
    public GameObject bulletSpawnObj;
    bool cannonFacingRight;

    [Header("OTHER")]
    public bool canShoot;

    PlayerHealth playerHealthScript;

    public IEnumerator BulletShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletCooldown);
        canShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHealthScript = player.GetComponent<PlayerHealth>();

        canShoot = true;
        cannonFacingRight = true;
    }
    void Update()
    {
        if (!playerHealthScript.dead)
        {
            FlipCannonSprite();
            RotateCannonTowardsMouse();

            if (Input.GetMouseButton(0) && canShoot)
                ShootBullet();
        }
    }
    public void ShootBullet()
    {
        Debug.Log("Shot bullet");
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        GameObject prefab = Instantiate(bullet, spawnPos, cannonRotationObj.transform.rotation);

        prefab.GetComponent<Bullet>().setStats(bulletSpeed, bulletDamage, bulletLifetime, true);
        StartCoroutine(BulletShootingCooldown());
    }
    private void FlipCannonSprite()
    {
        if (cannonRotationObj.transform.eulerAngles.z > 90
            &&
            cannonRotationObj.transform.eulerAngles.z < 270
            &&
            cannonFacingRight)
        {
            GetComponent<SpriteRenderer>().flipY = true;
            cannonFacingRight = false;
            Debug.Log("FLIPING SPRITE");
        }

        else if (cannonRotationObj.transform.eulerAngles.z < 90 && !cannonFacingRight
            ||
            cannonRotationObj.transform.eulerAngles.z > 270
            &&
            !cannonFacingRight)
        {
            GetComponent<SpriteRenderer>().flipY = false;
            cannonFacingRight = true;
            Debug.Log("UNFLIPING SPRITE");
        }
    }
    private void RotateCannonTowardsMouse()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - cannonRotationObj.transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        cannonRotationObj.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}
