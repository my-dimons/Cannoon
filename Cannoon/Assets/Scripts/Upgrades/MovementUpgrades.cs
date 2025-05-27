using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUpgrade : MonoBehaviour
{
    public float jumpHeightIncrease;
    public float speedIncrease;
    PlayerMovement playerMovementScript;
    Upgrade upgradeScript;

    private void Start()
    {
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        upgradeScript = GetComponent<Upgrade>();
    }

    public void ChangeStats()
    {
        playerMovementScript.baseJumpForce += playerMovementScript.baseJumpForce / 100 * jumpHeightIncrease;
        playerMovementScript.baseSpeed += playerMovementScript.baseSpeed / 100 * speedIncrease;
        upgradeScript.Pick();
    }
}