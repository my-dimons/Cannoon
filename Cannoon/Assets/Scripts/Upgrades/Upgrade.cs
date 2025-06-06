using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public bool specialUpgrade;
    public GameObject particles;
    public Color particleColor;

    GameObject description;
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

    public void Pick(bool reRoll, bool specialReRoll)
    {
        PlayParticles();

        upgradeManagerScript.FinishPickingUpgrades(reRoll, specialReRoll);
    }

    void PlayParticles()
    {
        GameObject particle = Instantiate(particles, transform.parent);

        // change color
        ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
        main.startColor = particleColor;

        particle.GetComponent<RectTransform>().localPosition = this.GetComponent<RectTransform>().localPosition;
    }
}
