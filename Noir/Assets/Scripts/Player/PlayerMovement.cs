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
    public float speed;
    public float jump;

    [Tooltip("Maximum amount of jumps")]
    public int jumps;
    [Tooltip("How many more times can the player currently jump")]
    public int jumpsRemaining;
    [Tooltip("When the player is falling this variable is used on the players y velocity")]
    public float gravityFallMultiplier;
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

        jumpsRemaining = jumps;
    }
    // Update is called once per frame
    void Update()
    {
        if (!playerHealthScript.dead)
        {
            Movement();

            // Jumping
            if (Input.GetKeyDown(KeyCode.Space) && CanPlayerJump())
                Jump();

            // Falling gravity multiplier
            GravityMultiplier();
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
        if (this.rb.velocity.normalized[1] < 0)
            rb.gravityScale = gravityFallMultiplier;
        else
            rb.gravityScale = 1;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jump));
    }
    private bool CanPlayerJump()
    {
        if (jumpsRemaining <= jumps && jumpsRemaining != 0)
        {
            jumpsRemaining -= 1;
            return true;
        }
        return false;
    }
}