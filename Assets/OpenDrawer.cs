using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;

public class OpenDrawer : MonoBehaviour, IClickable, IHoverable
{
    public Animator animator;
    public string animatorBoolName;
    public bool animatorState = false;
    public GameObject[] toggledObjects = new GameObject[0];

    public CinemachineVirtualCamera specialCamera;

    // Update is called once per frame
    void Update()
    {
        if (animatorState && (CameraCinemaSwitch.FindIndexOfSpecialCamera(specialCamera) != CameraCinemaSwitch.instance.currentSpecialCamera))
        {
            OnClick(false);
        }
    }

    public void OnClick()
    {
        OnClick(!animatorState);
    }
    public void OnClick(bool active)
    {
        animatorState = active;
        animator.SetBool(animatorBoolName, animatorState);

        ToggleObjects(animatorState);
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleObjects(animatorState);
    }

    public void ToggleObjects(bool boolean)
    {
        // Set active of toggled objects
        foreach (GameObject obj in toggledObjects)
        {
            obj.SetActive(boolean);
        }
    }

    public void OnHoverEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnRelease()
    {
        //throw new System.NotImplementedException();
    }
}
