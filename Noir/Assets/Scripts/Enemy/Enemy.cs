using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float damage;

    public float minDifficulty;
    public float maxDifficulty;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            GetComponent<EnemyAI>().target.gameObject.GetComponent<Player>().kills += 1;
        }
    }
}
