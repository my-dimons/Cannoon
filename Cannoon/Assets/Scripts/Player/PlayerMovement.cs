using System.Collections;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public GameObject playerSprite;
    public GameObject cannon;
    Animator playerAnimator;
    [Header("Speed")]
    public float baseSpeed;
    public float speed;
    public float airSpeedDivisor;

    [Header("Jumping")]
    public float baseJumpForce;
    public float jumpForce;
    public float jumpForceLimit;
    public bool canJump;
    public bool doubleJump;
    [HideInInspector] public bool canDoubleJump;
    public GameObject doubleJumpParticles;
    [Tooltip("in seconds")]
    public float coyoteJumpTime;

    [Header("Landing")]
    public float jumpLandingSpeedDivisor;
    public float jumpLandingSpeedTime;

    [Header("Ground Pound")]
    public bool groundPoundEnabled;
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
    public bool onGround;

    private bool facingRight;

    [Header("Cannon Bobbing")]
    public float bobAmount;
    public float bobTime;

    [Header("Audio")]
    public AudioSource playerAudio;
    public AudioClip hittingGround;
    public AudioClip groundPoundSound;
    public AudioClip[] runningSounds;
    public AudioClip doubleJumpSfx;


    [Header("Other")]
    PlayerHealth playerHealthScript;
    Cannon cannonScript;
    GameManager gameManager;
    Rigidbody2D rb;

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
        playerAnimator = playerSprite.GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        cannonScript = cannon.transform.Find("Cannon").GetComponent<Cannon>();
        speed = baseSpeed;
        jumpForce = baseJumpForce;

        StartCoroutine(CannonMovement());
    }
    // Update is called once per frame
    void Update()
    {
        // Falling gravity multiplier
        if (jumpForce > jumpForceLimit) 
            jumpForce = jumpForceLimit;

        GravityMultiplier();
        if (!playerHealthScript.dead)
        {
            if (!cannonScript.charging)
                InAir();
            Movement();

            // Ground Pound
            if (Input.GetKeyDown(KeyCode.LeftControl) && !onGround && groundPoundEnabled)
                GroundPound();

            // Jumping
            // double jump
            if (Input.GetKeyDown(KeyCode.Space) && doubleJump && canDoubleJump && !canJump)
                Jump(true);
            // normal jump
            else if (Input.GetKeyDown(KeyCode.Space) && canJump)
                Jump(false);
        }

        FacingDirection();

        // Update animation params
        playerAnimator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
    }

    IEnumerator CannonMovement()
    {
        if (rb.velocity.x != 0)
        {
            yield return new WaitForSeconds(bobTime);
            cannon.transform.position = new Vector3(cannon.transform.position.x, cannon.transform.position.y + bobAmount, cannon.transform.position.z);
            yield return new WaitForSeconds(bobTime);
            cannon.transform.position = new Vector3(cannon.transform.position.x, cannon.transform.position.y - bobAmount, cannon.transform.position.z);
        }
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(CannonMovement());
    }
    // Left right movement
    private void Movement()
    {
        //left & right movement
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        if (rb.velocity.x != 0 && onGround && !playerAudio.isPlaying)
        {
            int i = Random.Range(0, runningSounds.Length);
            playerAudio.pitch = Random.Range(0.75f, 1.25f);
            playerAudio.PlayOneShot(runningSounds[i], 1.75f * gameManager.soundVolume);
        }
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

    private void Jump(bool doubleJump)
    {
        canJump = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;

        if (doubleJump)
        {
            canDoubleJump = false;
            playerAudio.PlayOneShot(doubleJumpSfx, 1f *  gameManager.soundVolume);
            Instantiate(doubleJumpParticles, this.gameObject.transform.Find("Collisions").Find("Jump Detection").transform.position, Quaternion.identity);
        }

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

        playerAudio.pitch = Random.Range(0.75f, 1.25f);
        playerAudio.PlayOneShot(groundPoundSound, 2f * gameManager.soundVolume);
        playerAudio.pitch = 1f;

        // apply ground pound jump boost
        if (canApplyGroundPoundJumpBoost)
        {
            jumpForce = baseJumpForce + (baseJumpForce * groundPoundJumpPercentage / 100);
            canApplyGroundPoundJumpBoost = false;
            StartCoroutine(GroundPoundJumpBoostTimer());
        }
    }

    private void FacingDirection()
    {
        if (rb.velocity.x > 0)
            facingRight = true;
        else if (rb.velocity.x < 0)
            facingRight = false;

        if (facingRight)
        {
            playerSprite.transform.localScale = new Vector3(-3.25f, 3.25f, 1f);
        } else
        {
            playerSprite.transform.localScale = new Vector3(3.25f, 3.25f, 1f);
        }
    }
}