using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStateMoveToLoc : IObjectStates
{
    public void Click()
    {
        throw new System.NotImplementedException();
    }

    public void Enter(ObjectBehaviour ob, ObjectData obj)
    {
        //Remove from selected
        //Remove selected object
    }

    public void Execute()
    {
        //Move to Location

        //At the end enter idle again?
        //Maybe a new state?
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void HoverEnter()
    {
        throw new System.NotImplementedException();
    }

    public void HoverExit()
    {
        throw new System.NotImplementedException();
    }

    public void Release()
    {
        throw new System.NotImplementedException();
    }
}
