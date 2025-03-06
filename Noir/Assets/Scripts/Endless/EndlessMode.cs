using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class EndlessMode : MonoBehaviour
{
    public GameObject player;

    [Header("Text")]
    public TextMeshPro waveText;
    public TextMeshPro killsText;
    public TextMeshPro enemiesLeftText;

    public TextMeshPro waveCountdownText;

    [Header("Waves")]
    public int wave;
    public int enemiesLeft;
    public float difficultyRating;

    public int timeBetweenWaves;
    bool advancingToNextWave;

    public GameObject enemyParentObject;

    public GameObject[] enemySpawnLocations;
    public List<GameObject> possibleEnemySpawnLocations;

    [Header("Enemies")]
    public GameObject[] enemies;
    public List<GameObject> possibleSpawningEnemies;
    // Start is called before the first frame update
    void Start()
    {
        waveCountdownText.text = "";

        ResetPossibleEnemySpawnLocations();
        StartCoroutine(NextWave());
    }

    // Update is called once per frame
    void Update()
    {
        enemiesLeft = enemyParentObject.transform.childCount;
        waveText.text =  "Wave   " + wave;
        killsText.text = player.GetComponent<Player>().kills + "   Kills";
        enemiesLeftText.text = enemiesLeft + "   Left";

        if (enemiesLeft <= 0 && !advancingToNextWave)
            StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        advancingToNextWave = true;
        int secondsTillNextWave = timeBetweenWaves;
        waveCountdownText.text = secondsTillNextWave + "s Until Next Wave";
        secondsTillNextWave--;
        for (int i = 0; i < timeBetweenWaves + 1; i++)
        {
            yield return new WaitForSeconds(1);
            waveCountdownText.text = secondsTillNextWave + "s Until Next Wave";
            secondsTillNextWave--;
        }

        waveCountdownText.text = "";
        wave++;
        difficultyRating = wave;

        SpawnEnemies();
        ResetPossibleEnemySpawnLocations();

        advancingToNextWave = false;
    }

    void SpawnEnemies()
    {
        possibleSpawningEnemies.Clear();
        for (int i = 0; i < enemies.Length; i++)
            if (difficultyRating >= enemies[i].GetComponent<Enemy>().minDifficulty && difficultyRating <= enemies[i].GetComponent<Enemy>().maxDifficulty)
                possibleSpawningEnemies.Add(enemies[i]);

        //int amount = Mathf.RoundToInt(difficultyRating);
        float amount = difficultyRating/2;
        amount = Mathf.Clamp(amount, 1, enemySpawnLocations.Length);

        for (int i = 0; i < amount; i++)
        {
            GameObject spawningEnemy = FindSpawnableEnemy();
            Vector3 spawnPosition = GetEnemySpawnLocation();

            GameObject newEnemy = Instantiate(spawningEnemy, spawnPosition, spawningEnemy.transform.rotation);
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

    Vector3 GetEnemySpawnLocation()
    {
        Vector3 pos;
        int randomRange = Random.Range(0, possibleEnemySpawnLocations.Count);
        pos = possibleEnemySpawnLocations[randomRange].transform.position;
        possibleEnemySpawnLocations.Remove(possibleEnemySpawnLocations[randomRange]);
        return pos;
    }

    void ResetPossibleEnemySpawnLocations() // sets possibleEnemySpawnLocations to enemySpawnLocations
    {
        possibleEnemySpawnLocations.Clear();
        foreach (GameObject location in enemySpawnLocations)
        {
            possibleEnemySpawnLocations.Add(location);
        }
    }
}