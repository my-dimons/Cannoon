using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class Grunt : MonoBehaviour
{
    Enemy enemyScript;
    FollowEnemyAI enemyAiScript;
    Animator animator;
    [Tooltip("Assign this variable the baseMaceAttack animation clip")]
    public AnimationClip baseMaceAttack;

    [Header("Base Mace Attack")]
    public float attackCooldown;
    public bool canAttack;
    [Tooltip("How close the player needs to be for this enemy to use this attack")]
    public float playerDistance;

    IEnumerator BaseMaceAttack(float cooldown)
    {
        canAttack = false;
        StartCoroutine(GetComponent<FollowEnemyAI>().FreezeEnemy(baseMaceAttack.length));
        animator.SetBool("isAttacking", true);

        // stop animation
        yield return new WaitForSeconds(baseMaceAttack.length);

        animator.SetBool("isAttacking", false);

        // can attack
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<Enemy>();
        enemyAiScript = GetComponent<FollowEnemyAI>();
        animator = GetComponent<FollowEnemyAI>().animator;

        StartCoroutine(StartAttackCooldown());
        IEnumerator StartAttackCooldown()
        {
            yield return new WaitForSeconds(enemyAiScript.spawningAnimation.length);
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
            StartCoroutine(BaseMaceAttack(attackCooldown));
        }
    }
}
