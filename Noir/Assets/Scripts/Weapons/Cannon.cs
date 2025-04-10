using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Cannon : MonoBehaviour
{
    public GameObject player;
    public GameObject bullet;

    [Header("Stats")]
    [Tooltip("Minimum damage shot bullets do")]
    public float minBulletDamage;
    [Tooltip("Maximum damage shot bullets do")]
    public float maxBulletDamage;
    [Tooltip("Time inbetween shooting (In Seconds)")]
    public float bulletCooldown;
    [Tooltip("Time untill the shot bullet gets deleted (in Seconds)")]
    public float bulletLifetime;

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

    [Tooltip("Minimun hold time for shooting (In Seconds)")]
    public float minCharge;
    [Tooltip("Maximum hold time for shooting (In Seconds)")]
    public float maxCharge;
    [Tooltip("Minimum shooting power")]
    public float minPower;
    [Tooltip("Maximum shooting power")]
    public float maxPower;

    public Image cannonChargeImage;
    public GameObject cannonChargeCanvas;

    public bool timerActive;
    private float currentTime;

    [Header("OTHER")]
    bool charging;
    public bool canShoot;

    PlayerHealth playerHealthScript;
    PlayerManager playerManager;

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
        playerManager = player.GetComponent<PlayerManager>();

        if (playerManager.currentEquipedCannonballItem != null)
        {
            StartCoroutine(BulletShootingCooldown());
        }
        cannonFacingRight = true;
    }
    void Update()
    {
        if (!playerHealthScript.dead)
        {
            FlipCannonSprite();
            RotateCannonTowardsMouse();

            Shooting();
        }
    }

    // Hold down left click to shoot, vars are explained at their defining
    private void Shooting()
    {
        // Start timer and make charge meter appear
        if (Input.GetMouseButtonDown(0) && !playerManager.inventoryOpen && canShoot && !charging && playerManager.currentEquipedCannonballItem != null)
        {
            timerActive = true;
            charging = true;

            // charge meter
            cannonChargeCanvas.SetActive(true);
        }

        // Stop timer and shoot bullet (bullet stats depend on hold time), and make charge meter disappear
        else if (Input.GetMouseButtonUp(0) && !playerManager.inventoryOpen && canShoot && charging && playerManager.currentEquipedCannonballItem != null)
        {
            charging = false;
            // charge meter
            cannonChargeCanvas.SetActive(false);

            // clamp time
            Mathf.Clamp(currentTime, minCharge, maxCharge);

            // bullet stats
            float force = Mathf.Lerp(minPower, maxPower, Mathf.InverseLerp(minCharge, maxCharge, currentTime));
            float damage = Mathf.Lerp(minBulletDamage, maxBulletDamage, Mathf.InverseLerp(minCharge, maxCharge, currentTime));

            ShootBullet(force, damage);

            // reset timer
            timerActive = false;
            currentTime = 0;
        }

        // Advances timer and fills the charge meter
        if (timerActive && canShoot && charging)
        {
            // CHARGE METER FILL
            float fill = Mathf.Lerp(0, 1, Mathf.InverseLerp(minCharge, maxCharge, currentTime));
            cannonChargeImage.fillAmount = fill;

            // TIMER
            currentTime += Time.deltaTime;
        }
    }

    public void ShootBullet(float force, float damage)
    {
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        if (playerManager.currentEquipedCannonballItem == null)
        {
            Debug.Log("Not enough ammo");
            canShoot = false;
        } else
        {
            GameObject prefab = Instantiate(playerManager.currentEquipedCannonballItem.GetPrefab(), spawnPos, cannonRotationObj.transform.rotation);
            prefab.GetComponent<Bullet>().SetStats(0, damage, bulletLifetime, true);
            prefab.GetComponent<Rigidbody2D>().AddForce(prefab.transform.right * force, ForceMode2D.Impulse);

            // TODO: change to new shooting amount var soon

            playerManager.RemoveItem(new Item { itemType = playerManager.currentEquipedCannonballItem.itemType, amount = 1});
            if (playerManager.currentEquipedCannonballItem.amount <= 0)
            {
                playerManager.UnequipCannonball();
            }

            StartCoroutine(BulletShootingCooldown());
        }
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
            FlipCannonChargeCanvas();

            cannonFacingRight = false;
        }

        else if (cannonRotationObj.transform.eulerAngles.z < 90 && !cannonFacingRight
            ||
            cannonRotationObj.transform.eulerAngles.z > 270
            &&
            !cannonFacingRight)
        {
            GetComponent<SpriteRenderer>().flipY = false;

            FlipCannonChargeCanvas();

            cannonFacingRight = true;
        }

        void FlipCannonChargeCanvas()
        {
            RectTransform canvas = cannonChargeCanvas.GetComponent<RectTransform>();
            canvas.localScale = new Vector3(canvas.localScale.x, -canvas.localScale.y, canvas.localScale.z);
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