using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndlessMode : MonoBehaviour
{
    public GameObject player;
    GameManager gameManager;

    [Header("Text")] // info text

    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveCountdownText;
    public GameObject advancingWaveTexts;

    [Header("Waves")]
    [Tooltip("Current wave")]
    public int wave;
    [Tooltip("How many enemies are remaining?")]
    public int enemiesLeft;
    [Tooltip("Z Offset of spawning enemies (To have enemies be behind the ground slightly)")]
    public float enemyZOffset;
    [Tooltip("Determines how hard the wave is")]
    public float difficultyMultiplier;
    [Tooltip("How much the difficulty multiplier increases each wave (Should probably be lower than 0.05)")]
    public float difficultyMultiplierIncrease;

    [Tooltip("Transition time between waves (In Seconds)")]
    public int timeBetweenWaves;

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

        // starts first round
        if (enemiesLeft <= 0 && !advancingToNextWave && wavesStarted)
            StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        advancingToNextWave = true;
        advancingWaveTexts.SetActive(true);
        currentWaveText.text = "Wave  " + wave;

        // heal player (by 1 heart)
        player.GetComponent<PlayerHealth>().Heal(1);

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

        // DON'T activate off first wave
        if (wave != 1)
            difficultyMultiplier += difficultyMultiplierIncrease;

        // spawning enemy process
        SpawnEnemies();

        advancingToNextWave = false;
        advancingWaveTexts.SetActive(false);
    }

    void SpawnEnemies()
    {
        possibleSpawningEnemies.Clear();
        List<GameObject> tempEnemySpawningLocations = new(enemySpawnLocations);

        // gets all the possible spawnable enemies (by using each enemies min & max difficulty spawning range)
        for (int i = 0; i < enemies.Length; i++)
            if (difficultyMultiplier >= enemies[i].GetComponent<Enemy>().minWave && difficultyMultiplier <= enemies[i].GetComponent<Enemy>().maxWave)
                possibleSpawningEnemies.Add(enemies[i]);

        // how many enemies to spawn (using difficulty rating)
        float amount = wave/2;
        amount = Mathf.Clamp(amount, 1, tempEnemySpawningLocations.Count);

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
}