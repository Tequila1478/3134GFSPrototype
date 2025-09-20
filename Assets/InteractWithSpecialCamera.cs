using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithSpecialCamera : MonoBehaviour, IHoverable, IClickable, IInteractable
{
    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;
    private Color[] originalColors;
    private Color hoverColor = Color.white;

    private bool isFocusedOn = false;
    private bool _isFocusedOn = false;

    public CameraCinemaSwitch cameraController;
    public CinemachineVirtualCamera specialCamera;
    public GameObject cycleCameras;
    public GameObject backCamera;

    private InspectItem ii;

    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            renderers = new Renderer[] { rend };
        }

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].sharedMaterial; // or .material if you want a unique instance
            if (mat != null && mat.HasProperty("_OutlineColour"))
            {
                originalColors[i] = mat.GetColor("_OutlineColour");
            }
            else
            {
                originalColors[i] = Color.clear; // or some default value
            }
        }
        ii = GetComponent<InspectItem>();

    }

    private void Update()
    {
        /*
        if(isFocusedOn != _isFocusedOn)
        {
            GetComponent<BoxCollider>().enabled = _isFocusedOn;
            _isFocusedOn = isFocusedOn;
        }*/
    }

    public void OnClick()
    {
        StartSpecialView(); // Start special camera view when clicked on
    }

    public void OnHoverEnter()
    {
        HighlightObject();
    }

    public void OnHoverExit()
    {
        UnhighlightObject();
    }

    public void OnRelease()
    {
    }

    public void StartSpecialView()
    {
        if (!isFocusedOn)
        {
            cameraController.EnterSpecialCamera(specialCamera); // Activate special camera to focus on sometime else
            isFocusedOn = true; //Switch focus mode
        }
        else
        {
            ii.OnClick(); //Run OnClick on InspectItem component to play dialogue
        }
        cycleCameras.SetActive(false); // Disable cycling between standard cameras
        backCamera.SetActive(true); // Enable button for leaving special camera
    }

    public void EndSpecialView()
    {
        cameraController.LeaveSpecialCamera(); // Deactivate special camera view
        cycleCameras.SetActive(true); // Enable cycling between standard cameras
        backCamera.SetActive(false); // Disable button for leaving special camera

        isFocusedOn = false; //Switch focus mode
    }

    private void HighlightObject()
    {
        foreach (var rend in renderers)
        {
            var mpb = new MaterialPropertyBlock();
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", hoverColor);
            rend.SetPropertyBlock(mpb);
        }
    }

    private void UnhighlightObject()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var mpb = new MaterialPropertyBlock();
            var rend = renderers[i];
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_OutlineColour", originalColors[i]);
            rend.SetPropertyBlock(mpb);
        }
    }

    private void PlayAnim()
    {
        
    }

    public void EndInteraction()
    {
        isFocusedOn = false;
        if (cameraController != null && cameraController.currentFocused == this)
            cameraController.currentFocused = null;

    }

    public void BeginInteraction()
    {
        throw new System.NotImplementedException();
    }
}
