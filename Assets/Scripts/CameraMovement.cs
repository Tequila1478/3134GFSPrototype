using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;

public class CameraMovement : MonoBehaviour
{
    public Transform target; // Object to orbit around
    public float orbitSpeed = 50f;
    public float zoomSpeed = 10f;
    public float screenEdgeThreshold = 50f;
    public bool invert = true;
    public float scrollSensitivity = 100.0f;

    private float initialDistance;

    //Deals with the visibility of objects in front of the camera

    public float detectionRange = 10f; // Range to detect and hide objects
    public LayerMask targetLayers; // Layer mask to define which objects to hide

    private List<Renderer> invisibleRenderers = new List<Renderer>(); // Track rendered objects to make visible again

    // Camera vertical rotation settings
    public float verticalRotationSpeed = 0.05f; // Speed for vertical rotation
    public float maxVerticalAngle = 40f;  // Maximum vertical angle from the original angle
    private float originalVerticalAngle; // The initial vertical angle of the camera

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("NO target for camera");
            return;
        }

        // Store initial distance to limit zooming out
        initialDistance = Vector3.Distance(transform.position, target.position);

        // Store the initial vertical angle at the start
        originalVerticalAngle = transform.eulerAngles.x;
    }

    void Update()
    {
        if (target == null) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            HandleHorizontalOrbit();
            HandleZoom();
            HandleVerticalRotation();
        }

        HandleMouseScrollZoom(); // Mouse scroll doesn't require Shift



        // Raycast forward from the camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, detectionRange, targetLayers);

        // Hide objects that are within the detection range
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null && rend.enabled)
            {
                if (!invisibleRenderers.Contains(rend))
                {
                    rend.enabled = false;
                    invisibleRenderers.Add(rend);
                }
            }
        }

        // Check if objects are beyond detection range and make them visible again
        foreach (Renderer rend in invisibleRenderers.ToArray()) // Using ToArray() to avoid modification during iteration
        {
            if (rend != null && Vector3.Distance(transform.position, rend.transform.position) > detectionRange)
            {
                rend.enabled = true;
                invisibleRenderers.Remove(rend); // Remove from the list to stop checking
            }
        }
    }

    void HandleHorizontalOrbit()
    {
        float horizontalInput = 0f;

        // A / D keys
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        // Mouse edge of screen
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < screenEdgeThreshold)
            horizontalInput = -1f;
        else if (mousePos.x > Screen.width - screenEdgeThreshold)
            horizontalInput = 1f;

        if (horizontalInput != 0f)
        {
            float direction = invert ? -1f : 1f;
            transform.RotateAround(target.position, Vector3.up, horizontalInput * orbitSpeed * Time.deltaTime * direction);
        }
    }

    void HandleZoom()
    {
        float zoomInput = 0f;

        // W / S keys
        if (Input.GetKey(KeyCode.W)) zoomInput = -1f;
        else if (Input.GetKey(KeyCode.S)) zoomInput = 1f;

        if (zoomInput != 0f)
        {
            ZoomCamera(zoomInput);
        }
    }

    void HandleMouseScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            ZoomCamera(-scroll * scrollSensitivity); // Invert to make scroll up = zoom in
        }
    }

    void ZoomCamera(float direction)
    {
        Vector3 dirToTarget = (transform.position - target.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        Vector3 newPosition = transform.position + dirToTarget * direction * zoomSpeed * Time.deltaTime;

        float newDistance = Vector3.Distance(newPosition, target.position);

        // Limit zoom out to initial distance, but allow infinite zoom in
        if (newDistance <= initialDistance || newDistance < distance)
        {
            transform.position = newPosition;
        }
    }

    // Handle vertical rotation of the camera when holding Shift + Q/E or mouse at top/bottom of the screen
    void HandleVerticalRotation()
    {
        if (Input.GetKey(KeyCode.LeftShift)) // Ensure Shift is held for vertical rotation
        {
            float verticalInput = 0f;

            // Check if Q (down) or E (up) keys are pressed while holding Shift
            if (Input.GetKey(KeyCode.Q)) verticalInput = -1f;
            else if (Input.GetKey(KeyCode.E)) verticalInput = 1f;

            // Handle mouse vertical movement (top/bottom of screen) with Shift held
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.y <= screenEdgeThreshold)
                verticalInput = 1f;
            else if (mousePos.y >= Screen.height - screenEdgeThreshold)
                verticalInput = -1f;

            if (verticalInput != 0f)
            {
                RotateCameraVertically(verticalInput);
            }
        }
    }

    void RotateCameraVertically(float direction)
    {
        // Get the current rotation in the x-axis (vertical angle)
        float currentAngle = transform.eulerAngles.x;

        // Calculate the new angle based on the direction
        float newAngle = currentAngle + direction * verticalRotationSpeed;

        // Ensure the new vertical rotation stays within the clamp range from original angle
        float clampedAngle = Mathf.Clamp(newAngle, originalVerticalAngle - maxVerticalAngle, originalVerticalAngle + maxVerticalAngle);

        // Apply the new vertical rotation while keeping the horizontal rotation unchanged
        transform.eulerAngles = new Vector3(clampedAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

}
