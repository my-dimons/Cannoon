using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUpgradeBars();
    }

    void UpdateUpgradeBars()
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

        List<GameObject> availableUpgradeOrbs = new List<GameObject>();
        for (int i = 0;i < upgradeOrbs.Length; i++)
            availableUpgradeOrbs.Add(upgradeOrbs[i]);

        yield return new WaitForSeconds(2);
        upgradeTicks = 0;

        List<GameObject> pickedUpgrades = new List<GameObject>();
        // pick 2 random upgrades
        for (int i = 0;i < 2; i++)
        {
            int num = Random.Range(0, upgradeOrbs.Length);
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
    }

    public void FinishPickingUpgrades()
    {
        pauseWaves = false;

        for (int i = 0; i < spawnedUpgradeOrbs.Count; i++)
            Destroy(spawnedUpgradeOrbs[i]);

        spawnedUpgradeOrbs.Clear();
    }
}
