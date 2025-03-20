using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDetection : MonoBehaviour
{
    [Tooltip("Put the player game object that needs jump detection here")]
    public GameObject player;
    Player playerScript;

    private void Start()
    {
        playerScript = player.GetComponent<Player>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerScript.jumpsRemaining != playerScript.jumps)
        {
            Debug.Log("Reseting Jumps");
            playerScript.jumpsRemaining = playerScript.jumps;
        }
    }
}
