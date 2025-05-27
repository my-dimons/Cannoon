using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        // set new base variables
        playerMovementScript.baseJumpForce += playerMovementScript.baseJumpForce / 100 * jumpHeightIncrease;
        playerMovementScript.baseSpeed += playerMovementScript.baseSpeed / 100 * speedIncrease;

        // apply new base variables
        playerMovementScript.speed = playerMovementScript.baseSpeed;
        playerMovementScript.jumpForce = playerMovementScript.baseJumpForce;

        upgradeScript.Pick();
    }
}