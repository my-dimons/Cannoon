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
    public int maxHealth = 100;
    float health;

    public bool canTakeDamage;
    public float damageInvincibilityCooldown;

    public int jumps;
    public int jumpsRemaining;
    public float jumpCheckDistance;
    public LayerMask layerMask;

    [Header("Health Bar")]
    public TextMeshProUGUI healthText;
    public Image healthBar;

    [Header("Info")]
    public int kills;

    [Header("Death")]
    public Vector2 spawnPoint;
    public TextMeshProUGUI respawnText;
    public GameObject deathScreen;
    private bool dead;
    public int respawnTime;

    [Header("Cannon")]
    public GameObject cannon;
    public GameObject cannonRotationObj;
    bool cannonFacingRight;
    Cannon cannonScript;

    Rigidbody2D rb;

    IEnumerator DamageInvincibilityTimer()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageInvincibilityCooldown);
        canTakeDamage = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        canTakeDamage = true;
        health = maxHealth;

        cannonFacingRight = true;
        cannonScript = cannon.gameObject.GetComponent<Cannon>();
        jumpsRemaining = jumps;
        rb = this.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        // HEALTH (TEMPERARY)
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TakeDamage(25f);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Heal(25f);

        CheckJump();
        Movement();

        if (gameObject.transform.position.y <= -5 && !dead)
        {
            Death();
        }
        if (health <= 0 && !dead)
        {
            Death();
        }

        RotateCannonTowardsMouse();
        FlipCannonSprite();


        //SHOOT BULLET
        if (Input.GetMouseButton(0) && cannonScript.canShoot)
        {
            cannonScript.ShootBullet();
        }
    }

    // based on mouse position/cannon rotation
    private void FlipCannonSprite()
    {
        if (cannonRotationObj.transform.eulerAngles.z > 90 
            && 
            cannonRotationObj.transform.eulerAngles.z < 270 
            && 
            cannonFacingRight)
        {
            cannon.GetComponent<SpriteRenderer>().flipY = true;
            cannonFacingRight = false;
            Debug.Log("FLIPING SPRITE");
        }

        else if (cannonRotationObj.transform.eulerAngles.z < 90 && !cannonFacingRight 
            || 
            cannonRotationObj.transform.eulerAngles.z > 270 
            && 
            !cannonFacingRight)
        {
            cannon.GetComponent<SpriteRenderer>().flipY = false;
            cannonFacingRight = true;
            Debug.Log("UNFLIPING SPRITE");
        }
    }

    private void RotateCannonTowardsMouse()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        cannonRotationObj.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    private void CheckJump()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, jumpCheckDistance, layerMask);
        if (hit && jumpsRemaining < jumps)
        {
            jumpsRemaining = jumps;
            Debug.DrawRay(transform.position, Vector2.down, Color.blue, jumpCheckDistance);
        }
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
            SceneManager.LoadScene("EndlessMode");
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            Debug.Log("Taking Damage");
            health -= damage;
            health = Mathf.Clamp(health, 0, 100);

            healthBar.fillAmount = health / 100;
            healthText.text = Mathf.RoundToInt(health).ToString();

            StartCoroutine(DamageInvincibilityTimer());
        }
    }
    public void Heal(float healingAmount)
    {
        health += healingAmount;
        health = Mathf.Clamp(health, 0, 100);

        healthBar.fillAmount = health / 100;
        healthText.text = Mathf.RoundToInt(health).ToString();
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
}