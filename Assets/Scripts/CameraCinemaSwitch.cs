using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraCinemaSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    public int initialCamera = 0; //The index of the starting camera; is used in inspector-related logic
    private int currentCamera = 0; //Tracks the index of the current camera in the scene

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("keycode Alpha1 pressed");
            SetNewCamera(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("keycode Alpha2 pressed");
            SetNewCamera(1);
        }
    }

    public void SetNewCamera(int newIndex)
    {
        if (currentCamera != newIndex) //Check if initial camera num has been changed in inspector
        {
            cameras[currentCamera].m_Priority = 8; //Set previous "initial camera" to lower priority
            cameras[newIndex].m_Priority = 10; //Set new "initial camera" to higher priority
            currentCamera = newIndex; //Update script to start with this "initial camera"
        }
    }
}
