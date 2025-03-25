using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class EndlessMode : MonoBehaviour
{
    public GameObject player;
    GameManager gameManager;

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
    [Tooltip("Z Offset of spawning enemies (To have enemies be behind the ground slightly)")]
    public float enemyZOffset;
    [Tooltip("Determines how hard the wave is")]
    public float difficultyMultiplier;
    [Tooltip("How much the difficulty multiplier increases each wave (Should probably be lower than 0.05)")]
    public float difficultyMultiplierIncrease;

    [Tooltip("Transition time between waves (In Seconds)")]
    public int timeBetweenWaves;

    bool advancingToNextWave;

    [Header("Enemy Spawning")]
    public GameObject enemyParentObject;

    // Enemies
    [Tooltip("Total array of possible spawning enemies in this wave")]
    public GameObject[] enemies;

    [Tooltip("Enemies that can currently spawn in this wave (Based on their min/max wave)")]
    public List<GameObject> possibleSpawningEnemies;

    // Spawn Locations
    [Tooltip("The lists of the different stages spawn locations (The Parent Object), PUT THE STAGES IN THIS LEVELS PROPER ORDER")]
    public List<GameObject> parentEnemySpawnLocations;

    [Tooltip("Needs an empty game object (Or else it throws an error")]
    public List<GameObject> possibleEnemySpawnLocations;


    [Header("Level Stages")]
    
    // LEVEL is the parent color/name of stages (ex. Green Level)
    // =--=
    // STAGES are the subdivisions of the parent level (ex. Meadow & Forest stages of the Green level)

    public string levelColor; // TODO: Migrate to enum in GameManager (STILL SET COLOR HERE, JUST CHANGE STRING TO THE NEW ENUM)

    [Tooltip("Different stages in this level")]
    public List<string> stages;

    [Tooltip("Different stages in this level")]
    public string currentStage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        // Set difficulty multiplier increase based on this saves/games difficulty
        difficultyMultiplierIncrease *= (float)gameManager.difficulty / 100;

        //starts first round
        StartCoroutine(NextWave());

        AdvanceStage(0);
    }

    // Gets available spawning locations from a stages parent object
    // i corresponds to the stage you're advancing to (0 = first stage; 1 = second stage; etc.)
    private void AdvanceStage(int i)
    {
        // Set current stage
        currentStage = stages[i];

        // Get next stages spawning positions
        foreach (Transform x in parentEnemySpawnLocations[i].GetComponentInChildren<Transform>())
        {
            possibleEnemySpawnLocations.Add(x.gameObject);
        }
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
        amount = Mathf.Clamp(amount, 1, possibleEnemySpawnLocations.Count);

        // spawns all enemies
        for (int i = 0; i < amount; i++)
        {
            GameObject spawningEnemy = FindSpawnableEnemy();
            Enemy spawningEnemyScript = spawningEnemy.GetComponent<Enemy>();
            Vector3 spawnPosition = Vector3.zero;

            // spawns a ground enemy
            if (!spawningEnemyScript.flyingEnemy && !spawningEnemyScript.waterEnemy)
                spawnPosition = GetEnemySpawnLocation(false, false);
            // spawns a flying enemy
            else if (spawningEnemyScript.flyingEnemy && !spawningEnemyScript.waterEnemy)
                spawnPosition = GetEnemySpawnLocation(true, false);
            // spawns a water enemy
            else if (!spawningEnemyScript.flyingEnemy && spawningEnemyScript.waterEnemy)
                spawnPosition = GetEnemySpawnLocation(false, true);

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
    
    // Fetches a random enemy spawning location, if everything is false
    Vector3 GetEnemySpawnLocation(bool flyingEnemy, bool waterEnemy)
    {
        Vector3 pos;
        List<GameObject> spawnLocations = new List<GameObject>();
        foreach (GameObject location in possibleEnemySpawnLocations)
        {
            SpawnPosition locationScript = location.GetComponent<SpawnPosition>();

            // FLYING ENEMIES
            if (flyingEnemy && locationScript.flying && !waterEnemy && !locationScript.water)
                spawnLocations.Add(location);
            // WATER ENEMIES
            else if (waterEnemy && locationScript.water && !flyingEnemy && !locationScript.flying)
                spawnLocations.Add(location);
            else if (!flyingEnemy && !waterEnemy && !locationScript.flying && !locationScript.water)
                spawnLocations.Add(location);
        }

        pos = FetchPos();

        return pos;

        Vector3 FetchPos()
        {
            Vector3 pos;
            int randomRange = Random.Range(0, spawnLocations.Count);
            pos = new Vector3(possibleEnemySpawnLocations[randomRange].transform.position.x, possibleEnemySpawnLocations[randomRange].transform.position.y, enemyZOffset);
            possibleEnemySpawnLocations.Remove(spawnLocations[randomRange]);
            return pos;
        }
    }
}