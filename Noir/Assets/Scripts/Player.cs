using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Sprite basePlayerSprite;

    [Header("Stats")]
    public float speed;
    public float jump;
    public int health;

    public int jumps;
    public int jumpsRemaining;
    public LayerMask layerMask;
    float jumpCheckDistance;

    [Header("Death")]
    public Vector2 spawnPoint;
    public TextMeshProUGUI respawnText;
    public GameObject deathScreen;
    private bool dead;
    public int respawnTime;

    [Header("Weapons")]
    public GameObject currentWeapon;
    public GameObject itemSlot;
    public GameObject itemSlotSprite;
    Gun heldItemScript;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        heldItemScript = currentWeapon.GetComponent<Gun>();
        UpdateItemSlotSprite();
        jumpsRemaining = jumps;
        rb = this.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        // change later to a function to be able to switch guns
        if (currentWeapon != null)
        {
            heldItemScript = currentWeapon.gameObject.GetComponent<Gun>();
        } else
        {
            heldItemScript = null;
        }

        //SHOOT BULLET
        if (Input.GetMouseButton(0) && heldItemScript.canShoot)
        {
            heldItemScript.ShootBullet();
        }

        Movement();

        if (gameObject.transform.position.y <= -5 && !dead)
        {
            Death();
        }
    }

    public void SwapItems()
    {

    }

    public void UpdateItemSlotSprite()
    {
        itemSlotSprite.gameObject.GetComponent<Image>().sprite = heldItemScript.itemSlotSprite;
    }

    bool HoldingWeapon()
    {
        if (currentWeapon == null)
            return false;
        else
            return true;
    }
    void Death()
    {
        dead = true;
        deathScreen.SetActive(true);
        StartCoroutine(respawn());

        IEnumerator respawn()
        {
            for (int i = 0; i < respawnTime; i++) 
            {
                int time = respawnTime - i;
                respawnText.text = "RESPAWNING IN " + time.ToString() + " SECONDS";
                yield return new WaitForSeconds(1);
            }
            SceneManager.LoadScene("SampleScene");
        }
    }
    void Movement()
    {
        //left & right movement
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && CanPlayerJump())
        {
            Jump();
        }

        bool CanPlayerJump()
        {
            if (jumpsRemaining <= jumps && jumpsRemaining != 0)
            {
                jumpsRemaining -= 1;
                return true;
            }
            return false;
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jump));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpsRemaining = jumps;
        }
    }
}