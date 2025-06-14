using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : MonoBehaviour
{
    public float cooldown;
    public float attackLength;
    public bool canShield;
    public AnimationClip shieldAnim;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cooldown());
        StartCoroutine(SpawnAnim());
        StartCoroutine(Shield(GetComponent<Enemy>().spawningAnimation.length));
    }
    IEnumerator Cooldown()
    {
        canShield = false;
        yield return new WaitForSeconds(cooldown);
        canShield = true;
    }

    // fixes a small bug with the spawning anim (being interupted by any state transition)
    IEnumerator SpawnAnim()
    {
        GetComponent<Enemy>().animator.SetBool("lockState", true);
        yield return new WaitForSeconds(GetComponent<Enemy>().spawningAnimation.length);
        GetComponent<Enemy>().animator.SetBool("lockState", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (canShield)
        {
            StartCoroutine(Shield(0));
        }
    }

    IEnumerator Shield(float time)
    {
        yield return new WaitForSeconds(time);

        StartCoroutine(GetComponent<Enemy>().FreezeEnemy(shieldAnim.length + attackLength));

        Enemy enemy = GetComponent<Enemy>();
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        StartCoroutine(Cooldown());

        GetComponent<Enemy>().animator.SetBool("isShielded", true);
        enemy.canTakeDamage = false;
        enemy.destroyBullet = true;

        yield return new WaitForSeconds(shieldAnim.length);
        GetComponent<Enemy>().animator.SetBool("lockState", true);
        yield return new WaitForSeconds(attackLength);

        GetComponent<Enemy>().animator.SetBool("isShielded", false);
        GetComponent<Enemy>().animator.SetBool("lockState", false);

        enemy.canTakeDamage = true;
        enemy.destroyBullet = false;
    }
}
