using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thornbush : MonoBehaviour
{
    public int damage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            Debug.Log("Player Hit by Thornbush!");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            Debug.Log("Player Hit by Thornbush!");
        }
    }
}
