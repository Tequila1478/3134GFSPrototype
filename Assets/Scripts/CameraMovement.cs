using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;


public class CameraMovement : MonoBehaviour
{
    private class HiddenObjectData
    {
        public Renderer renderer;
        public int originalLayer;
    }
    private List<HiddenObjectData> hiddenObjects = new List<HiddenObjectData>();

    public Transform target; // Object to orbit around
    public float orbitSpeed = 50f;
    public float zoomSpeed = 10f;
    public float screenEdgeThreshold = 50f;
    public bool invert = true;
    public float scrollSensitivity = 100.0f;
    public float minZoomDistance = 2f; // Minimum allowed distance to target

    public string ignoreMouseRaycastLayerName = "IgnoreMouseRaycast"; // Name of the layer to use
    private int ignoreMouseRaycastLayer;

    private float initialDistance;
    private PlayerInteraction pi;

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
        pi = FindObjectOfType<PlayerInteraction>();
        if (target == null)
        {
            Debug.LogError("NO target for camera");
            return;
        }

        ignoreMouseRaycastLayer = LayerMask.NameToLayer(ignoreMouseRaycastLayerName);
        if (ignoreMouseRaycastLayer == -1)
        {
            Debug.LogError($"Layer '{ignoreMouseRaycastLayerName}' not found! Please create it in Unity.");
        }

        // Store initial distance to limit zooming out
        initialDistance = Vector3.Distance(transform.position, target.position);

        // Store the initial vertical angle at the start
        originalVerticalAngle = transform.eulerAngles.x;
    }

    void Update()
    {
        if (target == null) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (pi.itemHeld)
            {
                pi.itemHeld.oi.FreezeObject(); // Freeze immediately on shift press
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (pi.itemHeld)
            {
                pi.itemHeld.oi.ReEnableObject(); // Re-enable on shift release
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            HandleHorizontalOrbit();
            HandleZoom();
            HandleVerticalRotation();
        }

        HandleMouseScrollZoom(); // Mouse scroll doesn't require Shift

        HandleVisibility();
    }

    void HandleVisibility()
    {
        // Raycast forward from the camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, detectionRange, targetLayers);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            GameObject obj = hit.collider.gameObject;

            if (rend != null && !hiddenObjects.Exists(h => h.renderer == rend))
            {
                int originalLayer = obj.layer;
                obj.layer = ignoreMouseRaycastLayer;

                //rend.enabled = false;
                obj.SetActive(false);

                hiddenObjects.Add(new HiddenObjectData
                {
                    renderer = rend,
                    originalLayer = originalLayer
                });
            }
        }

        // Re-enable objects
        for (int i = hiddenObjects.Count - 1; i >= 0; i--)
        {
            Renderer rend = hiddenObjects[i].renderer;
            if (rend == null)
            {
                hiddenObjects.RemoveAt(i);
                continue;
            }

            if (Vector3.Distance(transform.position, rend.transform.position) > detectionRange)
            {
                GameObject obj = rend.gameObject;
                obj.layer = hiddenObjects[i].originalLayer;
                //rend.enabled = true;
                obj.SetActive(true);
                hiddenObjects.RemoveAt(i);
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

        // Only allow zooming if new distance is between min and initial distances
        if (newDistance >= minZoomDistance && newDistance <= initialDistance)
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
