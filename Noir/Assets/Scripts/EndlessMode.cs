using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public float difficultyRating;

    public GameObject enemyCounterObject;
    public GameObject[] enemySpawnLocations;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        enemiesLeft = enemyCounterObject.transform.childCount;
        waveText.text =  "Wave   " + wave;
        killsText.text = player.GetComponent<Player>().kills + "   Kills";
        enemiesLeftText.text = enemiesLeft + "   Left";

        if (enemiesLeft <= 0)
            NextWave();
    }

    void NextWave()
    {
        wave++;
        difficultyRating *= Random.Range(1.15f, 1.25f);
    }
}
