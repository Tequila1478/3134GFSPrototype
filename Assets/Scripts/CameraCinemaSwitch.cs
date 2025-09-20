using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CameraCinemaSwitch : MonoBehaviour
{
    [Header("UI")]
    public GameObject cycleCameras;
    public GameObject backCamera;

    [Header("Standard Cameras")]
    [Tooltip("Use this list for cameras that can be accessed through the number keys, or the arrows on both sides of the screen.")]
    public CinemachineVirtualCamera[] cameras;
    [Range(0, 9)]
    [SerializeField] private int initialCamera = 0; //The index of the starting camera; is used in inspector-related logic
    [NonSerialized] public int currentCamera = 0; //Tracks the index of the current camera in the scene

    [Header("Special Cameras")]
    [Tooltip("Use this list for any cameras that can only be accessed by clicking a certain object in the scene.")]
    public CinemachineVirtualCamera[] specialCameras;
    [Range(-1, 9)]
    [SerializeField] private int initialSpecialCamera = -1; //Also used in inspector-related logic
    [NonSerialized] public int currentSpecialCamera = -1;

    [Header("Debug")]
    public TextMeshProUGUI debugText;

    //Bee note: idk what this is for lmao
    public IInteractable currentFocused;

    // OnValidate is called whenever a value for this script is updated in the inspector
    private void OnValidate()
    {
        if (initialCamera > cameras.Length - 1) //Prevent invalid initialcamera 
        {
            initialCamera = cameras.Length - 1;
        }

        foreach (CinemachineVirtualCamera camera in cameras)
        {
            camera.m_Priority = 8; //Set all cameras to lower priority
        }

        if (initialSpecialCamera > specialCameras.Length - 1) //Prevent invalid initialspecialcamera 
        {
            initialSpecialCamera = specialCameras.Length - 1;
        }

        foreach (CinemachineVirtualCamera specialCamera in specialCameras)
        {
            specialCamera.m_Priority = 8; //Set all special cameras to lower priority
        }

        //Set initial camera
        if (initialSpecialCamera > -1) //Check if initial special camera num has been set in inspector
        {
            specialCameras[initialSpecialCamera].m_Priority = 10; //Set special "initial camera" to higher priority
        }
        else
        {
            cameras[initialCamera].m_Priority = 10; //Set standard "initial camera" to higher priority
        }
        currentCamera = initialCamera; //Update script to start with currently set "initial camera"
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (debugText != null) // Display debug information
        {
            debugText.text = "Current camera: " + (currentCamera+1).ToString();
        }

        // Check if any button from 1-9 is pressed...
        int i = 0;
        for (KeyCode key = KeyCode.Alpha1; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyDown(key)) //If number key is pressed, go to that camera
            {
                Debug.Log($"Key {key} was pressed!");
                SetNewCamera(i, true);
            }
            i++;
        }

        // ...then check for number key 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log($"Key {KeyCode.Alpha0} was pressed!");
            SetNewCamera(9);
        }

        // Set camera cycle UI
        if (currentSpecialCamera == -1)
        {
            //Do cycling between standard cameras if no special camera is active
            cycleCameras.SetActive(true);
            backCamera.SetActive(false);
        } else
        { //Do cycling for special camera if one is active
            cycleCameras.SetActive(false);
            backCamera.SetActive(true);
        }
    }

    public void NextCamera()
    {
        int newIndex = (currentCamera + 1) % cameras.Length; // wraps back to 0
        SetNewCamera(newIndex);
    }

    public void PreviousCamera()
    {
        int newIndex = (currentCamera - 1 + cameras.Length) % cameras.Length; // wraps to last camera
        SetNewCamera(newIndex);
    }

    // This is used to set a new standard camera, using a specific index.
    public void SetNewCamera(int newIndex, bool force = false)
    {
        if (force || currentCamera != newIndex) //Check if there is a new active camera (or a camera is being forced to be active)
        {
            cameras[currentCamera].m_Priority = 8; //Set previous camera to lower priority
            if (currentSpecialCamera > -1) specialCameras[currentSpecialCamera].m_Priority = 8; //Also set special camera to lower priority if it is currently active
            cameras[newIndex].m_Priority = 10; //Set new camera to higher priority

            currentCamera = newIndex; //Remember the new camera index
            currentSpecialCamera = -1; //Reset special camera index
        }
    }

    // This is used to set a new special camera.
    // This function will take a given virtual camera, and see if that virtual camera is registered as a special camera in the camera switch.
    // If so, another function is called to switch to that special camera's index.
    public void EnterSpecialCamera(CinemachineVirtualCamera virtualCamera)
    {
        int newIndex = ArrayUtility.IndexOf(specialCameras, virtualCamera);
        if (newIndex != -1)
        {
            EnterSpecialCamera(newIndex);
        }
    }

    // This is used to set a new special camera, using a specific index.
    public void EnterSpecialCamera(int newIndex)
    {
        // Lower the priority of the current camera
        cameras[currentCamera].m_Priority = 8;

        // Raise the priority of the calendar camera
        specialCameras[newIndex].m_Priority = 10;

        //Update index
        currentSpecialCamera = newIndex;
    }

    public void LeaveSpecialCamera()
    {
        SetNewCamera(currentCamera, true);

        // End interaction with an IInteractable
        if (currentFocused != null)
        {
            currentFocused.EndInteraction();
        }

    }
}
