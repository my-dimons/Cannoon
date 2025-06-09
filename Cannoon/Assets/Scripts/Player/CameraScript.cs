using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 pos;

    public AnimationCurve curve;
    public bool screenshaking;
    public Camera[] applyFov;
    private void Start()
    {
        pos = transform.localPosition;
    }

    private void Update()
    {
        for (int i = 0; i < applyFov.Length; i++)
            applyFov[i].fieldOfView = Camera.main.fieldOfView;
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
