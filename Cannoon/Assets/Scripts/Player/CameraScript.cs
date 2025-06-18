using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    Vector3 pos;

    public AnimationCurve curve;
    public bool screenshaking;
    public Camera[] applyFov;

    [Header("Mouse Cursor")]
    public GameObject crosshairCanvas;
    public GameObject crosshair;
    public Sprite[] crosshairs;
    public TMP_Dropdown crosshairDropdown;
    public Camera[] uiCamera;
    public float mouseSmoothSpeed;
    public float mouseRotationSmooth;
    public float maxTiltAngle;

    private Vector3 currentVelocity;
    private float currentTilt;

    private void Start()
    {
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
        RectTransform cursor = crosshair.GetComponent<RectTransform>();

        // Convert screen position to world position on canvas
        Vector3 screenPos = Input.mousePosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            cursor.parent as RectTransform,
            screenPos,
            uiCamera[1],
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