using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTime;
    public bool beingDestroyed;

    private void Start()
    {
        StartCoroutine(DestroyAfterSetTime());
    }
    public IEnumerator DestroyAfterSetTime()
    {
        beingDestroyed = true;
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
