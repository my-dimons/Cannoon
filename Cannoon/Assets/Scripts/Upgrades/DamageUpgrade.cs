using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpgrade : MonoBehaviour
{
    // IN PERCENTS
    public float damageIncrease;
    public float chargeSpeed;
    public float criticalChanceIncrease;

    Cannon cannonScript;
    Upgrade upgradeScript;

    private void Start()
    {
        cannonScript = GameObject.FindGameObjectWithTag("Cannon").GetComponent<Cannon>();
        upgradeScript = GetComponent<Upgrade>();
    }
    public void ChangeStats()
    {
        cannonScript.criticalStrikeChance += cannonScript.criticalStrikeChance / 100 * criticalChanceIncrease;
        cannonScript.maxBulletDamage += cannonScript.maxBulletDamage / 100 * damageIncrease;
        cannonScript.maxCharge += cannonScript.maxCharge / 100 * chargeSpeed;
        upgradeScript.Pick();
    }
}