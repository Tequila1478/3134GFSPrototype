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

    public TextMeshProUGUI debugText;

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

        // Bee note: too lazy to figure out a loopable alternative to this
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("keycode Alpha1 pressed");
            camOneIncrement++;
            if (camOneIncrement == 1)
                SetNewCamera(0, true);
            else if (camOneIncrement == 2)
                SetNewCamera(1, true);
            else if (camOneIncrement == 3)
                SetNewCamera(2, true);
            else
            {
                camOneIncrement = 0;
                SetNewCamera(0, true);
            }
           
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("keycode Alpha2 pressed");
            SetNewCamera(1);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("keycode Alpha3 pressed");
            SetNewCamera(2);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("keycode Alpha4 pressed");
            camOneIncrement++;
            if (camOneIncrement == 1)
                SetNewCamera(3, true);
            else if (camOneIncrement == 2)
                SetNewCamera(4, true);
            else if (camOneIncrement == 3)
                SetNewCamera(5, true);
            else
            {
                camOneIncrement = 0;
                SetNewCamera(3, true);
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("keycode Alpha5 pressed");
            SetNewCamera(4);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("keycode Alpha6 pressed");
            SetNewCamera(5);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log("keycode Alpha7 pressed");
            SetNewCamera(6);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Debug.Log("keycode Alpha8 pressed");
            SetNewCamera(7);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("keycode Alpha9 pressed");
            SetNewCamera(8);
            camOneIncrement = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("keycode Alpha0 pressed");
            SetNewCamera(9);
            camOneIncrement = 0;
        }
    }

    public void SetNewCamera(int newIndex, bool force = false)
    {
        if (force || currentCamera != newIndex) //Check if initial camera num has been changed in inspector
        {
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
}
