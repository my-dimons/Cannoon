using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public bool pauseWaves;
    [HideInInspector] public bool appliedUpgradeTick;

    [Tooltip("How many waves for each upgrade")]
    public int upgradeWaves;

    [Header("Upgrade Bars")]
    public Image[] upgradeBars;
    public int upgradeTicks;

    [Header("Upgrade Orbs")]
    public GameObject parentUpgradeOrb;
    public GameObject[] upgradeOrbs;
    public List<GameObject> spawnedUpgradeOrbs;

    [Header("Specific Upgrade Orbs")]
    public GameObject chargeUpgradeOrb;
    public GameObject healthUpgradeOrb;
    public GameObject regenUpgradeOrb;
    public GameObject criticalChanceOrb;

    [Header("Audio")]
    public AudioSource upgradeAudio;
    public AudioClip upgradesSpawningSound;
    public AudioClip selectionSound;
    public AudioClip hoverSound;

    [Header("Other")]
    Cannon cannonScript;
    GameObject player;
    EndlessMode endlessModeScript;
    [HideInInspector] public GameManager gameManager;

    private void Start()
    {
        cannonScript = GameObject.FindGameObjectWithTag("Cannon").GetComponent<Cannon>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        player = GameObject.FindGameObjectWithTag("Player");
        UpdateUpgradeBars();
    }

    public void UpdateUpgradeBars()
    {
        for (int i = 0; i < upgradeBars.Length; i++)
        {
            if (i < upgradeTicks)
                upgradeBars[i].gameObject.GetComponent<Animator>().SetBool("isFilled", true);
            else
                upgradeBars[i].gameObject.GetComponent<Animator>().SetBool("isFilled", false);

            if (i < upgradeWaves)
                upgradeBars[i].enabled = true;
            else
                upgradeBars[i].enabled = false;
        }
    }

    public IEnumerator SpawnUpgrades()
    {
        upgradeBars[upgradeWaves - 1].gameObject.GetComponent<Animator>().SetBool("isFilled", true);
        UpdateUpgradeBars();

        // base upgrade orbs
        List<GameObject> availableUpgradeOrbs = new();
        for (int i = 0;i < upgradeOrbs.Length; i++)
            availableUpgradeOrbs.Add(upgradeOrbs[i]);

        // more fine selection
        // max crit chance
        if (cannonScript.criticalStrikeChance >= 100)
            availableUpgradeOrbs.Remove(criticalChanceOrb);
        // max health
        if (player.GetComponent<PlayerHealth>().numOfHearts >= player.GetComponent<PlayerHealth>().hearts.Length)
            availableUpgradeOrbs.Remove(healthUpgradeOrb);
        // max regen
        if (endlessModeScript.healthRegen >= player.GetComponent<PlayerHealth>().hearts.Length - 1)
            availableUpgradeOrbs.Remove(regenUpgradeOrb);
        // charge time
        if (cannonScript.maxCharge <= 0.1f)
            availableUpgradeOrbs.Remove(chargeUpgradeOrb); 

        yield return new WaitForSeconds(1);

        upgradeAudio.PlayOneShot(upgradesSpawningSound, 1f * gameManager.soundVolume);

        upgradeTicks = 0;
        List<GameObject> pickedUpgrades = new();
        // pick 2 random upgrades
        for (int i = 0;i < 2; i++)
        {
            int num = Random.Range(0, availableUpgradeOrbs.Count);
            pickedUpgrades.Add(availableUpgradeOrbs[num]);
            availableUpgradeOrbs.Remove(availableUpgradeOrbs[num]);
        }

        // spawn upgrades
        GameObject spawnedObj1 = Instantiate(pickedUpgrades[0], parentUpgradeOrb.transform);
        GameObject spawnedObj2 = Instantiate(pickedUpgrades[1], parentUpgradeOrb.transform);
        spawnedObj1.GetComponent<RectTransform>().localPosition = new Vector3(300, 0, 0);
        spawnedObj2.GetComponent<RectTransform>().localPosition = new Vector3(-300, 0, 0);
        spawnedUpgradeOrbs.Add(spawnedObj1);
        spawnedUpgradeOrbs.Add(spawnedObj2);

        availableUpgradeOrbs.Clear();

        UpdateUpgradeBars();
    }

    public void FinishPickingUpgrades()
    {
        pauseWaves = false;
        upgradeAudio.PlayOneShot(selectionSound, 1f * gameManager.soundVolume);

        for (int i = 0; i < spawnedUpgradeOrbs.Count; i++)
            Destroy(spawnedUpgradeOrbs[i]);

        spawnedUpgradeOrbs.Clear();
    }
}
