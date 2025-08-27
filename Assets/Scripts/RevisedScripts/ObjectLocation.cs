using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLocation : MonoBehaviour, IHoverable, IClickable
{
    public bool isTrashcan = false;
    //reference to the object in this spot
    public ObjectData claimedObject;

    public void OnClick()
    {
        throw new System.NotImplementedException();
    }

    public void OnHoverEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnRelease()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
