using System.Collections;
using UnityEngine;

public class PlayerJumpDetection : MonoBehaviour
{
    [Tooltip("Put the player game object that needs jump detection here")]
    public GameObject player;
    [Tooltip("Slight delay of being able to jump once the player hits the ground")]
    PlayerMovement playerScript;

    public GameObject groundPoundParticles;
    public Vector3 groundPoundParticlesSpawningPos;

    // when the player hits the ground, make them move slower for a set time
    IEnumerator SlowPlayerOnLanding()
    {
        playerScript.speed /= playerScript.jumpLandingSpeedDivisor;
        yield return new WaitForSeconds(playerScript.jumpLandingSpeedTime);
        playerScript.speed = playerScript.baseSpeed;
    }
    IEnumerator CoyoteJumpTimer()
    {
        playerScript.fallingGravityOverride = true;
        player.GetComponent<Rigidbody2D>().gravityScale = playerScript.gravityFallMultiplier * playerScript.edgeCoastingGravity;
        yield return new WaitForSeconds(playerScript.coyoteJumpTime);
        playerScript.fallingGravityOverride = false;
        playerScript.canJump = false;
    }
    private void Start()
    {
        playerScript = player.GetComponent<PlayerMovement>();
    }
    // player hits ground
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // player falls on the ground
            if (!playerScript.onGround)
            {
                StartCoroutine(SlowPlayerOnLanding());

                playerScript.playerAudio.pitch = Random.Range(0.75f, 1.25f);
                playerScript.playerAudio.PlayOneShot(playerScript.hittingGround, 0.7f * GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume);
                StartCoroutine(Camera.main.GetComponent<CameraScript>().Screenshake(0.2f));
                // particles
                GameObject groundPoundParticle = Instantiate(groundPoundParticles, transform.position - groundPoundParticlesSpawningPos, groundPoundParticles.transform.rotation);
                groundPoundParticle.GetComponent<ParticleSystem>().Play();
            }
            // reset players jump when hitting the ground
            if (!playerScript.canJump)
            { 
                playerScript.canJump = true;
                playerScript.canDoubleJump = true;
            }

            playerScript.onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (playerScript.onGround && playerScript.canJump)
            {
                StartCoroutine(CoyoteJumpTimer());
            }

            playerScript.onGround = false;
        }
    }
}
