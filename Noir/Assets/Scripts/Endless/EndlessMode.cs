using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class EndlessMode : MonoBehaviour
{
    public GameObject player;

    [Header("Text")] // info text
    public TextMeshPro waveText;
    public TextMeshPro killsText;
    public TextMeshPro enemiesLeftText;

    public TextMeshPro waveCountdownText;

    [Header("Waves")]
    [Tooltip("Current wave")]
    public int wave;
    [Tooltip("How many enemies are remaining?")]
    public int enemiesLeft;

    [Tooltip("Determines how hard the wave is")]
    public float difficultyMultiplier;
    [Tooltip("How much the difficulty multiplier increases each wave (Should probably be lower 0.1)")]
    public float difficultyMultiplierIncrease;

    [Tooltip("Transition time between waves (In Seconds)")]
    public int timeBetweenWaves;

    bool advancingToNextWave;

    public GameObject enemyParentObject;
    [Header("Enemies")]
    // Enemies
    public GameObject[] enemies;
    [Tooltip("Needs an empty game object (Or else it throws an error")]
    public List<GameObject> possibleSpawningEnemies;

    // Spawn Locations
    public GameObject[] enemySpawnGroundLocations;
    public GameObject[] enemySpawnSkyLocations;

    [Tooltip("Needs an empty game object (Or else it throws an error")]
    public List<GameObject> possibleEnemySpawnGroundLocations;
    [Tooltip("Needs an empty game object (Or else it throws an error")]
    public List<GameObject> possibleEnemySpawnSkyLocations;

    // Start is called before the first frame update
    void Start()
    {
        // Resets
        waveCountdownText.text = "";
        ResetPossibleEnemySpawnLocations();

        //starts first round
        StartCoroutine(NextWave());
    }

    // Update is called once per frame
    void Update()
    {
        // checks how many enemies are left
        enemiesLeft = enemyParentObject.transform.childCount;

        // update info text
        waveText.text =  "Wave   " + wave;
        killsText.text = player.GetComponent<Player>().kills + "   Kills";
        enemiesLeftText.text = enemiesLeft + "   Left";

        // starts first round
        if (enemiesLeft <= 0 && !advancingToNextWave)
            StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        advancingToNextWave = true;

        // seconds until next wave countdown
        int secondsUntilNextWave = timeBetweenWaves;
        waveCountdownText.text = secondsUntilNextWave + "s Until Next Wave";
        secondsUntilNextWave--;
        for (int i = 0; i < timeBetweenWaves + 1; i++)
        {
            yield return new WaitForSeconds(1);
            waveCountdownText.text = secondsUntilNextWave + "s Until Next Wave";
            secondsUntilNextWave--;
        }

        // advance wave
        waveCountdownText.text = "";
        wave++;
        // DON'T activate of first wave
        if (wave != 1)
            difficultyMultiplier += difficultyMultiplierIncrease;

        // spawning enemy process
        SpawnEnemies();
        ResetPossibleEnemySpawnLocations();

        advancingToNextWave = false;
    }

    void SpawnEnemies()
    {
        possibleSpawningEnemies.Clear();

        // gets all the possible spawnable enemies (by using each enemies min & max difficulty spawning range)
        for (int i = 0; i < enemies.Length; i++)
            if (difficultyMultiplier >= enemies[i].GetComponent<Enemy>().minWave && difficultyMultiplier <= enemies[i].GetComponent<Enemy>().maxWave)
                possibleSpawningEnemies.Add(enemies[i]);

        // how many enemies to spawn (using difficulty rating)
        float amount = wave/2;
        amount = Mathf.Clamp(amount, 1, possibleSpawningEnemies.Count);

        // spawns all enemies
        for (int i = 0; i < amount; i++)
        {
            GameObject spawningEnemy = FindSpawnableEnemy();
            Vector3 spawnPosition;
            
            // if spawning enemy is a ground enemy: get a ground spawning position
            if (spawningEnemy.GetComponent<Enemy>().flyingEnemy == false)
            {
                spawnPosition = GetEnemySpawnLocation(true);
            }
            // else (if spawning enemy is a sky/flying enemy): get a sky position
            else
            {
                spawnPosition = GetEnemySpawnLocation(false);
            }

            // spawn enemy
            GameObject newEnemy = Instantiate(spawningEnemy, spawnPosition, spawningEnemy.transform.rotation);
            // set new enemy's parent to the object that holds all enemies (for counting and organization)
            newEnemy.transform.parent = enemyParentObject.transform;

            // might not need at the moment
            enemiesLeft++;
        }
    }

    GameObject FindSpawnableEnemy()
    {
        int number = Random.Range(0, possibleSpawningEnemies.Count);
        GameObject enemy = possibleSpawningEnemies[number];
        return enemy;
    }

    Vector3 GetEnemySpawnLocation(bool isGroundEnemy)
    {
        Vector3 pos;
        
        // fetches a random GROUND enemy spawn location
        if (isGroundEnemy)
        {
            int randomRange = Random.Range(0, possibleEnemySpawnGroundLocations.Count);
            pos = possibleEnemySpawnGroundLocations[randomRange].transform.position;
            possibleEnemySpawnGroundLocations.Remove(possibleEnemySpawnGroundLocations[randomRange]);
        }
        // fetches a random FLYING enemy spawn location
        else
        {
            int randomRange = Random.Range(0, possibleEnemySpawnSkyLocations.Count);
            pos = possibleEnemySpawnSkyLocations[randomRange].transform.position;
            possibleEnemySpawnSkyLocations.Remove(possibleEnemySpawnSkyLocations[randomRange]);
        }
        return pos;
    }

    void ResetPossibleEnemySpawnLocations() // sets possibleEnemySpawnLocations to enemySpawnLocations
    {
        possibleEnemySpawnGroundLocations.Clear();
        possibleEnemySpawnSkyLocations.Clear();

        // resets GROUND enemy locations
        foreach (GameObject location in enemySpawnGroundLocations)
        {
            possibleEnemySpawnGroundLocations.Add(location);
        }
        // resets FLYING enemy locations
        foreach (GameObject location in enemySpawnSkyLocations)
        {
            possibleEnemySpawnSkyLocations.Add(location);
        }
    }
}