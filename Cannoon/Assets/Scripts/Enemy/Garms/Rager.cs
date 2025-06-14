using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rager : MonoBehaviour
{
    public bool enraged;
    public AudioClip enragedAudio;
    public float dashForce;
    // Start is called before the first frame update
    void Start()
    {
        // can't move or do damage
        GetComponent<Enemy>().canMove = false;
        GetComponent<Enemy>().canDealDamage = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if hit can move & deal damage
        if (GetComponent<Enemy>().health < GetComponent<Enemy>().maxHealth && !enraged)
        {
            enraged = true;
            GetComponent<FollowEnemyAI>().DashForward(dashForce);
            GetComponent<AudioSource>().PlayOneShot(enragedAudio, GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume * 1f);
            GetComponent<Enemy>().canMove = true;
            GetComponent<Enemy>().canDealDamage = true;
            GetComponent<Enemy>().animator.SetBool("rage", true);
        }
    }

}
