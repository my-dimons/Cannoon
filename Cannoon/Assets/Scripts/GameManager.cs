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

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicTracks;
    [SerializeField] private AudioClip musicCurrentlyPlaying;
    public float musicTransitionTime;

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

    public bool pauseMenuEnabled;
    public bool creditScreenEnabled;

    [Header("Difficulty")]
    public Difficulty difficulty;

    public static GameManager Instance;
    // Numbers are the difficulty multiplier (DIVIDE BY 100, think of it as a percentage value)
    public enum Difficulty
    {
        easy = 75,
        normal = 100, // USUAL PLAYTHROUGH
        hard = 120,
        impossible = 200
    }

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
    }
    private void Awake()
    {
        // pause menu vars
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
        currentKills = 0;
        timePlayed = 0;

        Time.timeScale = 1;
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathQuitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathRespawnButton.GetComponent<Button>().onClick.AddListener(Respawn);
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
        creditsButton.GetComponent<Button>().onClick.AddListener(EnableCreditScreen);
        disableCreditsButton.GetComponent<Button>().onClick.AddListener(DisableCreditScreen);

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

    private void Start()
    {
        StartCoroutine(PlayMusicTrack());
    }

    IEnumerator PlayMusicTrack()
    {
        int track = UnityEngine.Random.Range(0, musicTracks.Length);
        musicSource.PlayOneShot(musicTracks[track]);
        musicCurrentlyPlaying = musicTracks[track];
        yield return new WaitForSeconds(musicTracks[track].length);
        musicCurrentlyPlaying = null;
        yield return new WaitForSeconds(musicTransitionTime);
        StartCoroutine(PlayMusicTrack());
    }

    public void PauseGame()
    {
        pauseMenu.GetComponent<RectTransform>().localPosition = Vector3.zero;
        Time.timeScale = 0;
        pauseMenuEnabled = true;
    }

    public void ResumeGame()
    {
        pauseMenu.GetComponent<RectTransform>().localPosition = new(0, 2000, 0);
        Time.timeScale = 1;
        pauseMenuEnabled = false;
    }

    public void EnableDeathScreen()
    {
        deathScreen.GetComponent<RectTransform>().localPosition = Vector3.zero;
        deathWaveText.GetComponent<TextMeshProUGUI>().text = "Wave " + GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().wave.ToString();
        
        TimeSpan time = TimeSpan.FromSeconds(timePlayed);
        deathTimeText.GetComponent<TextMeshProUGUI>().text = "Time: " + time.Minutes.ToString() + ":" + time.Seconds.ToString();
        deathKillsText.GetComponent<TextMeshProUGUI>().text = currentKills.ToString() + " Kills";
        UnityEngine.Time.timeScale = 0;
    }

    public void EnableCreditScreen()
    {
        Debug.Log("enableing credits");
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
}
