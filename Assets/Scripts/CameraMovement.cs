using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Vector2 mouseOrbitMultiplier = new(2.5f, 2.5f);
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
    
    // Enum for three styles of camera movement
    private enum CameraMovementMode
    {
        Keyboard, //Shift + Keys
        EdgeMouse, //Shift + mouse (at edge of screen)
        RightClickMouse //Right-click + mouse
    }

    // Debug
    public bool debug = false;
    public TMP_Text debugText;

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

        // Camera controls
        if (Input.GetKey(KeyCode.LeftShift) && (Input.mousePosition.x < screenEdgeThreshold || Input.mousePosition.x > Screen.width - screenEdgeThreshold || Input.mousePosition.y < screenEdgeThreshold || Input.mousePosition.y > Screen.height - screenEdgeThreshold)) //Only continue if left shift modifier is pressed
        {
            //Shift + mouse
            HandleHorizontalOrbit(CameraMovementMode.EdgeMouse); //Do horizontal rotation
            HandleVerticalRotation(CameraMovementMode.EdgeMouse); //Do vertical rotation
            Cursor.lockState = CursorLockMode.Confined; //Confine mouse to screen
        }
        else if (Input.GetMouseButton(1)) //Right-click + mouse
        {
            HandleHorizontalOrbit(CameraMovementMode.RightClickMouse); //Do horizontal rotation
            HandleVerticalRotation(CameraMovementMode.RightClickMouse); //Do vertical rotation
            Cursor.lockState = CursorLockMode.Confined; //Confine mouse to screen
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; //Do not confine mouse to screen
            if (Input.GetKey(KeyCode.LeftShift)) //Only continue if left shift modifier is pressed
            {
                //Shift + keys
                HandleHorizontalOrbit(CameraMovementMode.Keyboard); //Do horizontal rotation
                HandleVerticalRotation(CameraMovementMode.Keyboard); //Do vertical rotation
                HandleZoom(CameraMovementMode.Keyboard); //Do zoom
            }
        }

        HandleZoom(CameraMovementMode.RightClickMouse); // Mouse scroll doesn't require Shift or Right-click

        HandleVisibility(); //Check for objects to hide from camera view

        //Debug
        if (debug && debugText) //Only do this if debug is enabled and there is a text object to write to. Note that this is *NOT* using Debug.Log()
        {
            debugText.text = transform.eulerAngles.ToString(); //Write current camera angle to debug text
        }
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

    float mousePosX = Input.GetAxis("Mouse X");
    float mousePosY = Input.GetAxis("Mouse Y");
    void HandleHorizontalOrbit(CameraMovementMode orbitMode)
    {
        float horizontalInput = 0f; //Initialise variable to store potential input

        switch(orbitMode)
        {
            //Shift + mouse
            case (CameraMovementMode.EdgeMouse):
                Vector3 mousePos = Input.mousePosition; //Get current X,Y,Z position of mouse cursor
                if (mousePos.x < screenEdgeThreshold) //Read left input if mouse is at left side of screen
                    horizontalInput = -1f;
                else if (mousePos.x > Screen.width - screenEdgeThreshold) //Read right input if mouse is at right side of screen
                    horizontalInput = 1f;
                break;
            //Right-click + mouse
            case (CameraMovementMode.RightClickMouse): 
                float mouseNewX = Input.GetAxis("Mouse X"); //Get current X position of mouse cursor
                horizontalInput = mouseNewX - mousePosX; //Read amount of change in cursor X position this frame
                horizontalInput *= mouseOrbitMultiplier.x; //Apply multiplier to input
                break;
            //Shift + keys
            default:
                if (Input.GetKey(KeyCode.A)) horizontalInput = -1f; //Read left button
                else if (Input.GetKey(KeyCode.D)) horizontalInput = 1f; //Read right button
                break;
        }

        // Apply input if it was found
        if (horizontalInput != 0f)
        {
            float direction = invert ? -1f : 1f; //Invert direction if needed
            transform.RotateAround(target.position, Vector3.up, horizontalInput * orbitSpeed * Time.deltaTime * direction); //Apply rotation around target orbit point
        }
    }

    // Handle vertical rotation of the camera when holding Shift + Q/E or mouse at top/bottom of the screen
    void HandleVerticalRotation(CameraMovementMode orbitMode)
    {
        float verticalInput = 0f; //Initialise variable to store potential input

        switch (orbitMode)
        {
            //Shift + mouse
            case (CameraMovementMode.EdgeMouse):
                if (Input.GetKey(KeyCode.LeftShift)) //Only run if left shift modifier is pressed
                {
                    Vector3 mousePos = Input.mousePosition; //Get current X,Y,Z position of mouse cursor
                    if (mousePos.y <= screenEdgeThreshold) //Read up input if mouse is at top side of screen
                        verticalInput = 1f;
                    else if (mousePos.y >= Screen.height - screenEdgeThreshold) //Read down input if mouse is at bottom side of screen
                        verticalInput = -1f;
                }
                break;
            //Right-click + mouse
            case (CameraMovementMode.RightClickMouse):
                float mouseNewY = Input.GetAxis("Mouse Y"); //Get current Y position of mouse cursor
                verticalInput = mousePosY - mouseNewY; //Read amount of change in cursor Y position this frame
                verticalInput *= mouseOrbitMultiplier.y; //Apply multiplier to input
                break;
            //Shift + keys
            default:// Check if Q (down) or E (up) keys are pressed while holding Shift
                if (Input.GetKey(KeyCode.LeftShift)) //Only run if left shift modifier is pressed
                {
                    if (Input.GetKey(KeyCode.Q)) verticalInput = -1f; //Read down button
                    else if (Input.GetKey(KeyCode.E)) verticalInput = 1f; //Read up button
                }
                break;
        }

        // Apply input if it was found
        if (verticalInput != 0f)
        {
            RotateCameraVertically(verticalInput); //Call function for applying rotation
        }
    }

    void RotateCameraVertically(float direction)
    {
        // Get the current rotation in the x-axis (vertical angle)
        float currentAngle = transform.eulerAngles.x;

        // Calculate the new angle based on the direction
        float newAngle = currentAngle + direction * verticalRotationSpeed;

        // Make new angle negative if it is over 180 degrees (necessary for the clamp to work)
        if (newAngle > 180)
            newAngle = newAngle - 360;

        // Ensure the new vertical rotation stays within the clamp range from original angle
        float clampedAngle = Mathf.Clamp(newAngle, originalVerticalAngle - maxVerticalAngle, originalVerticalAngle + maxVerticalAngle);

        // Apply the new vertical rotation while keeping the horizontal rotation unchanged
        transform.eulerAngles = new Vector3(clampedAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void HandleZoom(CameraMovementMode orbitMode)
    {
        float zoomInput = 0f; //Initialise variable to store potential input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        switch (orbitMode)
        {
            //Both [Shift + mouse] and [Right-click + mouse]
            case (CameraMovementMode.EdgeMouse):
            case (CameraMovementMode.RightClickMouse):
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    ZoomCamera(-scroll * scrollSensitivity); // Invert to make scroll up = zoom in
                }
                break;
            //Shift + keys
            default:
                if (Input.GetKey(KeyCode.W)) zoomInput = -1f; //Read up button
                else if (Input.GetKey(KeyCode.S)) zoomInput = 1f; //Read down button
                break;
        }

        // Apply input if it was found
        if (zoomInput != 0f)
        {
            ZoomCamera(zoomInput); //Call function for applying zoom
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

}
