using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// most game stats, difficulty, and the current level
public class GameManager : MonoBehaviour
{
    [Header("Global Player Stats")]
    public int waveHighscore;
    [Tooltip("Players total kills")]
    public int globalKills;
    [Tooltip("Amount of kills the player has on the current stage")]
    public int currentKills;
    [Tooltip("Players total deaths")]
    public int globalDeaths;
    public float timePlayed;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float soundVolume; // value from 0 -> 1
    [Range(0f, 1f)]
    public float musicVolume;

    [Header("Music/Sfx")]
    public AudioSource musicSource;
    public AudioSource audioSource;
    public AudioClip deathSfx;
    public AudioClip[] musicTracks;
    public AudioClip musicCurrentlyPlaying;
    public float musicTransitionTime;
    public bool playingMusic;

    [Header("Pause Menu")]
    public static GameObject pauseMenu;
    public static Slider musicVolumeSlider;
    public static Slider SfxVolumeSlider;
    public static GameObject quitButton;
    public static GameObject resumeButton;

    [Header("Credit Screen")]
    public static GameObject creditsButton;
    public static GameObject creditScreen;
    public static GameObject disableCreditsButton;

    [Header("Death Screen")]
    public static GameObject deathScreen;
    public static GameObject deathQuitButton;
    public static GameObject deathRespawnButton;
    public static GameObject deathWaveText;
    public static GameObject deathTimeText;
    public static GameObject deathKillsText;
    public static GameObject deathWaveHighscore;

    public bool pauseMenuEnabled;
    public bool creditScreenEnabled;

    [Header("Difficulty")]
    public float difficulty;
    GameObject difficultyMenu;
    public float[] difficultyValues;

    public static GameManager Instance;


    // reloads the main scene
    public void Respawn()
    {
        SceneManager.LoadScene("Main Scene");
        Debug.Log("Respawning");
    }

    // Closes the application
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }

    private void Update()
    {
        timePlayed += Time.deltaTime;
        musicSource.volume = musicVolume;

        if (Input.GetKeyDown(KeyCode.Escape) && !creditScreenEnabled)
        {
            if (!pauseMenuEnabled)
                PauseGame();
            else
                ResumeGame();
        }

        if (!playingMusic)
        {
            StartCoroutine(PlayMusicTrack());
        }
    }
    private void Awake()
    {
        difficulty = difficultyValues[0];
        pauseMenu = GameObject.Find("Pause Menu");
        musicVolumeSlider = pauseMenu.transform.Find("Music Volume Slider").GetComponent<Slider>();
        SfxVolumeSlider = pauseMenu.transform.Find("SFX Volume Slider").GetComponent<Slider>();
        quitButton = pauseMenu.transform.Find("Quit Button").gameObject;
        resumeButton = pauseMenu.transform.Find("Resume Button").gameObject;

        // death screen vars
        deathScreen = GameObject.Find("Death Screen");
        deathQuitButton = deathScreen.transform.Find("Quit Button").gameObject;
        deathRespawnButton = deathScreen.transform.Find("Respawn Button").gameObject;
        deathWaveText = deathScreen.transform.Find("Wave").gameObject;
        deathTimeText = deathScreen.transform.Find("Time").gameObject;
        deathKillsText = deathScreen.transform.Find("Kills").gameObject;
        deathWaveHighscore = deathScreen.transform.Find("Wave Highscore").gameObject;

        // credit screen vars
        creditScreen = GameObject.Find("Credit Screen");
        disableCreditsButton = creditScreen.transform.Find("Close Credit Screen").gameObject;
        creditsButton = pauseMenu.transform.Find("Credits Button").gameObject;

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
   
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PlayMusicTrack());
        currentKills = 0;
        timePlayed = 0;

        Time.timeScale = 1;
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathQuitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathRespawnButton.GetComponent<Button>().onClick.AddListener(Respawn);
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
        creditsButton.GetComponent<Button>().onClick.AddListener(EnableCreditScreen);
        disableCreditsButton.GetComponent<Button>().onClick.AddListener(DisableCreditScreen);

        difficultyMenu = GameObject.Find("Difficulty Dropdown").gameObject;
        difficultyMenu.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { ChangeDifficulty(difficultyMenu.GetComponent<TMP_Dropdown>()); });
        ChangeDifficulty(difficultyMenu.GetComponent<TMP_Dropdown>());

        musicVolumeSlider.onValueChanged.AddListener((v) =>
        {
            Instance.musicVolume = v;
        });
        SfxVolumeSlider.onValueChanged.AddListener((v) =>
        {
            Instance.soundVolume = v;
        });

        musicVolumeSlider.value = Instance.musicVolume;
        SfxVolumeSlider.value = Instance.soundVolume;
    }

    IEnumerator PlayMusicTrack()
    {
        Debug.Log("Playing music");
        musicSource.Stop();
        playingMusic = true;
        int track = UnityEngine.Random.Range(0, musicTracks.Length);
        musicSource.PlayOneShot(musicTracks[track]);
        musicCurrentlyPlaying = musicTracks[track];
        yield return new WaitForSeconds(musicTracks[track].length);
        musicSource.Stop();
        musicCurrentlyPlaying = null;
        yield return new WaitForSeconds(musicTransitionTime);
        playingMusic = false;
    }

    public void PauseGame()
    {
        if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().dead)
        {
            pauseMenu.GetComponent<RectTransform>().localPosition = Vector3.zero;
            Time.timeScale = 0;
            pauseMenuEnabled = true;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.GetComponent<RectTransform>().localPosition = new(0, 2000, 0);
        pauseMenuEnabled = false;
        if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().dead)
            Time.timeScale = 1;
    }

    public void EnableDeathScreen()
    {
        // set highscore
        if (GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().wave > waveHighscore)
            waveHighscore = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().wave;

        // audio
        audioSource.PlayOneShot(deathSfx, 1 * soundVolume);
        musicSource.Stop();

        // wave text
        deathScreen.GetComponent<RectTransform>().localPosition = Vector3.zero;
        deathWaveText.GetComponent<TextMeshProUGUI>().text = "Wave " + GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().wave.ToString();
        deathWaveHighscore.GetComponent<TextMeshProUGUI>().text = "Highscore: " + waveHighscore;
        
        // timer text
        TimeSpan time = TimeSpan.FromSeconds(Mathf.RoundToInt(timePlayed));
        deathTimeText.GetComponent<TextMeshProUGUI>().text = "Time: " + string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        // kills text
        deathKillsText.GetComponent<TextMeshProUGUI>().text = currentKills.ToString() + " Kills";
        Time.timeScale = 0;
    }

    public void EnableCreditScreen()
    {
        pauseMenu.GetComponent<RectTransform>().localPosition = new(0, 2000, 0);
        creditScreenEnabled = true;
        creditScreen.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    public void DisableCreditScreen()
    {
        pauseMenu.GetComponent<RectTransform>().localPosition = Vector3.zero;
        creditScreen.GetComponent<RectTransform>().localPosition = new(0, 2000, 0);
        creditScreenEnabled = false;
    }

    public void ChangeDifficulty(TMP_Dropdown dropdown)
    {
        float value = dropdown.value;
        difficulty = value switch
        {
            0 => difficultyValues[0],
            1 => difficultyValues[1],
            2 => difficultyValues[2],
            3 => difficultyValues[3],
            4 => difficultyValues[4],
            _ => 1,
        };

        GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().difficultyMultiplier = difficulty;
    }
}
