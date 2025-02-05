using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject bullet;
    public Vector2 bulletSpawnPos;
    public float bulletSpeed;
    public float bulletCooldown;
    public bool canShoot;
    public float bulletDamage;

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
}
