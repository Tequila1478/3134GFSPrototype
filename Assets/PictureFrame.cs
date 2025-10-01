using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureFrame : MonoBehaviour
{
    public bool active = false;
    private CameraCinemaSwitch css;
    public int specialCameraNum = 3;

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
