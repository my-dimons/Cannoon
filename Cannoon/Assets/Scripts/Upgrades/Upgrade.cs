using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public GameObject particles;
    GameObject description;
    UpgradeManager upgradeManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        upgradeManagerScript = GameObject.FindGameObjectWithTag("UpgradeManager").GetComponent<UpgradeManager>();
        description = transform.Find("description").gameObject;

        PlayParticles();
    }

    private void OnMouseOver()
    {
        description.SetActive(true);
    }

    public void Pick()
    {
        particles.GetComponent<ParticleSystem>().Play();
        PlayParticles();

        upgradeManagerScript.FinishPickingUpgrades();
    }

    void PlayParticles()
    {
        Instantiate(particles, transform.parent);
    }
}
