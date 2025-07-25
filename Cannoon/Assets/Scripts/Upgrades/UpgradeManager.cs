using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public bool pauseWaves;
    public int upgrades;
    public float spawningRange;
    public bool appliedUpgradeTick;

    [Tooltip("How many waves for each upgrade")]
    public int baseUpgradeWaves;
    [Tooltip("How many base upgrade selections between special upgrade selection")]
    public int specialUpgradeWaves;
    public int difficultyIncreaseWaves;

    [Header("Upgrade Bars")]
    public Image[] upgradeBars;
    public int upgradeTicks;
    public int specialUpgradeTicks;
    public int difficultyIncreaseTicks;

    [Header("Upgrade Orbs")]
    public GameObject parentUpgradeOrb;
    public GameObject[] upgradeOrbs;
    public GameObject[] specialUpgradeOrbs;
    public List<GameObject> spawnedUpgradeOrbs;

    [Header("Specific Upgrade Orbs")]
    public GameObject chargeUpgradeOrb;
    public GameObject healthUpgradeOrb;
    public GameObject criticalChanceOrb;
    public GameObject criticalDamageOrb;
    public GameObject upgradeOrb;
    public GameObject explosionOrb;
    public GameObject autofireOrb;
    public GameObject stunOrb;
    public GameObject difficultyIncreaseOrb;
    public GameObject easierEnemies;
    public GameObject jumpHeight;
    public GameObject speedUpgrade;
    public GameObject explodeOnBounce;
    public GameObject explodeOnPierce;
    public GameObject doubleJump;
    public GameObject crown;
    public GameObject damage;

    [Header("Health Upgrade")]
    public int maxHealUpgrades;
    public int healUpgrades;

    [Header("Audio")]
    public AudioSource upgradeAudio;
    public AudioClip upgradesSpawningSound;
    public AudioClip selectionSound;
    public AudioClip hoverSound;

    [Header("Other")]
    Cannon cannonScript;
    GameObject player;
    EndlessMode endlessModeScript;
    [HideInInspector] public GameManager gameManager;

    private void Start()
    {
        cannonScript = GameObject.FindGameObjectWithTag("Cannon").GetComponent<Cannon>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        player = GameObject.FindGameObjectWithTag("Player");
        UpdateUpgradeBars();
    }

    public void UpdateUpgradeBars()
    {
        for (int i = 0; i < upgradeBars.Length; i++)
        {
            Animator anim = upgradeBars[i].GetComponent<Animator>();

            // difficulty upgrade fill
            if (i < upgradeTicks && (difficultyIncreaseTicks + 1 == difficultyIncreaseWaves))
            {
                anim.SetBool("isFilled", true);
                anim.SetBool("isDifficult", true);
            }
            // special upgrade fill
            else if (i < upgradeTicks && (specialUpgradeTicks + 1 == specialUpgradeWaves) && !(difficultyIncreaseTicks + 1 == difficultyIncreaseWaves))
            {
                anim.SetBool("isFilled", true);
                anim.SetBool("isSpecial", true);
            }
            // normal upgrade fill
            if (i < upgradeTicks && !(specialUpgradeTicks + 1 == specialUpgradeWaves) && !(difficultyIncreaseTicks + 1 == difficultyIncreaseWaves))
            {
                anim.SetBool("isFilled", true);
            }

            if (i >= upgradeTicks)
            {
                anim.SetBool("isFilled", false);
                anim.SetBool("isSpecial", false);
                anim.SetBool("isDifficult", false);
            }

            // disable upgrade bars depending on set amount
            if (i < baseUpgradeWaves)
                upgradeBars[i].enabled = true;
            else
                upgradeBars[i].enabled = false;
        }
    }

    public void SpawnUpgrades()
    {
        bool specialWave = false;
        bool difficultWave = false;
        upgradeBars[baseUpgradeWaves - 1].gameObject.GetComponent<Animator>().SetBool("isFilled", true);
        endlessModeScript.spawningEnemiesAmount += 1;
        UpdateUpgradeBars();

        if (difficultyIncreaseTicks <= difficultyIncreaseWaves)
        {
            difficultWave = false;
            difficultyIncreaseTicks += 1;
        }
        if (difficultyIncreaseTicks == difficultyIncreaseWaves)
        {
            difficultWave = true;
            difficultyIncreaseTicks = 0;
        }

        // special wave
        if (specialUpgradeTicks <= specialUpgradeWaves && !difficultWave)
        {
            specialWave = false;
            specialUpgradeTicks += 1;
        }
        if (specialUpgradeTicks == specialUpgradeWaves)
        {
            specialWave = true;
            specialUpgradeTicks = 0;
        }

        StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(0.4f));
        StartCoroutine(SelectAndSpawnUpgrades(specialWave, difficultWave));
    }

    IEnumerator SelectAndSpawnUpgrades(bool specialWave, bool difficultWave)
    {
        yield return new WaitForSeconds(0.8f);

        List<GameObject> availableUpgradeOrbs = new();
        availableUpgradeOrbs = SelectUpgrades(availableUpgradeOrbs, specialWave, difficultWave);
        upgradeAudio.PlayOneShot(upgradesSpawningSound, 1f * gameManager.soundVolume);

        upgradeTicks = 0;

        int spawnUpgrades = Mathf.Clamp(upgrades, 1, availableUpgradeOrbs.Count);
        List<GameObject> pickedUpgrades = new();
        // pick random upgrades
        for (int i = 0; i < spawnUpgrades; i++)
        {
            int num = Random.Range(0, availableUpgradeOrbs.Count);
            pickedUpgrades.Add(availableUpgradeOrbs[num]);
            availableUpgradeOrbs.Remove(availableUpgradeOrbs[num]);
        }

        for (int i = 0; i < pickedUpgrades.Count; i++)
        {
            float spawnRangeLength = spawningRange * 2;
            float distinceBetweenUpgrades = spawnRangeLength / (pickedUpgrades.Count - 1);
            Vector3 spawnPos = new(-spawningRange + (distinceBetweenUpgrades * i), 0, 0);
            if (pickedUpgrades.Count <= 1)
                spawnPos = Vector3.zero;

            GameObject spawnObj = Instantiate(pickedUpgrades[i], parentUpgradeOrb.transform);
            spawnObj.GetComponent<RectTransform>().localPosition = spawnPos;
            spawnedUpgradeOrbs.Add(spawnObj);
        }

        availableUpgradeOrbs.Clear();

        UpdateUpgradeBars();
    }

    private List<GameObject> SelectUpgrades(List<GameObject> availableUpgradeOrbs, bool specialWave, bool difficultWave)
    {
        if (difficultWave)
        {
            availableUpgradeOrbs.Add(difficultyIncreaseOrb);
            return availableUpgradeOrbs;
        }
        if (!specialWave && !difficultWave)
        {
            for (int i = 0; i < upgradeOrbs.Length; i++)
                availableUpgradeOrbs.Add(upgradeOrbs[i]);

            // more fine selection

            // max crit chance
            if (cannonScript.criticalStrikeChance >= 100)
                availableUpgradeOrbs.Remove(criticalChanceOrb);
            // crit damage
            if (cannonScript.criticalStrikeChance < 40)
                availableUpgradeOrbs.Remove(criticalDamageOrb);
            // max health
            if (healUpgrades >= maxHealUpgrades)
                availableUpgradeOrbs.Remove(healthUpgradeOrb);
            // charge time
            if (cannonScript.maxCharge <= cannonScript.chargeLimit)
                availableUpgradeOrbs.Remove(chargeUpgradeOrb);
            // easier enemies
            if (endlessModeScript.difficultyMultiplier <= 0.75f)
                availableUpgradeOrbs.Remove(easierEnemies);
            // jump
            if (player.GetComponent<PlayerMovement>().jumpForce > player.GetComponent<PlayerMovement>().jumpForceLimit)
                availableUpgradeOrbs.Remove(jumpHeight);
            // damage
            if (cannonScript.maxBulletDamage >= 60)
                availableUpgradeOrbs.Remove(damage);
            // speed
            if (player.GetComponent<PlayerMovement>().baseSpeed > 34)
                availableUpgradeOrbs.Remove(speedUpgrade);
        } 
        if (specialWave)
        {
            for (int i = 0; i < specialUpgradeOrbs.Length; i++)
                availableUpgradeOrbs.Add(specialUpgradeOrbs[i]);

            if (cannonScript.explodingBullets)
                availableUpgradeOrbs.Remove(explosionOrb);

            if (!(cannonScript.explodingBullets && cannonScript.bounces > 0 && !cannonScript.explodeOnBounce))
                availableUpgradeOrbs.Remove(explodeOnBounce);
            if (!(cannonScript.explodingBullets && cannonScript.pierces > 0 && !cannonScript.explodeOnPierce))
                availableUpgradeOrbs.Remove(explodeOnPierce);

            if (player.GetComponent<PlayerMovement>().doubleJump)
                availableUpgradeOrbs.Remove(doubleJump);
            if (cannonScript.autofire)
                availableUpgradeOrbs.Remove(autofireOrb);
            if (cannonScript.crown.activeSelf)
                availableUpgradeOrbs.Remove(crown);
            if (cannonScript.stunEnemies)
                availableUpgradeOrbs.Remove(stunOrb);
            if (upgrades >= 5)
                availableUpgradeOrbs.Remove(upgradeOrb);
        }

        return availableUpgradeOrbs;
    }

    public void FinishPickingUpgrades(bool reRoll, bool specialReRoll)
    {
        upgradeAudio.PlayOneShot(selectionSound, 1f * gameManager.soundVolume); 
        for (int i = 0; i < spawnedUpgradeOrbs.Count; i++)
            Destroy(spawnedUpgradeOrbs[i]);
        StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(0.4f));
        if (reRoll)
        {
            StartCoroutine(SelectAndSpawnUpgrades(specialReRoll, false));
        } else
        {
            StartCoroutine(StartWaves());

            player.GetComponent<PlayerHealth>().Heal(endlessModeScript.healthRegen);

            spawnedUpgradeOrbs.Clear();
        }
    }

    IEnumerator StartWaves()
    {
        yield return new WaitForSeconds(1.5f);
        pauseWaves = false;
    }
}
