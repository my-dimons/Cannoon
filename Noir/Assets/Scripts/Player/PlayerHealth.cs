using System.Collections;
using System.Collections.Generic;
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

    [Header("Respawning")]
    public Vector2 spawnPoint;
    public int respawnTime;
    public Button respawnButton;

    [Header("Health Bar")]
    public TextMeshProUGUI healthText;
    public Image healthBarImage;
    public GameObject healthBar;

    [Tooltip("The base max health")]
    public int baseHealth;
    [Tooltip("The current max health")]
    public int maxHealth;

    public float currentHealth;

    public bool canTakeDamage;
    public float damageInvincibilityCooldown;

    [Header("Other")]
    private GameManager gameManager;
    private EndlessMode endlessMode;
    IEnumerator DamageInvincibilityTimer()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageInvincibilityCooldown);
        canTakeDamage = true;
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        endlessMode = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();

        maxHealth = baseHealth;
        currentHealth = maxHealth;

        canTakeDamage = true;
    }

    private void Update()
    {        
        // FULL HEAL (TEMPERARY)
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Heal(100f);

        // Ways to die

        // No Health
        if (currentHealth <= 0 && !dead)
        {
            Death();
        }
    }
    void Death()
    {
        // VARS
        TextMeshProUGUI respawnButtonText = this.respawnButton.GetComponentInChildren<TextMeshProUGUI>();

        Volume volume = postProcessing.GetComponent<Volume>();

        volume.profile.TryGet(out ColorAdjustments colorAdjustments);
        volume.profile.TryGet(out Vignette vignette);
        // VARS

        DisablePlayer();
        gameManager.globalDeaths += 1;

        // Toggle UI
        deathScreen.SetActive(true);
        healthBar.SetActive(false);
        // Toggle UI

        // Post Processing Changes
        // set saturation to 0
        colorAdjustments.saturation.value = -50f;
        // increase vignette strength
        vignette.intensity.value = 1f;
        // Post Processing Changes


        // Respawning
        StartCoroutine(respawnButton());

        IEnumerator respawnButton()
        {
            this.respawnButton.interactable = false;

            // Countdown untill you're able to respawn
            for (int i = 0; i < respawnTime; i++)
            {
                int time = respawnTime - i;
                respawnButtonText.text = time.ToString();
                yield return new WaitForSeconds(1);
            }

            // Can Respawn
            this.respawnButton.interactable = true;
            respawnButtonText.text = "Respawn";
        }
        // Respawning


        void DisablePlayer()
        {
            dead = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            Debug.Log("Taking Damage");
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, 100);

            healthBarImage.fillAmount = currentHealth / 100;
            healthText.text = Mathf.RoundToInt(currentHealth).ToString();

            StartCoroutine(DamageInvincibilityTimer());
        }
    }
    public void Heal(float healingAmount)
    {
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);

        healthBarImage.fillAmount = currentHealth / 100;
        healthText.text = Mathf.RoundToInt(currentHealth).ToString();
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        endlessMode.wave = 0;
        endlessMode.difficultyMultiplier = 1;
        endlessMode.wavesStarted = true;
    }
}