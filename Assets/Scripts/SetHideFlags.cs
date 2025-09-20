using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHideFlags : MonoBehaviour
{
    public HideFlags setHideFlags = HideFlags.None;

    // Start is called before the first frame update
    void Start()
    {
        hideFlags = setHideFlags; // Set hide flags of the object to whatever is set in inspector
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
