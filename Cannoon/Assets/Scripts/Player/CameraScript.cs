using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 pos;

    public AnimationCurve curve;
    public bool screenshaking;
    private void Start()
    {
        pos = transform.localPosition;
    }
    public IEnumerator Screenshake(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration && !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().dead)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = pos + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = pos;
    }
}
