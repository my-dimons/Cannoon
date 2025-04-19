using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [Tooltip("The players base speed stat")]
    public float baseSpeed;
    [Tooltip("the players current speed")]
    public float speed;
    [Tooltip("when the player is in the air divide speed by this variable to slow the player (not in contact with any ground")]
    public float airSpeedDivisor;

    [Header("Jumping")]
    [Tooltip("the base stat number of how high the player can jump")]
    public float baseJumpForce;
    [Tooltip("players current jump force")]
    public float jumpForce;
    [Tooltip("can the player jump")]
    public bool canJump;
    [Tooltip("in seconds")]
    public float coyoteJumpTime;

    [Header("Landing")]
    [Tooltip("how much slower the player is after landing on the ground (divides speed by this variable)")]
    public float jumpLandingSpeedDivisor;
    [Tooltip("the small pause time when the player lands on the ground (in seconds)")]
    public float jumpLandingSpeedTime;

    [Header("Ground Pound")]
    public float groundPoundForce;
    [Tooltip("(%) when the player ground pounds, what percentage of the jump force is added to the jump")]
    public float groundPoundJumpPercentage;
    [Tooltip("(in seconds) how long the player has the groundPoundJumpPercentage var affect the players jump force after performing a jump")]
    public float groundPoundJumpBoostTimer;
    [Tooltip("can the groundPountJumpPercentage be added to the players jump force")]
    public bool canApplyGroundPoundJumpBoost;

    [Header("Falling")]
    [Tooltip("when the player is falling this variable is used on the players y velocity")]
    public float gravityFallMultiplier;
    [Tooltip("how fast the player needs to be falling for the players gravity to increase (can be applied before the player starts falling)")]
    public float fallingAffecterNumber;
    [Tooltip("(MULTIPLIER) When falling off an edge, for a certain amount of time, multiply the players gravity scale by this variable (calculated in the JumpDetections coyote jump)")]
    public float edgeCoastingGravity;
    [Tooltip("Lets the players gravity scale be overriden (disables GravityMultiplier() function)")]
    public bool fallingGravityOverride;

    [Header("Info")]
    [Header("is the player on the ground")]
    public bool onGround;

    Rigidbody2D rb;

    [Header("Other")]
    PlayerHealth playerHealthScript;

    IEnumerator GroundPoundJumpBoostTimer()
    {
        yield return new WaitForSeconds(groundPoundJumpBoostTimer);
        jumpForce = baseJumpForce;
        canApplyGroundPoundJumpBoost = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        playerHealthScript = GetComponent<PlayerHealth>();
        speed = baseSpeed;
        jumpForce = baseJumpForce;
    }
    // Update is called once per frame
    void Update()
    {
        // Falling gravity multiplier
        GravityMultiplier();
        if (!playerHealthScript.dead)
        {
            InAir();
            Movement();

            // Ground Pound
            if (Input.GetKeyDown(KeyCode.LeftControl) && !onGround)
                GroundPound();
            // Jumping
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
                Jump();
        }
    }

    // Left right movement
    private void Movement()
    {
        //left & right movement
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
    }

    // when FALLING increase the gravity by a set amount
    private void GravityMultiplier()
    {
        if (!fallingGravityOverride)
        {
            // after jumping
            if (this.rb.velocity.y < fallingAffecterNumber && !onGround)
                rb.gravityScale = gravityFallMultiplier;
            // falling of an edge
            else if (this.rb.velocity.y < 0 && !onGround)
                rb.gravityScale = gravityFallMultiplier;
            // normal gravity
            else
                rb.gravityScale = 1;
        }
    }

    private void Jump()
    {
        canJump = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
        // add upward force
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce), ForceMode2D.Impulse);
    }

    private void InAir()
    {
        if (!onGround)
        {
            // slows the player down in the air
            speed = baseSpeed / airSpeedDivisor;
        }
    }

    private void GroundPound()
    {
        // add downward force
        rb.AddForce(new Vector2(0, -groundPoundForce), ForceMode2D.Impulse);

        // apply ground pound jump boost
        if (canApplyGroundPoundJumpBoost)
        {
            jumpForce = baseJumpForce + (baseJumpForce * groundPoundJumpPercentage / 100);
            canApplyGroundPoundJumpBoost = false;
            StartCoroutine(GroundPoundJumpBoostTimer());
        }
    }
}