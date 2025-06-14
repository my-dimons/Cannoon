using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public float cooldown;
    public bool canHeal;
    public AnimationClip healing;
    public AudioClip healingSound;
    public float healAnimLength;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        canHeal = false;
        yield return new WaitForSeconds(cooldown);
        canHeal = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canHeal)
        {
            StartCoroutine(GetComponent<Enemy>().FreezeEnemy(healing.length));
            StartCoroutine(HealAllEnemies());
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator HealAllEnemies()
    {
        GetComponent<Enemy>().animator.SetBool("isHealing", true);

        yield return new WaitForSeconds(healAnimLength);

        GetComponent<AudioSource>().PlayOneShot(healingSound, GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().soundVolume * 1f);
        Debug.Log("Healing All Enemies");
        foreach (Transform child in GameObject.FindGameObjectWithTag("EndlessModeGameManager").GetComponent<EndlessMode>().enemyParentObject.transform)
        {
            GameObject childGo = child.gameObject;
            Enemy enemy = childGo.GetComponent<Enemy>();
            if (enemy != null)
            {
                float healAmount = enemy.maxHealth - enemy.health;
                enemy.Heal(healAmount);
            }
        }

        GetComponent<Enemy>().animator.SetBool("isHealing", false);
    }
}
