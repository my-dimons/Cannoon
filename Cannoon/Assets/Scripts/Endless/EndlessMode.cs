using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndlessMode : MonoBehaviour
{
    public GameObject player;
    GameManager gameManager;
    UpgradeManager upgradeManager;

    [Header("Text")] // info text

    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveCountdownText;
    public GameObject advancingWaveTexts;

    [Header("Waves")]
    [Tooltip("Current wave")]
    public int wave;
    public int healthRegen;
    [Tooltip("How many enemies are remaining?")]
    public int enemiesLeft;
    [Tooltip("Z Offset of spawning enemies (To have enemies be behind the ground slightly)")]
    public float enemyZOffset;

    public float minSpawningRandomness;
    public float maxSpawningRandomness;

    [Header("Difficulty Multiplier")]
    [Tooltip("Determines how hard the wave is")]
    public float difficultyMultiplier;
    [Tooltip("How much the difficulty multiplier increases each wave (Should probably be lower than 0.05)")]
    public float difficultyMultiplierIncrease;
    [Tooltip("If enabled, the difficulty multiplier will increase by the difficultyMultiplierIncrease every round")]
    public bool increasingDifficulty;

    [Header("Wave Transition")]
    [Tooltip("Transition time between waves (In Seconds)")]
    public int timeBetweenWaves;
    [Tooltip("How long the player is invincible for when a wave starts (in seconds)")]
    public float playerInvincibility;
    bool advancingToNextWave;
    public bool wavesStarted;

    [Header("Enemy Spawning")]
    public GameObject enemyParentObject;

    // Enemies
    [Tooltip("Total array of possible spawning enemies in this wave")]
    public GameObject[] enemies;

    [Tooltip("Enemies that can currently spawn in this wave (Based on their min/max wave)")]
    public List<GameObject> possibleSpawningEnemies;

    // Spawn Locations
    [Tooltip("Parent object of all the enemy spawn locations")]
    public GameObject enemySpawnLocationsParent;

    [Tooltip("Needs an empty game object (Or else it throws an error")]
    public List<GameObject> enemySpawnLocations;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        upgradeManager = GameObject.FindGameObjectWithTag("UpgradeManager").GetComponent<UpgradeManager>();
        foreach (Transform child in enemySpawnLocationsParent.transform)
        {
            enemySpawnLocations.Add(child.gameObject);
        }
        // Set difficulty multiplier increase based on this saves/games difficulty
        difficultyMultiplierIncrease *= (float)gameManager.difficulty / 100;
    }

    // Update is called once per frame
    void Update()
    {
        // checks how many enemies are left
        enemiesLeft = enemyParentObject.transform.childCount;

        // Starts next wave
        if (enemiesLeft <= 0 && !advancingToNextWave && wavesStarted)
        {
            if (wave != 0 && !upgradeManager.appliedUpgradeTick)
            {
                upgradeManager.upgradeTicks += 1;
                upgradeManager.appliedUpgradeTick = true;
                upgradeManager.UpdateUpgradeBars();

                if (upgradeManager.upgradeTicks == upgradeManager.baseUpgradeWaves)
                {
                    upgradeManager.pauseWaves = true;
                    StartCoroutine(upgradeManager.SpawnUpgrades());
                }
            }

            // if the wave is the upgrade wave, starting the next wave is managed on UpgradeManager.cs
            if (!upgradeManager.pauseWaves) 
                StartCoroutine(NextWave());
        }
    }

    IEnumerator NextWave()
    {
        advancingToNextWave = true;
        upgradeManager.appliedUpgradeTick = false;
        advancingWaveTexts.SetActive(true);
        currentWaveText.text = "Wave  " + wave;

        // heal player every other round
        if (wave % 2 == 0)
            player.GetComponent<PlayerHealth>().Heal(healthRegen);
        // trigger player invincibility
        StartCoroutine(player.GetComponent<PlayerHealth>().Invincibility(playerInvincibility + timeBetweenWaves));

        // seconds until next wave countdown
        int secondsUntilNextWave = timeBetweenWaves;
        waveCountdownText.text = secondsUntilNextWave.ToString();
        secondsUntilNextWave--;
        for (int i = 0; i < timeBetweenWaves + 1; i++)
        {
            yield return new WaitForSeconds(1);
            waveCountdownText.text = secondsUntilNextWave.ToString();
            secondsUntilNextWave--;
        }

        // advance wave
        waveCountdownText.text = "";
        wave++;

        // Increase difficulty (if bool is enabled)
        if (increasingDifficulty)
        {
            // DON'T activate on first wave
            if (wave != 0)
                IncreaseDifficulty();
        }

        // spawning enemy process
        SpawnEnemies();

        advancingToNextWave = false;
        advancingWaveTexts.SetActive(false);
    }

    void SpawnEnemies()
    {
        possibleSpawningEnemies.Clear();
        List<GameObject> tempEnemySpawningLocations = new(enemySpawnLocations);

        // gets all the possible spawnable enemies (by using each enemies min & max wave spawning range, unless it has wave override on)
        for (int i = 0; i < enemies.Length; i++)
            if (wave >= enemies[i].GetComponent<Enemy>().minWave && wave <= enemies[i].GetComponent<Enemy>().maxWave || enemies[i].GetComponent<Enemy>().waveOverride)
                possibleSpawningEnemies.Add(enemies[i]);

        // how many enemies to spawn (using difficulty rating)
        float amount;
        if (wave <= 20)
            amount = wave / 3.5f;
        else if (wave <= 60)
            amount = wave / 3.8f;
        else if (wave <= 120)
            amount = wave / 4.3f;
        else if (wave <= 250)
            amount = wave / 5.0f;
        else
            amount = wave / 7.6f;
        amount = Random.Range(amount * minSpawningRandomness, amount * maxSpawningRandomness);
        amount = Mathf.RoundToInt(amount);
        amount = Mathf.Clamp(amount, 1, tempEnemySpawningLocations.Count);
        Debug.Log("Spawning " + amount + " Enemies");

        // spawns all enemies
        for (int i = 0; i < amount; i++)
        {
            GameObject spawningEnemy = FindSpawnableEnemy();
            Vector3 spawnPosition = GetEnemySpawnLocation();

            // spawn enemy
            GameObject newEnemy = Instantiate(spawningEnemy, spawnPosition, spawningEnemy.transform.rotation);
            // set new enemy's parent to the object that holds all enemies (for counting and organization)
            newEnemy.transform.parent = enemyParentObject.transform;

            // might not need at the moment
            enemiesLeft++;

            Vector3 GetEnemySpawnLocation()
            {
                Vector3 pos;
                List<GameObject> spawnLocations = new();
                foreach (GameObject location in tempEnemySpawningLocations)
                    spawnLocations.Add(location);

                int randomRange = Random.Range(0, spawnLocations.Count);
                pos = new Vector3(tempEnemySpawningLocations[randomRange].transform.position.x, tempEnemySpawningLocations[randomRange].transform.position.y, enemyZOffset);
                tempEnemySpawningLocations.Remove(spawnLocations[randomRange]);
                return pos;
            }
        }
    }

    GameObject FindSpawnableEnemy()
    {
        int number = Random.Range(0, possibleSpawningEnemies.Count);
        GameObject enemy = possibleSpawningEnemies[number];
        return enemy;
    }

    private void IncreaseDifficulty()
    {
        difficultyMultiplier += difficultyMultiplierIncrease;
    }
}