using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableWhenInSpecialCamera : MonoBehaviour
{
    public bool active = false;
    private CameraCinemaSwitch css;
    public int specialCameraNum = 3;

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
        }

        else
        {
            active = false;
        }
        this.gameObject.GetComponent<MeshRenderer>().enabled = active;
    }
}
