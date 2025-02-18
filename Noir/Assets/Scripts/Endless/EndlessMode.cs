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

    [Header("Waves")]
    public int wave;
    public int enemiesLeft;
    public int difficultyRating;

    public GameObject enemyParentObject;
    public GameObject[] enemySpawnLocations;
    public List<GameObject> possibleEnemySpawnLocations;

    [Header("Enemies")]
    public GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        ResetPossibleEnemySpawnLocations();
        NextWave();
    }

    // Update is called once per frame
    void Update()
    {
        enemiesLeft = enemyParentObject.transform.childCount;
        waveText.text =  "Wave   " + wave;
        killsText.text = player.GetComponent<Player>().kills + "   Kills";
        enemiesLeftText.text = enemiesLeft + "   Left";

        if (enemiesLeft <= 0)
            NextWave();
    }

    void NextWave()
    {
        wave++;
        difficultyRating++;
        SpawnEnemies();
        ResetPossibleEnemySpawnLocations();
        //difficultyRating *= Random.Range(1.15f, 1.25f);
    }

    void SpawnEnemies()
    {
        //int amount = Mathf.RoundToInt(difficultyRating);
        int amount = difficultyRating;
        amount = Mathf.Clamp(amount, 1, enemySpawnLocations.Length);

        for (int i = 0; i < amount; i++)
        {
            GameObject spawningEnemy = FindSpawnableEnemy();
            Vector3 spawnPosition = GetEnemySpawnLocation();

            GameObject newEnemy = Instantiate(spawningEnemy, spawnPosition, spawningEnemy.transform.rotation);
            newEnemy.transform.parent = enemyParentObject.transform;
            newEnemy.GetComponent<EnemyAI>().target = player.transform;

            // might not need at the moment
            enemiesLeft++;
        }
    }

    GameObject FindSpawnableEnemy()
    {
        GameObject enemy = enemies[Random.Range(0, enemies.Length)];
        return enemy;
    }

    Vector3 GetEnemySpawnLocation()
    {
        Vector3 pos;
        int randomRange = Random.Range(0, possibleEnemySpawnLocations.Count);
        pos = possibleEnemySpawnLocations[randomRange].transform.position;

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
