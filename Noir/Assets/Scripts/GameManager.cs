using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Global Player Stats")]
    public int globalKills;
    public int globalDeaths;

    [Header("Difficulty")]
    int test;
    // Numbers are the difficulty multiplier (DIVIDE BY 100, think of it almost as a percentage value)
    public enum Difficulty
    {
        easy = 75,
        normal = 100, // USUAL PLAYTHROUGH
        hard = 120,
        impossible = 200
    }

    public Difficulty difficulty;


    // Loads a specified SCENE
    public void LoadLevel(string scene)
    {
        SceneManager.LoadScene(scene);
        Debug.Log("Loading " + scene);
    }

    // Closes the application
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
