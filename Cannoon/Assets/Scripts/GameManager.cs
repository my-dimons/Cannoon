using UnityEngine;
using UnityEngine.SceneManagement;

// most game stats, difficulty, and the current level
public class GameManager : MonoBehaviour
{
    [Header("Global Player Stats")]
    [Tooltip("Players total kills")]
    public int globalKills;
    [Tooltip("Amount of kills the player has on the current stage")]
    public int currentKills;
    [Tooltip("Players total deaths")]
    public int globalDeaths;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float audioVolume; // value from 0 -> 1
    [Range(0f, 1.5f)]
    public float musicVolume;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;

    [Header("Difficulty")]
    public Difficulty difficulty;
    // Numbers are the difficulty multiplier (DIVIDE BY 100, think of it as a percentage value)
    public enum Difficulty
    {
        easy = 75,
        normal = 100, // USUAL PLAYTHROUGH
        hard = 120,
        impossible = 200
    }

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

    private void Update()
    {
        musicSource.volume = musicVolume;
        if (!musicSource.isPlaying)
        {
            PlayMusicTrack();
        }    
    }

    public void PlayMusicTrack()
    {
        int track = Random.Range(0, musicTracks.Length);
        musicSource.PlayOneShot(musicTracks[track]);
    }
}
