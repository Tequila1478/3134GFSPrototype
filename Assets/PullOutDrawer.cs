using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullOutDrawer : MonoBehaviour, IHoverable, IClickable
{
    public bool active = false;

    public CinemachineVirtualCamera specialCamera;

    public void OnClick()
    {
    }

    public void OnHoverEnter()
    {
    }

    public void OnHoverExit()
    {
    }

    public void OnRelease()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetDrawerActive(CameraCinemaSwitch.instance.currentSpecialCamera == CameraCinemaSwitch.FindIndexOfSpecialCamera(specialCamera));
    }

    void SetDrawerActive(bool boolean)
    {
        active = boolean;
        GetComponent<BoxCollider>().enabled = boolean;
    }


}
