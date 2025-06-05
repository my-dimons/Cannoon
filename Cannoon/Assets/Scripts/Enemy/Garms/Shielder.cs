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
        GetComponent<FollowEnemyAI>().animator.SetBool("lockState", true);
        yield return new WaitForSeconds(GetComponent<FollowEnemyAI>().spawningAnimation.length);
        GetComponent<FollowEnemyAI>().animator.SetBool("lockState", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (canShield)
        {
            StartCoroutine(Shield());
            StartCoroutine(GetComponent<FollowEnemyAI>().FreezeEnemy(shieldAnim.length + attackLength));
        }
    }

    IEnumerator Shield()
    {
        Enemy enemy = GetComponent<Enemy>();
        StartCoroutine(Cooldown());

        GetComponent<FollowEnemyAI>().animator.SetBool("isShielded", true);
        enemy.canTakeDamage = false;
        enemy.destroyBullet = true;

        yield return new WaitForSeconds(shieldAnim.length);
        GetComponent<FollowEnemyAI>().animator.SetBool("lockState", true);
        yield return new WaitForSeconds(attackLength);

        GetComponent<FollowEnemyAI>().animator.SetBool("isShielded", false);
        GetComponent<FollowEnemyAI>().animator.SetBool("lockState", false);

        enemy.canTakeDamage = true;
        enemy.destroyBullet = false;
    }
}
