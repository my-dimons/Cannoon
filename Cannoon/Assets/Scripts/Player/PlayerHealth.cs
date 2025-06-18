using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


// Deals with the players heal (Damage, Healing, and Death)
public class PlayerHealth : MonoBehaviour
{
    [Header("Death")]
    public GameObject deathScreen;
    public GameObject postProcessing;
    public bool dead;

    [Header("Health")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject healParticles;
    public bool invincible;

    [Tooltip("players max amount of hearts")]
    public int numOfHearts;
    [Tooltip("players current health/hearts")]
    public float health;

    public bool canTakeDamage;
    public float damageInvincibilityCooldown;

    [Header("Audio")]
    public AudioSource playerAudio;
    public AudioClip hitSound;

    [Header("Other")]
    private GameManager gameManager;
    private EndlessMode endlessMode;
    [HideInInspector] public GameObject cannon;
    public IEnumerator Invincibility(float time)
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(time);
        canTakeDamage = true;
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        endlessMode = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        cannon = GameObject.FindGameObjectWithTag("Cannon");

        canTakeDamage = true;
        health = numOfHearts;
    }

    private void Update()
    {
        UpdateHearts();

        // FULL HEAL (TEMPERARY)
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Heal(numOfHearts);

        // Ways to die

        // No Health
        if (health <= 0 && !dead)
        {
            Death();
        }
    }
    void Death()
    {
        gameManager.EnableDeathScreen();
        // VARS
        Volume volume = postProcessing.GetComponent<Volume>();

        volume.profile.TryGet(out ColorAdjustments colorAdjustments);
        volume.profile.TryGet(out Vignette vignette);
        // VARS

        DisablePlayer();
        gameManager.globalDeaths += 1;

        // Toggle UI
        deathScreen.SetActive(true);

        // Post Processing Changes

        // set saturation to 0
        colorAdjustments.saturation.value = -50f;
        // increase vignette strength
        vignette.intensity.value = 0.5f;

        void DisablePlayer()
        {
            dead = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage && !invincible)
        {
            Debug.Log("Taking Damage");
            health -= damage;
            health = Mathf.Clamp(health, 0, numOfHearts);

            // effects
            StartCoroutine(cannon.GetComponent<DamageFlash>().FlashWhite());
            StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(0.3f));

            playerAudio.pitch = Random.Range(0.5f, 1.5f);
            playerAudio.PlayOneShot(hitSound, 1f * gameManager.soundVolume);
            playerAudio.pitch = 1;

            StartCoroutine(Invincibility(damageInvincibilityCooldown));
        }
    }
    public void Heal(float healingAmount)
    {
        Instantiate(healParticles, transform.position, transform.rotation);
        health += healingAmount;
        health = Mathf.Clamp(health, 0, numOfHearts);
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // set heart to a full or empty heart based on health
            if (i < health)
                hearts[i].gameObject.GetComponent<Animator>().SetBool("emptyHeart", false);
            else
                hearts[i].gameObject.GetComponent<Animator>().SetBool("emptyHeart", true);

            // enable or disable hearts based on health
            if (i < numOfHearts)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }
}