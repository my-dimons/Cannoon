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

    [Header("Health")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Tooltip("players max amount of hearts")]
    public int numOfHearts;
    [Tooltip("players current health/hearts")]
    public float health;

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
        // Toggle UI

        // Post Processing Changes
        // set saturation to 0
        colorAdjustments.saturation.value = -50f;
        // increase vignette strength
        vignette.intensity.value = 0.5f;
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
            health -= damage;
            health = Mathf.Clamp(health, 0, numOfHearts);

            StartCoroutine(DamageInvincibilityTimer());
        }
    }
    public void Heal(float healingAmount)
    {
        health += healingAmount;
        health = Mathf.Clamp(health, 0, numOfHearts);
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        endlessMode.wave = 0;
        endlessMode.difficultyMultiplier = 1;
        endlessMode.wavesStarted = true;
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