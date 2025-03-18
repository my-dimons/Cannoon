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

    [Tooltip("The base max health")]
    public int baseHealth;
    [Tooltip("The current max health")]
    public int maxHealth;

    public float currentHealth;

    public bool canTakeDamage;
    public float damageInvincibilityCooldown;

    public int jumps;
    public int jumpsRemaining;
    public float jumpCheckDistance;
    public LayerMask layerMask;

    [Header("Health Bar")]
    public TextMeshProUGUI healthText;
    public Image healthBarImage;
    public GameObject healthBar;

    [Header("Info")]
    public string currentLevel;
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

    [Header("Other")]
    GameManager gameManager;

    IEnumerator DamageInvincibilityTimer()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageInvincibilityCooldown);
        canTakeDamage = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        maxHealth = baseHealth;
        currentHealth = maxHealth;

        canTakeDamage = true;
        cannonFacingRight = true;
        jumpsRemaining = jumps;

        cannonScript = cannon.GetComponent<Cannon>();
        rb = this.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        // HEALTH (TEMPERARY)
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Heal(100f);

        CheckJump();
        Movement();

        if (gameObject.transform.position.y <= -5 && !dead)
        {
            Death();
        }
        if (currentHealth <= 0 && !dead)
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
        healthBar.SetActive(false);
        StartCoroutine(respawn());
        gameManager.globalDeaths += 1;

        IEnumerator respawn()
        {
            for (int i = 0; i < respawnTime; i++) 
            {
                int time = respawnTime - i;
                respawnText.text = "RESPAWNING IN " + time.ToString() + " SECONDS";
                yield return new WaitForSeconds(1);
            }
            gameManager.LoadLevel(currentLevel);
        }

    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            Debug.Log("Taking Damage");
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, 100);

            healthBarImage.fillAmount = currentHealth / 100;
            healthText.text = Mathf.RoundToInt(currentHealth).ToString();

            StartCoroutine(DamageInvincibilityTimer());
        }
    }
    public void Heal(float healingAmount)
    {
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);

        healthBarImage.fillAmount = currentHealth / 100;
        healthText.text = Mathf.RoundToInt(currentHealth).ToString();
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