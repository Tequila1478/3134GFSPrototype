using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CameraCinemaSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    public int initialCamera = 0; //The index of the starting camera; is used in inspector-related logic
    private int currentCamera = 0; //Tracks the index of the current camera in the scene

    public CinemachineVirtualCamera calendarCamera;

    private int camOneIncrement = 0;
    private int previousCamera = -1;
    public TextMeshProUGUI debugText;

    public IInteractable currentFocused;

    // OnValidate is called whenever a value for this script is updated in the inspector
    private void OnValidate()
    {
        if (initialCamera > cameras.Length - 1) //Prevent invalid initialcamera 
        {
            initialCamera = cameras.Length - 1;
        }

        if (currentCamera != initialCamera) //Check if initial camera num has been changed in inspector
        {
            cameras[currentCamera].m_Priority = 8; //Set previous "initial camera" to lower priority
            cameras[initialCamera].m_Priority = 10; //Set new "initial camera" to higher priority
            currentCamera = initialCamera; //Update script to start with this "initial camera"
        }
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

    public void SetNewCamera(int newIndex, bool force = false)
    {
        if (force || currentCamera != newIndex) //Check if initial camera num has been changed in inspector
        {
            previousCamera = currentCamera;

            cameras[currentCamera].m_Priority = 8; //Set previous "initial camera" to lower priority
            cameras[newIndex].m_Priority = 10; //Set new "initial camera" to higher priority
            currentCamera = newIndex; //Update script to start with this "initial camera"
        }
    }

    public void SwitchToCalendarCamera()
    {
        // Lower the priority of the current camera
        cameras[currentCamera].m_Priority = 8;

        // Raise the priority of the calendar camera
        calendarCamera.m_Priority = 10;
    }

    public void BackCamera()
    {
        if (previousCamera >= 0)
        {
            SetNewCamera(previousCamera);
        }
        else
            SetNewCamera(0);

        if (currentFocused != null)
        {
            currentFocused.EndInteraction();
        }

    }
}
