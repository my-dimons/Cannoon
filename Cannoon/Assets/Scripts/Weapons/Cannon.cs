using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
    [Tooltip("Is a random number from 0 -> thisVar which is a percent of the damage the shot did")]
    public float bonusDamagePercentage;
    [Tooltip("Time inbetween shooting (In Seconds)")]
    public float bulletCooldown;
    [Tooltip("Time untill the shot bullet gets deleted (in Seconds)")]
    public float bulletLifetime;

    [Header("Critical Hits")]
    public Color baseChargeColor;
    public Color criticalChargeColor;
    public float criticalStrikeChance;
    float critDamageMult = 1;
    float critPowerMult = 1;
    float critSizeMult = 1;

    [Header("Rotation & Shooting")]
    public bool canShoot;
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
    private float chargeTime;

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

    [Header("OTHER")]
    bool charging;

    GameManager gameManager;
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
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerHealthScript = player.GetComponent<PlayerHealth>();
        volume = postProcessing.GetComponent<Volume>();
        animator = GetComponent<Animator>();

        cannonFacingRight = true;

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
            RotateCannonTowardsMouse();

            Shooting();
        }
    }

    // Hold down left click to shoot, vars are explained at their defining
    private void Shooting()
    {
        // Start timer and make charge meter appear
        if (Input.GetMouseButtonDown(0) && canShoot && !charging)
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
            // crit stats
            critDamageMult = 1;
            critPowerMult = 1;
            critSizeMult = 1;

            charging = false;
            // charge meter
            cannonChargeCanvas.SetActive(false);

            // animation
            animator.SetBool("isLoading", false);

            // clamp time
            Mathf.Clamp(chargeTime, minCharge, maxCharge);

            // bullet stats
            float force = Mathf.Lerp(minPower, maxPower * critPowerMult, Mathf.InverseLerp(minCharge, maxCharge, chargeTime));
            float damage = Mathf.Lerp(minBulletDamage, maxBulletDamage * critDamageMult, Mathf.InverseLerp(minCharge, maxCharge, chargeTime));
            float extraDamage;
            // fully charged
            if (chargeTime >= maxCharge)
                extraDamage = Random.Range(0, damage / 100 * bonusDamagePercentage);
            else
                extraDamage = 0;
            ShootBullet(force, damage + extraDamage, critSizeMult);

            // sound
            float audioVolume = Mathf.Lerp(0, 1, Mathf.InverseLerp(minCharge, maxCharge, chargeTime));
            cannonAudio.pitch = Random.Range(0.85f, 1.15f);
            cannonAudio.PlayOneShot(shootingSound, audioVolume / 1.25f * gameManager.audioVolume);

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

        // Advances timer, fills the charge meter, and applies shooting effects
        if (timerActive && canShoot && charging)
        {
            float time = Mathf.InverseLerp(minCharge, maxCharge, chargeTime);

            // CHARGE METER FILL
            float fill = Mathf.Lerp(0, 1, time);
            cannonChargeImage.fillAmount = fill;

            // Shooting effects
            float fov = Mathf.Lerp(baseFov, maxFov, time);
            float chromaticAbberationValue = Mathf.Lerp(0, 0.15f, time);
            float vigetteValue = Mathf.Lerp(baseVigette, 0.45f, time);
            float bloomValue = Mathf.Lerp(baseBloom, 14, time);

            volume.profile.TryGet(out ChromaticAberration chromaticAbberation);
            volume.profile.TryGet(out Vignette vignette);
            volume.profile.TryGet(out Bloom bloom);

            chromaticAbberation.intensity.value = chromaticAbberationValue;
            vignette.intensity.value = vigetteValue;
            bloom.intensity.value = bloomValue;
            Camera.main.fieldOfView = fov;

            // TIMER
            chargeTime += Time.deltaTime;
            

            // Fully charged
            if (chargeTime >= maxCharge && !playedParticles)
            {
                float gambling = Random.Range(0, 100);

                // CRITICAL HIT
                if (gambling < criticalStrikeChance)
                {
                    // stats
                    critDamageMult = 1.5f;
                    critPowerMult = 1.25f;
                    critSizeMult = 1.55f;

                    // colors
                    ParticleSystem.MainModule main = chargeParticleSystem.GetComponent<ParticleSystem>().main;
                    cannonChargeImage.color = criticalChargeColor;
                    main.startColor = criticalChargeColor;

                    // sound
                    chargeAudio.PlayOneShot(critChargedSound, 1f * gameManager.audioVolume);
                } 
                // NOT critical hit :(
                else
                {
                    // colors
                    ParticleSystem.MainModule main = chargeParticleSystem.GetComponent<ParticleSystem>().main;
                    cannonChargeImage.color = baseChargeColor;
                    main.startColor = baseChargeColor;

                    // sound
                    chargeAudio.PlayOneShot(normalChargedSound, 1f * gameManager.audioVolume);
                }

                chargeParticleSystem.GetComponent<ParticleSystem>().Play();
                playedParticles = true;
            }

        }
    }

    public void ShootBullet(float force, float damage, float sizeMult)
    {
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        GameObject prefab = Instantiate(cannonball, spawnPos, cannonRotationObj.transform.rotation);
        prefab.transform.localScale = new Vector3(prefab.transform.localScale.x * sizeMult, prefab.transform.localScale.y * sizeMult, 1f);
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