using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Needed Stats")]
    public Vector2 bulletSpawnPos;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletCooldown;
    public float bulletDamage;

    //[Header("Special Stats")]
    //public bool canPierce;
    //public int pierceAmount;
    //public bool canBounce;
    //public int bounceAmount;

    [Header("OTHER")]
    public bool canShoot;
    public Sprite itemSlotSprite;
    public IEnumerator bulletShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(bulletCooldown);
        canShoot = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShootBullet()
    {
        Vector2 spawnPos;
        spawnPos = new Vector2(transform.position.x - bulletSpawnPos.x, transform.position.y + bulletSpawnPos.y);

        GameObject prefab = Instantiate(bullet, spawnPos, new Quaternion(0, 0, 0, 0));
        prefab.GetComponent<Bullet>().setStats(bulletSpeed, this.gameObject, true, bulletDamage);

        // bullet rotates towards cursor
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseScreenPosition - spawnPos).normalized;
        prefab.transform.right = direction;
        StartCoroutine(bulletShootingCooldown());
    }
}
