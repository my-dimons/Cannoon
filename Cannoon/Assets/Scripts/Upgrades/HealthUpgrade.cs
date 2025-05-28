using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    public int health;
    public int regen;
    PlayerHealth playerHealthScript;
    EndlessMode endlessModeScript;
    Upgrade upgradeScript;

    private void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        endlessModeScript = GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>();
        upgradeScript = GetComponent<Upgrade>();
    }
    public void UpdateStats()
    {
        playerHealthScript.numOfHearts += health;
        endlessModeScript.healthRegen += regen;
        upgradeScript.Pick();
    }
}