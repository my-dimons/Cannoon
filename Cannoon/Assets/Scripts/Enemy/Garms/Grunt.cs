using System.Collections;
using UnityEngine;

public class Grunt : MonoBehaviour
{
    Enemy enemyScript;
    FollowEnemyAI enemyAiScript;
    Animator animator;
    [Tooltip("Assign this variable the attack animation clip")]
    public AnimationClip attackAnimation;

    [Header("Attack")]
    public float attackCooldown;
    public bool canAttack;
    [Tooltip("How close the player needs to be for this enemy to use this attack")]
    public float playerDistance;

    [Header("Audio")]
    public AudioClip attackSound;

    IEnumerator Attack(float cooldown)
    {
        StartCoroutine(GetComponent<Enemy>().FreezeEnemy(attackAnimation.length));
        enemyScript.enemyAudio.PlayOneShot(attackSound, 1f * enemyScript.gameManager.soundVolume);
        animator.SetBool("isAttacking", true);
        enemyScript.canTurn = false;
        canAttack = false;

        // stop animation
        yield return new WaitForSeconds(attackAnimation.length);

        animator.SetBool("isAttacking", false);
        enemyScript.canTurn = true;

        // can attack
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        enemyAiScript = GetComponent<FollowEnemyAI>();
        animator = GetComponent<Enemy>().animator;

        StartCoroutine(StartAttackCooldown());
        IEnumerator StartAttackCooldown()
        {
            yield return new WaitForSeconds(enemyScript.spawningAnimation.length);
            canAttack = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentPlayerDistance = Vector2.Distance(enemyScript.player.transform.position, this.transform.position);
        // Base Mace Attack
        if (canAttack && currentPlayerDistance < playerDistance && enemyScript.onGround)
        {
            StartCoroutine(Attack(attackCooldown));
        }
    }
}
