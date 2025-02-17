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
    public float jumpCheckDistance;
    public LayerMask layerMask;

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
    // Start is called before the first frame update
    void Start()
    {
        cannonFacingRight = true;
        cannonScript = cannon.gameObject.GetComponent<Cannon>();
        jumpsRemaining = jumps;
        rb = this.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        //FLIP CANNON SPRITE
        if (cannonRotationObj.transform.eulerAngles.z > 90 && cannonRotationObj.transform.eulerAngles.z < 270 && cannonFacingRight)
        {
            cannon.GetComponent<SpriteRenderer>().flipY = true;
            cannonFacingRight = false;
            Debug.Log("FLIIPING SPRITE");
        }
        else if (cannonRotationObj.transform.eulerAngles.z < 90 && !cannonFacingRight || cannonRotationObj.transform.eulerAngles.z > 270 && !cannonFacingRight)
        {
            cannon.GetComponent<SpriteRenderer>().flipY = false;
            cannonFacingRight = true;
            Debug.Log("UNFLIIPING SPRITE");
        }


        //SHOOT BULLET
        if (Input.GetMouseButton(0) && cannonScript.canShoot)
        {
            cannonScript.ShootBullet();
        }

        //CANNON ROTATE TOWARDS MOUSE
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        cannonRotationObj.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        CheckJump();
        Movement();

        if (gameObject.transform.position.y <= -5 && !dead)
        {
            Death();
        }
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
}