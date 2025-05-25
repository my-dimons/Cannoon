using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    EndlessMode endlessMode;
    [Tooltip("How many waves for each upgrade")]
    public int upgradeWaves;

    [Header("Upgrade Bars")]
    public Image[] upgradeBars;
    public int upgradeTicks;
    // Start is called before the first frame update
    void Start()
    {
        endlessMode = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUpgradeBars();
    }

    void UpdateUpgradeBars()
    {
        if (upgradeTicks == upgradeWaves)
        {
            StartCoroutine(ResetUpgradeBars());
        }
        else
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
    }

    IEnumerator ResetUpgradeBars()
    {
        upgradeBars[upgradeWaves - 1].gameObject.GetComponent<Animator>().SetBool("isFilled", true);
        yield return new WaitForSeconds(2);
        upgradeTicks = 0;
    }

    void Upgrades()
    {

    }
}
