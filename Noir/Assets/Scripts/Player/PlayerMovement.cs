using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public float baseSpeed;
    public float speed;
    public float jumpForce;

    [Tooltip("When the player is falling this variable is used on the players y velocity")]
    public float gravityFallMultiplier;
    [Tooltip("in seconds")]
    public float cyoteJump;
    [Tooltip("how fast the player needs to be falling for the players gravity to increase (can be applied before the player starts falling)")]
    public float fallingAffecterNumber;
    [Tooltip("how much slower the player is after landing on the ground (divides speed by this variable)")]
    public float jumpLandingSpeedDivisor;
    [Tooltip("the small pause time when the player lands on the ground (in seconds)")]
    public float jumpLandingSpeedTime;
    public bool canJump;

    public LayerMask layerMask;

    [Header("Info")]
    public string currentLevel;

    Rigidbody2D rb;

    [Header("Other")]
    PlayerHealth playerHealthScript;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        playerHealthScript = GetComponent<PlayerHealth>();
        speed = baseSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        // Falling gravity multiplier
        GravityMultiplier();
        if (!playerHealthScript.dead)
        {
            Movement();

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
        if (this.rb.velocity.y < fallingAffecterNumber && !canJump)
            rb.gravityScale = gravityFallMultiplier;
        else if (this.rb.velocity.y < 0 && !canJump)
            rb.gravityScale = gravityFallMultiplier;
        else
            rb.gravityScale = 1;
    }

    private void Jump()
    {
        canJump = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
    }
}