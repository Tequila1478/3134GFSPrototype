using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightenPictureFrame : MonoBehaviour, IHoverable, IClickable
{
    [Header("Target Settings")]
    public Transform targetObject;          
    public Vector3 targetRotationEuler;
    public Vector3 rotationAxis = new Vector3(1, 0, 0);

    [Header("Rotation Settings")]
    public float dragSpeed = 100f;
    public float stickiness = 2f;
    private bool isDragging = false;
    private Vector3 lastMousePos;

    public bool active = false;
    private CameraCinemaSwitch css;
    public int specialCameraNum = 3;

    public void OnClick()
    {
        if (!active || targetObject == null) return;

        // Toggle drag mode
        isDragging = true;

        lastMousePos = Input.mousePosition;
        Debug.Log("Begin drag rotate");

    }

    public void OnHoverEnter() { }
    public void OnHoverExit() { }
    public void OnRelease() {

        isDragging = false;

    }

    void Start()
    {
        css = FindObjectOfType<CameraCinemaSwitch>();
    }

    // Update is called once per frame
    void Update()
    {
        if (css.currentSpecialCamera == specialCameraNum)
        {
            active = true;
        }

        else
        {
            active = false;
        }

        if (isDragging && targetObject != null)
        {
            // Get mouse delta
            Vector3 mouseDelta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            // Rotate around the desired axis (here X)
            float rotationAmount = mouseDelta.x * dragSpeed * Time.deltaTime;
            targetObject.Rotate(rotationAxis, rotationAmount, Space.Self);

            // Convert rotation to -180..180 for clamping
            Vector3 euler = targetObject.localEulerAngles;
            euler.x = Mathf.Repeat(euler.x + 180f, 360f) - 180f;

            // Hard clamp
            euler.x = Mathf.Clamp(euler.x, -89f, 89f);

            // Stickiness: snap to target if within threshold
            float delta = Mathf.DeltaAngle(euler.x, targetRotationEuler.x);
            if (Mathf.Abs(delta) <= stickiness)
            {
                euler.x = targetRotationEuler.x;
            }

            // Apply final rotation
            targetObject.localEulerAngles = euler;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f; // convert to -180..180
        return Mathf.Clamp(angle, min, max);
    }
}
