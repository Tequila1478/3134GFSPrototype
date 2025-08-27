using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectData))]

public class ObjectBehaviour : MonoBehaviour, IClickable, IHoverable
{  
    [Header("Reference to the object being moved")]
    public ObjectData objectData;

    private IObjectStates _currentState;
    private void Start()
    {
        objectData = gameObject.GetComponent<ObjectData>();
        ChangeState(new ObjectStateIdle());
    }

    public void ChangeState(IObjectStates newState)
    {
        _currentState?.Exit(); // Call Exit on previous state if it exists
        _currentState = newState;
        _currentState.Enter(this, objectData); // Call Enter on new state
    }

    private void Update()
    {
        _currentState.Execute();
    }

    public void OnClick()
    {
        _currentState.Click();
    }

    public void OnHoverEnter()
    {
        _currentState.HoverEnter();
    }

    public void OnHoverExit()
    {
        _currentState.HoverExit();
    }

    public void OnRelease()
    {
        _currentState.Release();
    }

    public void HighlightObject()
    {
        Debug.Log("Object highlight needs logic");
    }

    public void UnHighlightObject()
    {
        Debug.Log("Object un highlighted, logic needed");
    }
}
