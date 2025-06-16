using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    public GameObject arrow;
    public AnimationClip attackAnim;
    public AudioClip shootingSound;
    public Vector2 arrowSpawnPos;
    public float cooldown;
    public float attackTime;
    public bool canAttack;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cooldown());
    }

    // Update is called once per frame
    void Update()
    {
        if (canAttack)
        {
            StartCoroutine(ShootArrow());
            StartCoroutine(GetComponent<Enemy>().FreezeEnemy(attackAnim.length));
        }
    }

    IEnumerator Cooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
    IEnumerator ShootArrow()
    {
        StartCoroutine(Cooldown());
        GetComponent<Enemy>().animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackTime);

        GetComponent<AudioSource>().PlayOneShot(shootingSound, GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume * 0.7f);
        GetComponent<Enemy>().animator.SetBool("isAttacking", false);
        if (GetComponent<Enemy>().facingRight)
        {
            GameObject spawnedArrow = Instantiate(arrow, new(transform.position.x - arrowSpawnPos.x, transform.position.y + arrowSpawnPos.y, transform.position.z), transform.rotation);
            spawnedArrow.GetComponent<Bullet>().SetStats(30, 1, 25);
            spawnedArrow.GetComponent<Bullet>().sprite.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GameObject spawnedArrow = Instantiate(arrow, new(transform.position.x + arrowSpawnPos.x, transform.position.y + arrowSpawnPos.y, transform.position.z), transform.rotation);
            spawnedArrow.GetComponent<Bullet>().SetStats(-30, 1, 25);
        }

    }
}
