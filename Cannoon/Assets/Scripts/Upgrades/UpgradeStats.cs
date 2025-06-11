using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using UnityEngine;

public class UpgradeStats : MonoBehaviour
{
    [Header("Movement")]
    public float jumpHeightIncrease;
    public float speedIncrease;

    [Header("Damage")]
    public float damageIncrease;
    public float chargeSpeedIncrease;
    public float chargeLimitIncrease;
    public float sizeMultIncrease;
    public float bulletSpeedIncrease;

    [Header("Criticals")]
    public float criticalChanceIncrease;
    public float criticalDamageMultIncrease;

    [Header("Enemies")]
    public float waveDifficultyIncrease;

    [Header("Health")]
    public int health;
    public int regen;

    [Header("Special")]
    public int bounces;
    public int pierces;
    public float immunityIncrease;
    public bool unlockAutofire;
    public bool explodingBullets;
    public bool explodeOnBounce;
    public bool explodeOnPierce;
    public bool doubleJump;
    public int spawnUpgradesIncrease;

    [Header("Difficulty Increase")]
    public int increaseUpgradeBar;
    public float difficultyUpgradeTicksDivisor;
    public float increaseDifficultyIncreasePercent;

    [Header("Random")]
    public bool enableCrown;
    public bool reRoll;
    public bool specialReRoll;

    PlayerMovement playerMovementScript;
    PlayerHealth playerHealthScript;
    UpgradeManager upgradeManager;
    EndlessMode endlessModeScript;
    Upgrade upgradeScript;
    Cannon cannonScript;
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        upgradeManager = GameObject.FindGameObjectWithTag("UpgradeManager").GetComponent<UpgradeManager>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        cannonScript = GameObject.FindGameObjectWithTag("Cannon").GetComponent<Cannon>();
        upgradeScript = GetComponent<Upgrade>();
    }
    public void IncreaseStats()
    {
        // movement
        playerMovementScript.baseJumpForce += jumpHeightIncrease;
        playerMovementScript.baseSpeed += speedIncrease;

        playerMovementScript.speed = playerMovementScript.baseSpeed;
        playerMovementScript.jumpForce = playerMovementScript.baseJumpForce;

        // damage
        cannonScript.maxBulletDamage += damageIncrease;
        cannonScript.maxCharge += chargeSpeedIncrease;
        cannonScript.chargeLimit += chargeLimitIncrease;
        cannonScript.maxPower += bulletSpeedIncrease;
        cannonScript.baseSizeMult += sizeMultIncrease;
        endlessModeScript.difficultyMultiplier += waveDifficultyIncrease;

        // crits
        cannonScript.criticalStrikeChance += criticalChanceIncrease;
        cannonScript.baseCritDamageMult += criticalDamageMultIncrease;

        // health
        playerHealthScript.numOfHearts += health;
        endlessModeScript.healthRegen += regen;

        // special
        cannonScript.bounces += bounces;
        cannonScript.pierces += pierces;
        upgradeManager.upgrades += spawnUpgradesIncrease;
        playerHealthScript.damageInvincibilityCooldown += immunityIncrease;
        if (!cannonScript.explodingBullets)
            cannonScript.explodingBullets = explodingBullets;
        if (!cannonScript.autofire)
            cannonScript.autofire = unlockAutofire;
        if (!cannonScript.explodeOnBounce)
            cannonScript.explodeOnBounce = explodeOnBounce;
        if (!cannonScript.explodeOnPierce)
            cannonScript.explodeOnPierce = explodeOnPierce;
        if (!playerMovementScript.doubleJump)
            playerMovementScript.doubleJump = doubleJump;

        // difficulty upgrade
        endlessModeScript.difficultyMultiplierIncrease += endlessModeScript.difficultyMultiplierIncrease / 100 * increaseDifficultyIncreasePercent;
        endlessModeScript.difficultyMultiplier += endlessModeScript.difficultyMultiplier / 100 * increaseDifficultyIncreasePercent;
        upgradeManager.baseUpgradeWaves += increaseUpgradeBar;
        if (difficultyUpgradeTicksDivisor != 0)
            upgradeManager.difficultyIncreaseWaves = Mathf.RoundToInt(upgradeManager.difficultyIncreaseWaves / difficultyUpgradeTicksDivisor);
        upgradeManager.UpdateUpgradeBars();

        // random
        if (enableCrown)
            cannonScript.crown.SetActive(true);

        upgradeScript.Pick(reRoll, specialReRoll);
    }
}
