using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    public int health;
    PlayerHealth playerHealthScript;
    Upgrade upgradeScript;

    private void Start()
    {
        playerHealthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        upgradeScript = GetComponent<Upgrade>();
    }
    public void Pick()
    {
        playerHealthScript.numOfHearts += health;
        upgradeScript.Pick();
    }
}