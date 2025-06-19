using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    public bool cheated;

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
    public AudioClip[] bossTracks;
    public bool playingBossTracks;
    public AudioClip musicCurrentlyPlaying;
    public float musicTransitionTime;
    public bool playingMusic;
    public bool canPlayMusic;

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
    public static GameObject difficultyModeText;
    public static GameObject cheatedText;
    public static GameObject copyButton;

    public bool pauseMenuEnabled;
    public bool creditScreenEnabled;

    [Header("Difficulty")]
    public float difficulty;
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

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && !creditScreenEnabled)
        {
            if (!pauseMenuEnabled)
                PauseGame();
            else
                ResumeGame();
        }

        if (!playingMusic && canPlayMusic)
        {
            if (!playingBossTracks)
                StartCoroutine(PlayMusicTrack());
            else
                StartCoroutine(PlayBossMusicTrack());
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
        difficultyModeText = deathScreen.transform.Find("Difficulty").gameObject;
        cheatedText = deathScreen.transform.Find("Cheated").gameObject;
        copyButton = deathScreen.transform.Find("Copy Button").gameObject;

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
        canPlayMusic = true;
        StartCoroutine(PlayMusicTrack());
        cheated = false;
        currentKills = 0;
        timePlayed = 0;

        Time.timeScale = 1;
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathQuitButton.GetComponent<Button>().onClick.AddListener(Quit);
        deathRespawnButton.GetComponent<Button>().onClick.AddListener(Respawn);
        copyButton.GetComponent<Button>().onClick.AddListener(CopyScore);
        resumeButton.GetComponent<Button>().onClick.AddListener(ResumeGame);
        creditsButton.GetComponent<Button>().onClick.AddListener(EnableCreditScreen);
        disableCreditsButton.GetComponent<Button>().onClick.AddListener(DisableCreditScreen);

        // difficulty dropdown
        GameObject.Find("Difficulty Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { ChangeDifficulty(GameObject.Find("Difficulty Dropdown").GetComponent<TMP_Dropdown>()); });
        // crosshair dropdown
        GameObject.Find("Crosshair Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { ChangeCrosshair(GameObject.Find("Crosshair Dropdown").GetComponent<TMP_Dropdown>()); });

        ChangeDifficulty(GameObject.Find("Difficulty Dropdown").GetComponent<TMP_Dropdown>());

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

    public IEnumerator PlayMusicTrack()
    {
        Debug.Log("Playing background music");
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

    public IEnumerator PlayBossMusicTrack()
    {
        Debug.Log("Playing boss music");
        musicSource.Stop();
        playingMusic = true;
        int track = UnityEngine.Random.Range(0, bossTracks.Length);
        musicSource.PlayOneShot(bossTracks[track]);
        musicCurrentlyPlaying = bossTracks[track];

        yield return new WaitForSeconds(bossTracks[track].length);

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

        if (!cheated)
            cheatedText.SetActive(false);

        TMP_Dropdown modeText = GameObject.Find("Difficulty Dropdown").GetComponent<TMP_Dropdown>();

        string original = modeText.options[modeText.value].text;
        string cleaned = Regex.Replace(original, @"  +", " ");  // replace 2+ spaces with 1 space

        difficultyModeText.GetComponent<TextMeshProUGUI>().text = "Difficulty: " + cleaned;

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
            5 => difficultyValues[5],
            _ => 1,
        };

        GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().difficultyMultiplier = difficulty;
    }
    void ChangeCrosshair(TMP_Dropdown dropdown)
    {
        float value = dropdown.value;
        CameraScript cameraScript = Camera.main.GetComponent<CameraScript>();
        Sprite newCrosshair = value switch
        {
            0 => cameraScript.crosshairs[0],
            1 => cameraScript.crosshairs[1],
            2 => cameraScript.crosshairs[2],
            3 => cameraScript.crosshairs[3],
            4 => cameraScript.crosshairs[4],
            5 => cameraScript.crosshairs[5],
            _ => cameraScript.crosshairs[0],
        };

        cameraScript.crosshair.GetComponent<Image>().sprite = newCrosshair;
    }

    void CopyScore()
    {
        // difficulty text formatting
        TMP_Dropdown modeText = GameObject.Find("Difficulty Dropdown").GetComponent<TMP_Dropdown>();
        string original = modeText.options[modeText.value].text;
        string noTags = Regex.Replace(original, @"<.*?>", "");
        string cleaned = Regex.Replace(noTags, @" {2,}", " ");  // replace 2+ spaces with 1 space

        EndlessMode endlessMode = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();

        // time text
        TimeSpan time = TimeSpan.FromSeconds(Mathf.RoundToInt(timePlayed));
        string timeText = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        string cheat = cheated ? " | 🚨 CHEATER CHEATER CANNON EATER 🚨" : "";

        // color squares
        string[] colorSquares = { "🟦", "🟩", "❎", "🟨", "🟧", "🟥" };
        string colorSquare = "";
        for (int i = 0; i < difficultyValues.Length; i++)
        {
            if (difficulty == difficultyValues[i])
                colorSquare = colorSquares[i];
        }

            string text = "🌊 Wave: " + endlessMode.wave + " | 🕛 Time: " + timeText + " | 💀 Kills: " + currentKills + " | ⭐ Difficulty: " + cleaned + " " + colorSquare + cheat;
        GUIUtility.systemCopyBuffer = text;
    }
}
