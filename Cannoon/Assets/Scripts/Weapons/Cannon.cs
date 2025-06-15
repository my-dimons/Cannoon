using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    public GameObject player;
    public AnimationClip loadingAnimation;
    public GameObject crown;
    Animator animator;

    [Header("Stats")]
    [Tooltip("Minimum damage shot bullets do")]
    public float minBulletDamage;
    [Tooltip("Maximum damage shot bullets do")]
    public float maxBulletDamage;
    [Tooltip("Is a random number from 0 -> thisVar which is a percent of the damage the shot did")]
    public float bonusDamagePercentage;
    [Tooltip("Time inbetween shooting (In Seconds)")]
    public float bulletCooldown;
    [Tooltip("Time untill the shot bullet gets deleted (in Seconds)")]
    public float bulletLifetime;

    [Header("Special Stats")]
    public int bounces;
    public int pierces;
    public bool explodingBullets;
    public bool explodeOnPierce;
    public bool explodeOnBounce;
    public bool autofire;
    public bool stunEnemies;
    public float stunChance;
    public float stunTime;

    [Header("Overheat")]
    public bool overheat;
    public float overheatDecline;
    bool overheated;
    public float overheatValue;
    public float overheatDeclineShootingTime;
    public GameObject overheatUi;
    public Color overheatColor;
    public Color overheatWarningColor;
    public Color overheatedColor;
    public GameObject overheatParticles;

    [Header("Critical Hits")]
    public Color baseChargeColor;
    public Color criticalChargeColor;
    public float criticalStrikeChance;
    [HideInInspector] public float baseCritDamageMult = 1;
    float critDamageMult;
    float critPowerMult = 1;
    public float baseSizeMult = 1;
    float sizeMult;

    [Header("Rotation & Shooting")]
    public bool canShoot;
    public bool charging;
    public GameObject cannonRotationObj;
    public GameObject bulletSpawnObj;
    bool cannonFacingRight;

    public float chargeLimit;
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
    private float chargeTime;

    [Header("Cannonballs")]
    public GameObject baseCannonball;
    public GameObject piercingCannonball;
    public GameObject bouncingCannonball;
    public GameObject piercingBouncingCannonball;
    public GameObject activeCannonball;

    [Header("Shooting Effects")]
    public GameObject postProcessing;
    public GameObject chargeParticleSystem;
    private bool playedParticles;
    private Volume volume;
    public float baseFov;
    public float maxFov;

    private float baseVigette;
    private float baseBloom;

    [Header("Audio")]
    public AudioSource cannonAudio;
    public AudioSource chargeAudio;
    public AudioClip normalChargedSound;
    public AudioClip critChargedSound;
    public AudioClip shootingSound;
    public AudioClip declineShot;


    GameManager gameManager;
    PlayerHealth playerHealthScript;
    PlayerMovement playerMovementScript;

    public IEnumerator BulletShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletCooldown);
        canShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerMovementScript = player.GetComponent<PlayerMovement>();
        playerHealthScript = player.GetComponent<PlayerHealth>();
        volume = postProcessing.GetComponent<Volume>();
        animator = GetComponent<Animator>();

        cannonFacingRight = true;
        sizeMult = baseSizeMult;
        critDamageMult = baseCritDamageMult;

        volume.profile.TryGet(out Bloom bloom);
        volume.profile.TryGet(out Vignette vignette);
        baseVigette = vignette.intensity.value;
        baseBloom = bloom.intensity.value;
    }
    void Update()
    {
        if (!playerHealthScript.dead)
        {
            FlipCannonSprite();

            if (!gameManager.pauseMenuEnabled)
            {
                RotateCannonTowardsMouse();
                Shooting();
            }

            if (overheat)
            {
                if (overheatUi.activeSelf == false)
                    overheatUi.SetActive(true);

                overheatValue -= overheatDecline * Time.deltaTime;
                overheatValue = Mathf.Clamp01(overheatValue);

                if (canShoot && overheatValue >= 0.98f)
                    StartCoroutine(PauseShooting(overheatDeclineShootingTime));

                if (!overheated)
                {
                    if (overheatValue <= 0.7f)
                        overheatUi.GetComponent<Image>().color = overheatColor;
                    else
                        overheatUi.GetComponent<Image>().color = overheatWarningColor;
                }

                overheatUi.GetComponent<Image>().fillAmount = overheatValue;


            }

            maxBulletDamage = Mathf.Clamp(maxBulletDamage, 1, Mathf.Infinity);
        }
    }

    private void LateUpdate()
    {
        // set overheat particle rotation
        if (overheat)
        {
            overheatParticles.transform.rotation = Quaternion.identity;
        }
    }

    // Hold down left click to shoot, vars are explained at their defining
    private void Shooting()
    {
        // screenshake if cant shoot
        if (Input.GetMouseButtonDown(0) && overheated && !EventSystem.current.IsPointerOverGameObject())
        {
            StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(0.5f));
        }

        // Start timer and make charge meter appear
        if (Input.GetMouseButton(0) && canShoot && !charging && !EventSystem.current.IsPointerOverGameObject() && !overheated)
        {
            StartShooting();
        }

        // Stop timer and shoot bullet (bullet stats depend on hold time), and make charge meter disappear
        else if (Input.GetMouseButtonUp(0) && canShoot && charging && !overheated)
        {
            Shoot();
        }

        // Advances timer, fills the charge meter, and applies shooting effects
        if (timerActive && canShoot && charging && !overheated)
        {
            ChargingShot();
        }

        if (!canShoot && chargeTime > 0)
        {
            timerActive = false;
            Shoot();
        }

        void ChargingShot()
        {
            // TIMER
            chargeTime += Time.deltaTime;

            // autofire
            if (chargeTime >= maxCharge + Mathf.Lerp(0.05f, 0.3f, Mathf.InverseLerp(chargeLimit, 1, maxCharge)) && autofire)
            {
                Shoot();
                return;
            }
            float time = Mathf.InverseLerp(minCharge, maxCharge, chargeTime);

            // CHARGE METER FILL
            float fill = Mathf.Lerp(0, 1, time);
            cannonChargeImage.fillAmount = fill;

            // Shooting effects
            float fov = Mathf.Lerp(baseFov, maxFov, time);
            float chromaticAbberationValue = Mathf.Lerp(0, 0.15f, time);
            float vigetteValue = Mathf.Lerp(baseVigette, 0.45f, time);
            float bloomValue = Mathf.Lerp(baseBloom, 1.2f, time);

            volume.profile.TryGet(out ChromaticAberration chromaticAbberation);
            volume.profile.TryGet(out Vignette vignette);
            volume.profile.TryGet(out Bloom bloom);

            chromaticAbberation.intensity.value = chromaticAbberationValue;
            vignette.intensity.value = vigetteValue;
            bloom.intensity.value = bloomValue;
            Camera.main.fieldOfView = fov;

            // slow player down based on charge time
            float speed = playerMovementScript.baseSpeed * Mathf.Lerp(1, 0.55f, time);
            if (playerMovementScript.onGround)
                playerMovementScript.speed = speed;
            else
                playerMovementScript.speed = speed / playerMovementScript.airSpeedDivisor;

            // Fully charged
            if (chargeTime >= maxCharge && !playedParticles)
            {
                FullyChargedShot();
            }

            void FullyChargedShot()
            {
                StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(Mathf.Lerp(0, 0.12f, time)));
                float gambling = Random.Range(0, 100);

                // CRITICAL HIT
                if (gambling < criticalStrikeChance)
                {
                    // stats
                    critDamageMult = baseCritDamageMult * 1.25f;
                    critPowerMult *= 1.25f;
                    sizeMult = baseSizeMult * 1.3f;

                    // colors
                    ParticleSystem.MainModule main = chargeParticleSystem.GetComponent<ParticleSystem>().main;
                    cannonChargeImage.color = criticalChargeColor;
                    main.startColor = criticalChargeColor;

                    // sound
                    chargeAudio.PlayOneShot(critChargedSound, 1f * gameManager.soundVolume);
                }
                // NOT critical hit :(
                else
                {
                    // colors
                    ParticleSystem.MainModule main = chargeParticleSystem.GetComponent<ParticleSystem>().main;
                    cannonChargeImage.color = baseChargeColor;
                    main.startColor = baseChargeColor;

                    // sound
                    chargeAudio.PlayOneShot(normalChargedSound, 1f * gameManager.soundVolume);
                }


                chargeParticleSystem.GetComponent<ParticleSystem>().Play();
                playedParticles = true;
            }
        }

        void Shoot()
        {

            charging = false;
            float time = Mathf.InverseLerp(minCharge, maxCharge, chargeTime);

            // reset player speed
            playerMovementScript.speed = playerMovementScript.baseSpeed;

            // charge meter
            cannonChargeCanvas.SetActive(false);

            // overheat
            if (overheat)
            {
                overheatValue += 0.08f;
            }

            // animation
            animator.SetBool("isLoading", false);
            StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(Mathf.Lerp(0, 0.5f, time)));


            // bullet stats
            critDamageMult = Mathf.Clamp(critDamageMult, 1, Mathf.Infinity);
            float force = Mathf.Lerp(minPower, maxPower * critPowerMult, time);
            float damage = Mathf.Lerp(minBulletDamage, maxBulletDamage * critDamageMult, time);
            float extraDamage;
            // fully charged
            if (chargeTime >= maxCharge)
                extraDamage = Random.Range(0, damage / 100 * bonusDamagePercentage);
            else
                extraDamage = 0;
            ShootBullet(force, damage + extraDamage, sizeMult);

            // crit stats
            critDamageMult = baseCritDamageMult;
            sizeMult = baseSizeMult;
            critPowerMult = 1;

            // sound
            float audioVolume = Mathf.Lerp(0, 1, Mathf.InverseLerp(minCharge, maxCharge, chargeTime));
            cannonAudio.pitch = Random.Range(0.85f, 1.15f);
            cannonAudio.PlayOneShot(shootingSound, audioVolume / 1.25f * gameManager.soundVolume);

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

            // reset timer
            timerActive = false;
            chargeTime = 0;
        }

        void StartShooting()
        {
            if (maxCharge < chargeLimit)
                maxCharge = chargeLimit;
            timerActive = true;
            charging = true;

            // animation
            animator.SetBool("isLoading", true);
            animator.SetFloat("loadingMultiplier", loadingAnimation.length / maxCharge);

            // charge meter
            cannonChargeCanvas.SetActive(true);
        }
    }

    public void ShootBullet(float force, float damage, float sizeMult)
    {
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        // select cannonball
        if (pierces > 0 && bounces > 0)
            activeCannonball = piercingBouncingCannonball;
        else if (pierces > 0)
            activeCannonball = piercingCannonball;
        else if (bounces > 0)
            activeCannonball = bouncingCannonball;
        else
            activeCannonball = baseCannonball;

        GameObject prefab = Instantiate(activeCannonball, spawnPos, cannonRotationObj.transform.rotation);
        GameObject prefabChild = prefab.transform.Find("collisions").gameObject;
        prefab.transform.localScale = new Vector3(prefab.transform.localScale.x * sizeMult, prefab.transform.localScale.y * sizeMult, 1f);
        prefabChild.GetComponent<Cannonball>().SetStats(damage, bulletLifetime, bounces, pierces, explodingBullets, explodeOnPierce, explodeOnBounce, stunEnemies, this.gameObject);
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
            crown.GetComponent<SpriteRenderer>().flipY = true;
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
            crown.GetComponent<SpriteRenderer>().flipY = false;

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

    public void EnableOverheat()
    {
        overheat = true;
        overheatValue = 0;

        overheatUi.SetActive(true);
        overheatUi.GetComponent<Image>().color = overheatColor;
    }

    IEnumerator PauseShooting(float time)
    {
        canShoot = false;
        overheated = true;

        cannonAudio.PlayOneShot(declineShot, 1f * gameManager.soundVolume);
        overheatParticles.GetComponent<ParticleSystem>().Play();

        overheatUi.GetComponent<Image>().color = overheatedColor;

        yield return new WaitForSeconds(time);
        overheatParticles.GetComponent<ParticleSystem>().Stop();

        canShoot = true;
        overheated = false;
    }
}