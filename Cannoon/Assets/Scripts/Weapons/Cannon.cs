using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.AnimatedValues;

public class Cannon : MonoBehaviour
{
    public GameObject player;
    public GameObject cannonball;
    public AnimationClip loadingAnimation;
    Animator animator;

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

    [Header("Shooting Effects")]
    public GameObject postProcessing;
    public GameObject chargeParticleSystem;
    private bool playedParticles;
    private Volume volume;
    public float baseFov;
    public float maxFov;

    private float baseVigette;
    private float baseBloom;

    [Header("OTHER")]
    bool charging;
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
        volume = postProcessing.GetComponent<Volume>();
        animator = GetComponent<Animator>();

        cannonFacingRight = true;
        baseVigette = volume.GetComponent<Vignette>().intensity.value;
        baseVigette = volume.GetComponent<Bloom>().intensity.value;
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
        if (Input.GetMouseButtonDown(0) && canShoot && !charging )
        {
            timerActive = true;
            charging = true;

            // animation
            animator.SetBool("isLoading", true);
            animator.SetFloat("loadingMultiplier", loadingAnimation.length / maxCharge);

            // charge meter
            cannonChargeCanvas.SetActive(true);
        }

        // Stop timer and shoot bullet (bullet stats depend on hold time), and make charge meter disappear
        else if (Input.GetMouseButtonUp(0) && canShoot && charging)
        {
            charging = false;
            // charge meter
            cannonChargeCanvas.SetActive(false);

            // animation
            animator.SetBool("isLoading", false);

            // clamp time
            Mathf.Clamp(currentTime, minCharge, maxCharge);

            // bullet stats
            float force = Mathf.Lerp(minPower, maxPower, Mathf.InverseLerp(minCharge, maxCharge, currentTime));
            float damage = Mathf.Lerp(minBulletDamage, maxBulletDamage, Mathf.InverseLerp(minCharge, maxCharge, currentTime));

            ShootBullet(force, damage);

            // reset timer
            timerActive = false;
            currentTime = 0;

            // reset shooting effects
            Camera.main.fieldOfView = baseFov;
            volume.profile.TryGet(out ChromaticAberration chromaticAbberation);
            volume.profile.TryGet(out Vignette vignette);
            volume.profile.TryGet(out Bloom bloom);
            chromaticAbberation.intensity.value = 0;
            vignette.intensity.value = baseVigette;
            bloom.intensity.value = baseBloom;
            playedParticles = false;
            cannonChargeImage.color = Color.white;
        }

        // Advances timer, fills the charge meter, and applies shooting effects
        if (timerActive && canShoot && charging)
        {
            float time = Mathf.InverseLerp(minCharge, maxCharge, currentTime);

            // CHARGE METER FILL
            float fill = Mathf.Lerp(0, 1, time);
            cannonChargeImage.fillAmount = fill;
            // Shooting effects
            float fov = Mathf.Lerp(baseFov, maxFov, time);
            float chromaticAbberationValue = Mathf.Lerp(0, 0.15f, time);
            float vigetteValue = Mathf.Lerp(baseVigette, 0.36f, time);
            float bloomValue = Mathf.Lerp(baseBloom, 14, time);

            volume.profile.TryGet(out ChromaticAberration chromaticAbberation);
            volume.profile.TryGet(out Vignette vignette);
            volume.profile.TryGet(out Bloom bloom);

            chromaticAbberation.intensity.value = chromaticAbberationValue;
            vignette.intensity.value = vigetteValue;
            bloom.intensity.value = bloomValue;
            Camera.main.fieldOfView = fov;

            // TIMER
            currentTime += Time.deltaTime;

            // Fully charged
            if (currentTime >= maxCharge && !playedParticles)
            {
                chargeParticleSystem.GetComponent<ParticleSystem>().Play();
                cannonChargeImage.color = new Color(0.4f, 1f, 0.4f);
                playedParticles = true;
            }

        }
    }

    public void ShootBullet(float force, float damage)
    {
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        GameObject prefab = Instantiate(cannonball, spawnPos, cannonRotationObj.transform.rotation);
        prefab.GetComponent<Bullet>().SetStats(0, damage, bulletLifetime, true);
        prefab.GetComponent<Rigidbody2D>().AddForce(prefab.transform.right * force, ForceMode2D.Impulse);

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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(cannonRotationObj.transform.position);
        mousePos.x -= objectPos.x;
        mousePos.y -= objectPos.y;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        cannonRotationObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}