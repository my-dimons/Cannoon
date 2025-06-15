using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 pos;

    public AnimationCurve curve;
    public bool screenshaking;
    public Camera[] applyFov;

    [Header("Mouse Cursor")]
    public GameObject cursorCanvas;
    public GameObject mouseCursor;
    public float mouseSmoothSpeed;
    public float mouseRotationSmooth;
    public float maxTiltAngle;

    private Vector3 currentVelocity;
    private float currentTilt;

    private void Start()
    {
        //activeMouseCursor = Instantiate(mouseCursor, cursorCanvas.transform);
        pos = transform.localPosition;
        Cursor.visible = false;
    }

    private void Update()
    {
        for (int i = 0; i < applyFov.Length; i++)
            applyFov[i].fieldOfView = Camera.main.fieldOfView;
        MouseFollow();
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

    // thanks chat gpt!
    void MouseFollow()
    {
        Vector3 targetPos = Input.mousePosition;
        RectTransform cursor = mouseCursor.GetComponent<RectTransform>();

        // Smoothly move the cursor
        cursor.position = Vector3.SmoothDamp(
            cursor.position,
            targetPos,
            ref currentVelocity,
            1f / mouseSmoothSpeed,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );


        // Get the movement direction
        Vector3 movement = currentVelocity;

        // Only rotate if there's movement
        if (movement.sqrMagnitude > 0.01f)
        {
            // Angle to tilt based on horizontal movement only
            float targetTilt = Mathf.Clamp(movement.x, -1f, 1f) * maxTiltAngle;

            // Smoothly interpolate rotation
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.unscaledDeltaTime * mouseRotationSmooth);
        }
        else
        {
            // Slowly return to neutral rotation when not moving
            currentTilt = Mathf.Lerp(currentTilt, 0f, Time.unscaledDeltaTime * mouseRotationSmooth);
        }

        // Apply rotation (only around Z axis for 2D tilt)
        cursor.rotation = Quaternion.Euler(0f, 0f, -currentTilt);
    }
}