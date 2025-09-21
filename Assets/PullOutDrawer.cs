using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullOutDrawer : MonoBehaviour, IHoverable, IClickable
{
    public bool active = false;
    private CameraCinemaSwitch css;
    public int specialCameraNum = 1;

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
        css = FindObjectOfType<CameraCinemaSwitch>();
    }

    // Update is called once per frame
    void Update()
    {
        if (css.currentSpecialCamera == specialCameraNum)
        {
            active = true;
            GetComponent<BoxCollider>().enabled = true;
        }
        
        else
        {
            active = false;
            GetComponent<BoxCollider>().enabled = false;
        }
    }


}
