using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float jump;
    public int health;

    public int jumps;
    public int jumpsRemaining;
    public float jumpCheckDistance;
    public LayerMask layerMask;
    bool facingLeft;

    [Header("Weapons")]
    public GameObject bullet;
    public GameObject heldWeapon;
    public Vector2 bulletSpawnPos;
    public float bulletSpeed;
    public float bulletCooldown;
    private bool canShoot;
    public float bulletDamage;

    IEnumerator bulletShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletCooldown);
        canShoot = true;
    }

    [Header("Death")]
    public Vector2 spawnPoint;
    public TextMeshProUGUI respawnText;
    public GameObject deathScreen;
    public int respawnTime;
    private bool dead;

    [Header("Item Slots")]
    public GameObject[] itemSlots;
    public GameObject selectedItemSlot;


    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        jumpsRemaining = jumps;
        canShoot = true;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !itemSlots[0].GetComponent<ItemSlot>().selected)
        {
            selectedItemSlot.GetComponent<ItemSlot>().SelectItemSlot(itemSlots[0].GetComponent<ItemSlot>());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !itemSlots[1].GetComponent<ItemSlot>().selected)
        {
            selectedItemSlot.GetComponent<ItemSlot>().SelectItemSlot(itemSlots[1].GetComponent<ItemSlot>());
        }
        Movement();

        if (Input.GetMouseButton(0) && canShoot)
        {
            ShootBullet();
        }
        if (gameObject.transform.position.y <= -5 && !dead)
        {
            Death();
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

        //flip sprite based on movement direction
        // facing right
        if (horizontalInput < 0 && transform.localScale.x > 0 && !facingLeft)
        {
            ChangePlayerFacingDirection(true);
        }
        // facing left
        else if (horizontalInput > 0 && transform.localScale.x < 0 && facingLeft)
        {
            ChangePlayerFacingDirection(false);
        }


        if (Input.GetKeyDown(KeyCode.Space) && CanPlayerJump())
        {
            Jump();
        }
    }
    void ChangePlayerFacingDirection(bool isFacingRight)
    {
        facingLeft = isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    bool CanPlayerJump()
    {
        if (jumpsRemaining <= jumps && jumpsRemaining != 0 )
        {
            jumpsRemaining -= 1;
            return true;
        }
        return false;
    }
    void Jump()
    {
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(new Vector2(rb.velocity.x, jump));
    }
    void ShootBullet()
    {
        Vector2 spawnPos;
        if (facingLeft)
            spawnPos = new Vector2(transform.position.x - bulletSpawnPos.x, transform.position.y + bulletSpawnPos.y);
        else
            spawnPos = new Vector2(transform.position.x + bulletSpawnPos.x, transform.position.y + bulletSpawnPos.y);

        GameObject prefab = Instantiate(bullet, spawnPos, new Quaternion(0, 0, 0, 0));
        prefab.GetComponent<Bullet>().setStats(bulletSpeed, this.gameObject, true, bulletDamage);

        // bullet rotates towards cursor
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - spawnPos).normalized;
        prefab.transform.right = direction;
        StartCoroutine(bulletShootingCooldown());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpsRemaining = jumps;
        }
    }
}