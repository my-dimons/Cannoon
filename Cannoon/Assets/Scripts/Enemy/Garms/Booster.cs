using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public float cooldown;
    bool canDash;
    public float dashForce;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cooldown(GetComponent<FollowEnemyAI>().spawningAnimation.length * 1.5f));
    }

    IEnumerator Cooldown(float time)
    {
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (canDash)
        {
            GetComponent<FollowEnemyAI>().DashForward(dashForce);
            StartCoroutine(Cooldown(cooldown));
            StartCoroutine(AnimCooldown());
        }
    }

    IEnumerator AnimCooldown()
    {
        GetComponent<FollowEnemyAI>().animator.SetBool("isDashing", true);
        yield return new WaitForSeconds(0.5f);
        GetComponent<FollowEnemyAI>().animator.SetBool("isDashing", false);
    }
}
