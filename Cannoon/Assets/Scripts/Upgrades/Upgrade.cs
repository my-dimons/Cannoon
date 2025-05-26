using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject particles;
    public GameObject description;
    UpgradeManager upgradeManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        upgradeManagerScript = GameObject.FindGameObjectWithTag("UpgradeManager").GetComponent<UpgradeManager>();
        description = transform.Find("description").gameObject;

        PlayParticles();
    }

    public void EnteringHover()
    {
        description.SetActive(true);
    }

    public void ExitingHover()
    {
        description.SetActive(false);
    }

    public void Pick()
    {
        particles.GetComponent<ParticleSystem>().Play();
        PlayParticles();

        upgradeManagerScript.FinishPickingUpgrades();
    }

    void PlayParticles()
    {
        GameObject particle = Instantiate(particles, transform.parent);
        particle.GetComponent<RectTransform>().localPosition = this.GetComponent<RectTransform>().localPosition;
    }
}
