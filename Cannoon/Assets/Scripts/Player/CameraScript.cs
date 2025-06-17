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
    public Camera uiCamera;
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
            elapsedTime += Time.unscaledDeltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = pos + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.localPosition = pos;
    }

    // thanks chat gpt!
    void MouseFollow()
    {
        RectTransform cursor = mouseCursor.GetComponent<RectTransform>();

        // Convert screen position to world position on canvas
        Vector3 screenPos = Input.mousePosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cursor.parent as RectTransform,
            screenPos,
            uiCamera,
            out Vector3 targetPos
        );

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

        if (movement.sqrMagnitude > 0.01f)
        {
            float targetTilt = Mathf.Clamp(movement.x, -1f, 1f) * maxTiltAngle;
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.unscaledDeltaTime * mouseRotationSmooth);
        }
        else
        {
            currentTilt = Mathf.Lerp(currentTilt, 0f, Time.unscaledDeltaTime * mouseRotationSmooth);
        }

        cursor.rotation = Quaternion.Euler(0f, 0f, -currentTilt);
    }
}