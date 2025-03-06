using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    [Header("Needed Stats")]
    public GameObject player;
    public GameObject bullet;
    public GameObject bulletSpawnObj;

    public float bulletSpeed;
    public float bulletCooldown;
    public float bulletDamage;
    public float bulletLifetime;

    //[Header("Special Stats")]
    //public bool canPierce;
    //public int pierceAmount;
    //public bool canBounce;
    //public int bounceAmount;

    [Header("OTHER")]
    public bool canShoot;
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

    public void ShootBullet()
    {
        Debug.Log("Shot bullet");
        Vector2 spawnPos;
        spawnPos = new Vector2(bulletSpawnObj.transform.position.x, bulletSpawnObj.transform.position.y);

        GameObject prefab = Instantiate(bullet, spawnPos, player.GetComponent<Player>().cannon.transform.rotation);

        prefab.GetComponent<Bullet>().setStats(bulletSpeed, bulletDamage, bulletLifetime, true);
        StartCoroutine(bulletShootingCooldown());
    }
}
