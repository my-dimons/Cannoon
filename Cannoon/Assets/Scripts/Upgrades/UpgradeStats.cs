using System.Collections;
using System.Collections.Generic;
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

    PlayerMovement playerMovementScript;
    PlayerHealth playerHealthScript;
    EndlessMode endlessModeScript;
    Upgrade upgradeScript;
    Cannon cannonScript;
    // Start is called before the first frame update
    void Start()
    {
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
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
        cannonScript.maxPower += bulletSpeedIncrease;
        cannonScript.baseSizeMult += sizeMultIncrease;
        endlessModeScript.difficultyMultiplier += waveDifficultyIncrease;

        // crits
        cannonScript.criticalStrikeChance += criticalChanceIncrease;
        cannonScript.baseCritDamageMult += criticalDamageMultIncrease;

        // health
        playerHealthScript.numOfHearts += health;
        endlessModeScript.healthRegen += regen;
        upgradeScript.Pick();
    }
}
