using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    [Tooltip("Put the player game object that needs jump detection here")]
    public GameObject player;
    [Tooltip("Slight delay of being able to jump once the player hits the ground")]
    PlayerMovement playerScript;

    // when the player hits the ground, make them move slower for a set time
    IEnumerator PlayerLanding()
    {
        playerScript.speed /= playerScript.jumpLandingSpeedDivisor;
        yield return new WaitForSeconds(playerScript.jumpLandingSpeedTime);
        playerScript.speed = playerScript.baseSpeed;
    }
    private void Start()
    {
        playerScript = player.GetComponent<PlayerMovement>();
    }
    // player hits ground
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!playerScript.canJump)
        {
            playerScript.canJump = true;
            StartCoroutine(PlayerLanding());
        }
    }
}
